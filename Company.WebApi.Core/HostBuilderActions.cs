using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Company.WebApi.Core
{
  public sealed partial class HostBuilder
  {
    /// <summary>
    /// Позволяет дополнительно сконфигурировать MVC Builder
    /// </summary>
    /// <param name="configAction"></param>
    /// <returns></returns>
    public HostBuilder AddMvcBuilderConfiguration(Action<IMvcCoreBuilder, IConfiguration> configAction)
    {
      _mvcBuilderActions.Add(configAction);
      return this;
    }

    /// <summary>
    /// Позволяет сконфигурировать создание MVC Builder
    /// </summary>
    /// <param name="configureMvcOptions"></param>
    /// <returns></returns>
    public HostBuilder AddMvcOptionsConfiguration(Action<MvcOptions> configureMvcOptions)
    {
      _configureMvcOptions = configureMvcOptions;
      return this;
    }

    /// <summary>
    /// Конфигурирование внутреннего билдера приложения, здесь добавляются Middleware, применяется Cors и т.п.
    /// </summary>
    /// <param name="appConfigurationAction"></param>
    /// <param name="initBeforeContainer">флаг вызова действия до инициализации контейнера</param>
    /// <returns></returns>
    public HostBuilder ConfigureApp(
      Action<IApplicationBuilder, IConfiguration, IHostingEnvironment, object> appConfigurationAction, bool initBeforeContainer = false)
    {
      _appActions.Add((appConfigurationAction, initBeforeContainer));
      return this;
    }

    /// <summary>
    /// Регистрирует стандартное middleware, без привязки к стороннему контейнеру
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public HostBuilder AddMiddleware<T>() where T : IMiddleware
    {
      _addMiddlewareActions.Add(app => app.UseMiddleware<T>());
      return this;
    }

    /// <summary>
    /// Кофигурирует сторонний контейнер для замещения встроенного
    /// </summary>
    /// <param name="configureContainerFunc"></param>
    /// <returns></returns>
    public HostBuilder ConfigureContainer(Func<IServiceCollection, IConfiguration, object> configureContainerFunc)
    {
      _configureContainerFunc = configureContainerFunc;
      return this;
    }

    /// <summary>
    /// Кофигурирует сторонний контейнер без замещения встроенного
    /// </summary>
    /// <param name="configureContainerAction"></param>
    /// <returns></returns>
    public HostBuilder ConfigureContainer(Action<IServiceCollection, IConfiguration, object> configureContainerAction)
    {
      _configureContainerAction = configureContainerAction;
      return this;
    }

    /// <summary>
    /// Инициализация стороннего контейнера
    /// </summary>
    /// <param name="initializeContainerAction"></param>
    /// <returns></returns>
    public HostBuilder InitializeContainer(Action<IApplicationBuilder, object> initializeContainerAction)
    {
      _initializeContainerAction = initializeContainerAction;
      return this;
    }

    /// <summary>
    /// Регистрация сервисов в стандартном DI
    /// </summary>
    /// <param name="registerServices"></param>
    /// <returns></returns>
    public HostBuilder RegisterServices(Action<IServiceCollection, IConfiguration> registerServices)
    {
      _servicesToRegister.Add(registerServices);
      return this;
    }

    /// <summary>
    /// Функция для создания объекта DI-контейнера. Если не была вызвана, то используется встроенный контейнер
    /// </summary>
    /// <param name="createContainerAction"></param>
    /// <returns></returns>
    public HostBuilder CreateContainer(Func<object> createContainerAction)
    {
      _createContainerAction = createContainerAction;
      return this;
    }

    /// <summary>
    /// Функция для вызова валидации стороннего контейнера
    /// </summary>
    /// <param name="varifyContainerAction"></param>
    /// <returns></returns>
    public HostBuilder VerifyContainer(Action<object> varifyContainerAction)
    {
      _verifyContainerAction = varifyContainerAction;
      return this;
    }

    /// <summary>
    /// Настройка сервера
    /// </summary>
    /// <param name="useServerAction"></param>
    /// <returns></returns>
    public HostBuilder UseServer(Action<IWebHostBuilder> useServerAction)
    {
      _useServerAction = useServerAction;
      return this;
    }

    /// <summary>
    /// Добавляет операцию, вызываемую после старта сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public HostBuilder AddOnStartAction(Action<IConfiguration> action)
    {
      _onStartAction = action;
      return this;
    }

    /// <summary>
    /// Добавляет операцию, вызываемую после остановки сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public HostBuilder AddOnStopAction(Action<IConfiguration> action)
    {
      _onStopAction = action;
      return this;
    }

    /// <summary>
    /// Добавляет операцию, вызываемую после старта сервиса, извлекаемую посредством IoC-контейнера и параметра functionToGetAction
    /// </summary>
    /// <param name="functionToGetAction"></param>
    /// <returns></returns>
    public HostBuilder AddOnStartAction(Func<object, Action> functionToGetAction)
    {
      _onStartAction = configuration =>
      {
        functionToGetAction(_container).Invoke();
      };
      return this;
    }

    /// <summary>
    /// Добавляет операцию, вызываемую после остановки сервиса, извлекаемую посредством IoC-контейнера и параметра functionToGetAction 
    /// </summary>
    /// <param name="functionToGetAction"></param>
    /// <returns></returns>
    public HostBuilder AddOnStopAction(Func<object, Action> functionToGetAction)
    {
      _onStopAction = configuration =>
      {
        functionToGetAction(_container).Invoke();
      };
      return this;
    }

    /// <summary>
    /// Дополнительная конфигурация Swagger
    /// </summary>
    /// <param name="swaggerUi"></param>
    /// <param name="swaggerGen"></param>
    /// <returns></returns>
    public HostBuilder AddSwaggerConfigurations(
      Action<SwaggerUIOptions, IConfiguration> swaggerUi, Action<SwaggerGenOptions, IConfiguration> swaggerGen)
    {
      _configureSwaggerGen = swaggerGen;
      _configureSwaggerUi = swaggerUi;

      return this;
    }
  }
}
