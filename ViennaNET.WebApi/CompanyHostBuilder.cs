using ViennaNET.Logging;
using ViennaNET.WebApi.Cors;
using ViennaNET.WebApi.Metrics;
using ViennaNET.WebApi.StaticFiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ViennaNET.WebApi
{
  /// <summary>
  /// Класс-строитель для создания и конфигурирования WebApi-сервисов
  /// </summary>
  public sealed partial class CompanyHostBuilder
  {
    private readonly Assembly _serviceAssembly;

    private IHostEnvironment HostingEnvironment { get; set; }
    private IConfiguration Configuration { get; set; }

    private object _container;
    private Func<object> _createContainerAction;
    private Action<object> _verifyContainerAction;
    private Action<IWebHostBuilder> _useServerAction;
    private readonly List<Action<IMvcCoreBuilder, IConfiguration>> _mvcBuilderActions;
    private Action<MvcOptions> _configureMvcOptions;

    private readonly List<(Action<IApplicationBuilder, IConfiguration, IHostEnvironment, object> action, bool initBeforeContainer)>
      _appActions;

    private readonly List<Action<IApplicationBuilder>> _addMiddlewareActions;
    private Func<IServiceCollection, IConfiguration, object> _configureContainerFunc;
    private Action<IServiceCollection, IConfiguration, object> _configureContainerAction;
    private Action<IApplicationBuilder, object, IConfiguration> _initializeContainerAction;
    private readonly List<Action<IServiceCollection, IConfiguration>> _servicesToRegister;

    private Action<IConfiguration> _onStartAction;
    private Action<IConfiguration> _onStopAction;

    private bool _useDefaultLogging = false;

    private CompanyHostBuilder(Assembly serviceAssembly)
    {
      _serviceAssembly = serviceAssembly;

      _appActions = new List<(Action<IApplicationBuilder, IConfiguration, IHostEnvironment, object> action, bool initBeforeContainer)>();
      _addMiddlewareActions = new List<Action<IApplicationBuilder>>();
      _servicesToRegister = new List<Action<IServiceCollection, IConfiguration>>();
      _mvcBuilderActions = new List<Action<IMvcCoreBuilder, IConfiguration>>();
    }

    /// <summary>
    /// Включает встроенный логгер
    /// </summary>
    /// <returns></returns>
    public CompanyHostBuilder UseDefaultLogging()
    {
      _useDefaultLogging = true;

      return this;
    }

    /// <summary>
    /// Создает новый экземпляр строителя
    /// </summary>
    /// <returns></returns>
    public static CompanyHostBuilder Create()
    {
      return new CompanyHostBuilder(Assembly.GetEntryAssembly());
    }

    private void ConfigureAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
    {
      HostingEnvironment = hostingContext.HostingEnvironment;

      var serviceAssemblyName = _serviceAssembly.GetName();
      var serviceAssemblyProps = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("serviceAssemblyName", serviceAssemblyName.Name),
        new KeyValuePair<string, string>("serviceAssemblyVersion", serviceAssemblyName.Version?.ToString()),
      };

      config.AddInMemoryCollection(serviceAssemblyProps);
      config.AddJsonFile(Path.GetDirectoryName(_serviceAssembly.Location) + "/conf/appsettings.json");
      Configuration = config.Build();
    }

    private void ConfigureBaseServices(IServiceCollection services)
    {
      var builder = services.AddMvcCore(options =>
                            {
                              _configureMvcOptions?.Invoke(options);
                              options.EnableEndpointRouting = false;
                            })
                            .AddApiExplorer()
                            .AddApplicationPart(_serviceAssembly)
                            .AddCors();

      _mvcBuilderActions.ForEach(a => a(builder, Configuration));

      var name = _serviceAssembly.GetName();
      Logger.DefaultService = name.Name;

      _servicesToRegister.ForEach(service => service(services, Configuration));
    }

    private void ConfigureServices(IServiceCollection services)
    {
      ConfigureBaseServices(services);
      _configureContainerAction?.Invoke(services, Configuration, _container);
    }

    // TODO: нужно доработать, сделать корректные интерфейсы
    private object ConfigureServicesWithContainerReplace(IServiceCollection services)
    {
      ConfigureBaseServices(services);
      return _configureContainerFunc(services, Configuration);
    }

    private void ConfigureAppBuilder(IApplicationBuilder app)
    {
      // Register configurations before initialize container
      foreach (var configAction in _appActions.Where(a => a.initBeforeContainer))
      {
        configAction.action(app, Configuration, HostingEnvironment, _container);
      }

      _initializeContainerAction?.Invoke(app, _container, Configuration);

      CorsConfigurator.Configure(app);

      // Register middlewares
      _addMiddlewareActions.ForEach(m => m(app));

      // Register configurations after initialize container
      foreach (var configAction in _appActions.Where(a => !a.initBeforeContainer))
      {
        configAction.action(app, Configuration, HostingEnvironment, _container);
      }

      // ASP.NET default stuff here
      app.UseMvc();
      app.ConfigureStaticFiles(Configuration);

      _verifyContainerAction?.Invoke(_container);

      // configure lifetime actions
      if (_onStartAction is null && _onStopAction is null)
      {
        return;
      }

      var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
      if (_onStartAction != null)
      {
        lifetime.ApplicationStarted.Register(() => _onStartAction(Configuration));
      }

      if (_onStopAction != null)
      {
        lifetime.ApplicationStopped.Register(() => _onStopAction(Configuration));
      }
    }

    /// <summary>
    /// Собирает сервис
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public IWebHost BuildWebHost(string[] args)
    {
      VerifyBuilderState();

      if (_createContainerAction != null)
      {
        _container = _createContainerAction();
      }

      var pathToExe = Process.GetCurrentProcess()
                             .MainModule.FileName;
      var pathToContentRoot = Path.GetDirectoryName(pathToExe);

      var hostBuilder = WebHost.CreateDefaultBuilder(args);

      _useServerAction(hostBuilder);
      hostBuilder.UseContentRoot(pathToContentRoot)
                 .ConfigureAppConfiguration(ConfigureAppConfiguration)
                 .ConfigureServices(ConfigureServices)
                 .ConfigureCompanyMetrics()
                 .Configure(ConfigureAppBuilder)
                 .ConfigureLogging(logBuilder =>
                 {
                   if (!_useDefaultLogging)
                   {
                     logBuilder.ClearProviders();
                   }
                 });


      return hostBuilder.Build();
    }
  }
}
