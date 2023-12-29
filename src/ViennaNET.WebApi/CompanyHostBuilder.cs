using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ViennaNET.WebApi.Cors;
using ViennaNET.WebApi.Metrics;
using ViennaNET.WebApi.StaticFiles;

namespace ViennaNET.WebApi
{
  /// <summary>
  ///   Класс-строитель для создания и конфигурирования WebApi-сервисов
  /// </summary>
  public sealed partial class CompanyHostBuilder
  {
    private readonly List<Action<IApplicationBuilder>> _addMiddlewareActions;

    private readonly List<(Action<IApplicationBuilder, IConfiguration, IHostEnvironment, object> action, bool
        initBeforeContainer)>?
      _appActions;

    private readonly List<Action<IMvcCoreBuilder, IConfiguration>>? _mvcBuilderActions;
    private readonly List<Action<IServiceCollection, IConfiguration>>? _servicesToRegister;
    private readonly List<Action<IServiceCollection, object, IConfiguration>>? _servicesToRegisterInContainer;
    private Action<IServiceCollection, IConfiguration, object>? _configureContainerAction;
    private Func<IServiceCollection, IConfiguration, object>? _configureContainerFunc;
    private Action<MvcOptions>? _configureMvcOptions;

    private object? _container;
    private Func<object>? _createContainerAction;

    private Action<IHostBuilder>? _hostBuilderAction;
    private Action<IConfigurationBuilder>? _configurationBuilderAction;
    private Action<IApplicationBuilder, object, IConfiguration>? _initializeContainerAction;

    private Action<IConfiguration>? _onStartAction;
    private Action<IConfiguration>? _onStoppedAction;
    private Action<IConfiguration>? _onStoppingAction;
    private Action<IWebHostBuilder>? _useServerAction;
    private Action<object>? _verifyContainerAction;

    private CompanyHostBuilder()
    {
      _appActions =
        new List<(Action<IApplicationBuilder, IConfiguration, IHostEnvironment, object> action, bool initBeforeContainer
          )>();
      _addMiddlewareActions = new List<Action<IApplicationBuilder>>();
      _servicesToRegister = new List<Action<IServiceCollection, IConfiguration>>();
      _mvcBuilderActions = new List<Action<IMvcCoreBuilder, IConfiguration>>();
      _servicesToRegisterInContainer = new List<Action<IServiceCollection, object, IConfiguration>>();
    }

    private IHostEnvironment? HostingEnvironment { get; set; }
    private IConfiguration? Configuration { get; set; }

    /// <summary>
    ///   Собирает сервис
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public IHost BuildWebHost(string[] args)
    {
      _container = _createContainerAction?.Invoke();

      var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(builder =>
        {
          _useServerAction?.Invoke(builder);

          builder.ConfigureCompanyMetrics()
            .Configure(ConfigureAppBuilder);
        })
        .ConfigureAppConfiguration((context, builder) =>
        {
          builder
            .AddJsonFile("conf/appsettings.json")
            .AddJsonFile($"conf/appsettings.{context.HostingEnvironment.EnvironmentName}.json", true);

          _configurationBuilderAction?.Invoke(builder);
          Configuration = builder.Build();
        })
        .ConfigureServices(ConfigureServices);

      _hostBuilderAction?.Invoke(hostBuilder);
      var host = hostBuilder.Build();

      return host;
    }

    /// <summary>
    ///   Создает новый экземпляр строителя
    /// </summary>
    /// <returns></returns>
    public static CompanyHostBuilder Create()
    {
      return new();
    }

    private void ConfigureBaseServices(IServiceCollection services)
    {
      var builder = services.AddMvcCore(options =>
        {
          _configureMvcOptions?.Invoke(options);
          options.EnableEndpointRouting = false;
        })
        .AddApiExplorer()
        .AddCors();

      if (Assembly.GetEntryAssembly() is { } assembly)
      {
        builder.AddApplicationPart(assembly);
      }

      _mvcBuilderActions?.ForEach(a => a(builder, Configuration));
      _servicesToRegister?.ForEach(service => service(services, Configuration));
    }

    private void ConfigureServices(IServiceCollection services)
    {
      ConfigureBaseServices(services);
      _configureContainerAction?.Invoke(services, Configuration, _container);
      _servicesToRegisterInContainer?.ForEach(x => x(services, _container, Configuration));
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
      if (_onStartAction is null && _onStoppingAction is null && _onStoppedAction is null)
      {
        return;
      }

      var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
      if (_onStartAction != null)
      {
        lifetime.ApplicationStarted.Register(() => _onStartAction(Configuration));
      }

      if (_onStoppingAction != null)
      {
        lifetime.ApplicationStopping.Register(() => _onStoppingAction(Configuration));
      }

      if (_onStoppedAction != null)
      {
        lifetime.ApplicationStopped.Register(() => _onStoppedAction(Configuration));
      }
    }
  }
}