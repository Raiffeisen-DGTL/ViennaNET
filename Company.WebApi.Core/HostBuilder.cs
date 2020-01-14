using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Company.Logging;
using Company.WebApi.Core.Cors;
using Company.WebApi.Core.JsonConfiguration;
using Company.WebApi.Core.Metrics;
using Company.WebApi.Core.Net;
using Company.WebApi.Core.Net.IpTools;
using Company.WebApi.Core.StaticFiles;
using Company.WebApi.Core.Swagger;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Company.WebApi.Core
{
  /// <summary>
  /// Класс-строитель для создания и конфигурирования WebApi-сервисов
  /// </summary>
  public sealed partial class HostBuilder
  {
    private readonly Assembly _serviceAssembly;

    private IHostingEnvironment HostingEnvironment { get; set; }
    private IConfiguration Configuration { get; set; }

    private bool SwaggerEnabled { get; set; }

    private object _container;
    private Func<object> _createContainerAction;
    private Action<object> _verifyContainerAction;
    private Action<IWebHostBuilder> _useServerAction;
    private readonly List<Action<IMvcCoreBuilder, IConfiguration>> _mvcBuilderActions;
    private Action<MvcOptions> _configureMvcOptions;

    private readonly List<(Action<IApplicationBuilder, IConfiguration, IHostingEnvironment, object> action, bool initBeforeContainer)>
      _appActions;

    private readonly List<Action<IApplicationBuilder>> _addMiddlewareActions;
    private Func<IServiceCollection, IConfiguration, object> _configureContainerFunc;
    private Action<IServiceCollection, IConfiguration, object> _configureContainerAction;
    private Action<IApplicationBuilder, object> _initializeContainerAction;
    private readonly List<Action<IServiceCollection, IConfiguration>> _servicesToRegister;
    private Action<SwaggerUIOptions, IConfiguration> _configureSwaggerUi;
    private Action<SwaggerGenOptions, IConfiguration> _configureSwaggerGen;

    private Action<IConfiguration> _onStartAction;
    private Action<IConfiguration> _onStopAction;

    private HostBuilder(Assembly serviceAssembly)
    {
      _serviceAssembly = serviceAssembly;

      _appActions =
        new List<(Action<IApplicationBuilder, IConfiguration, IHostingEnvironment, object> action, bool initBeforeContainer)>();
      _addMiddlewareActions = new List<Action<IApplicationBuilder>>();
      _servicesToRegister = new List<Action<IServiceCollection, IConfiguration>>();
      _mvcBuilderActions = new List<Action<IMvcCoreBuilder, IConfiguration>>();
    }

    /// <summary>
    /// Создает новый экземпляр строителя
    /// </summary>
    /// <returns></returns>
    public static HostBuilder Create()
    {
      return new HostBuilder(Assembly.GetEntryAssembly());
    }

    private void ConfigureAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
    {
      HostingEnvironment = hostingContext.HostingEnvironment;
      config.AddJsonFile(Path.GetDirectoryName(_serviceAssembly.Location) + "/conf/appsettings.json");
      Configuration = config.Build();

      var webApiConfiguration = Configuration.GetSection(BaseWebApiConfiguration.SectionName)
                                             .Get<BaseWebApiConfiguration>();

      SwaggerEnabled = webApiConfiguration.SwaggerSubmit ?? true;
    }

    private void ConfigureBaseServices(IServiceCollection services)
    {
      var builder = services.AddMvcCore(options =>
                            {
                              _configureMvcOptions?.Invoke(options);
                            })
                            .AddApiExplorer()
                            .AddApplicationPart(_serviceAssembly)
                            .AddJsonFormatters()
                            .AddCors();

      _mvcBuilderActions.ForEach(a => a(builder, Configuration));

      services.AddSingleton<ILocalIpProvider, LocalIpProvider>();
      services.AddSingleton<ILoopbackIpFilter, LoopbackIpFilter>();

      var name = _serviceAssembly.GetName();
      if (SwaggerEnabled)
      {
        services.AddSwagger($"{name.Name}, version: {name.Version}", o => _configureSwaggerGen?.Invoke(o, Configuration));
      }

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

      _initializeContainerAction?.Invoke(app, _container);

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

      // Swagger
      if (SwaggerEnabled)
      {
        app.ConfigureSwagger(o => _configureSwaggerUi?.Invoke(o, Configuration));
      }

      _verifyContainerAction?.Invoke(_container);

      // configure lifetime actions
      if (_onStartAction is null && _onStopAction is null)
      {
        return;
      }

      var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
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

      var webHostBuilder = WebHost.CreateDefaultBuilder(args);

      _useServerAction(webHostBuilder);
      webHostBuilder.UseContentRoot(pathToContentRoot)
                    .ConfigureAppConfiguration(ConfigureAppConfiguration)
                    .ConfigureServices(ConfigureServices)
                    .ConfigureCompanyMetrics()
                    .Configure(ConfigureAppBuilder)
                    .ConfigureLogging(logBuilder =>
                    {
                      logBuilder.ClearProviders();
                    });


      return webHostBuilder.Build();
    }
  }
}
