using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi
{
  public sealed partial class CompanyHostBuilder : ICompanyHostBuilder
  {
    /// <summary>
    ///   Позволяет дополнительно сконфигурировать MVC Builder
    /// </summary>
    /// <param name="configAction"></param>
    /// <returns></returns>
    public ICompanyHostBuilder AddMvcBuilderConfiguration(Action<IMvcCoreBuilder, IConfiguration> configAction)
    {
      _mvcBuilderActions.Add(configAction);
      return this;
    }

    /// <summary>
    ///   Позволяет сконфигурировать создание MVC Builder
    /// </summary>
    /// <param name="configureMvcOptions"></param>
    /// <returns></returns>
    public ICompanyHostBuilder AddMvcOptionsConfiguration(Action<MvcOptions> configureMvcOptions)
    {
      _configureMvcOptions = configureMvcOptions;
      return this;
    }

    /// <summary>
    ///   Конфигурирование внутреннего билдера приложения, здесь добавляются Middleware, применяется Cors и т.п.
    /// </summary>
    /// <param name="appConfigurationAction"></param>
    /// <param name="initBeforeContainer">флаг вызова действия до инициализации контейнера</param>
    /// <returns></returns>
    public ICompanyHostBuilder ConfigureApp(
      Action<IApplicationBuilder, IConfiguration, IHostEnvironment, object> appConfigurationAction,
      bool initBeforeContainer = false)
    {
      _appActions.Add((appConfigurationAction, initBeforeContainer));
      return this;
    }

    /// <summary>
    ///   Регистрирует стандартное middleware, без привязки к стороннему контейнеру
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ICompanyHostBuilder AddMiddleware<T>() where T : IMiddleware
    {
      _addMiddlewareActions.Add(app => app.UseMiddleware<T>());
      return this;
    }

    /// <summary>
    ///   Кофигурирует сторонний контейнер для замещения встроенного
    /// </summary>
    /// <param name="configureContainerFunc"></param>
    /// <returns></returns>
    public ICompanyHostBuilder ConfigureContainer(
      Func<IServiceCollection, IConfiguration, object> configureContainerFunc)
    {
      _configureContainerFunc = configureContainerFunc;
      return this;
    }

    /// <summary>
    ///   Кофигурирует сторонний контейнер без замещения встроенного
    /// </summary>
    /// <param name="configureContainerAction"></param>
    /// <returns></returns>
    public ICompanyHostBuilder ConfigureContainer(
      Action<IServiceCollection, IConfiguration, object> configureContainerAction)
    {
      _configureContainerAction = configureContainerAction;
      return this;
    }

    /// <summary>
    ///   Инициализация стороннего контейнера
    /// </summary>
    /// <param name="initializeContainerAction"></param>
    /// <returns></returns>
    public ICompanyHostBuilder InitializeContainer(
      Action<IApplicationBuilder, object, IConfiguration> initializeContainerAction)
    {
      _initializeContainerAction = initializeContainerAction;
      return this;
    }

    /// <summary>
    ///   Регистрация сервисов в стандартном DI
    /// </summary>
    /// <param name="registerServices"></param>
    /// <returns></returns>
    public ICompanyHostBuilder RegisterServices(Action<IServiceCollection, IConfiguration> registerServices)
    {
      _servicesToRegister.Add(registerServices);
      return this;
    }

    /// <summary>
    ///   Функция для создания объекта DI-контейнера. Если не была вызвана, то используется встроенный контейнер
    /// </summary>
    /// <param name="createContainerAction"></param>
    /// <returns></returns>
    public ICompanyHostBuilder CreateContainer(Func<object> createContainerAction)
    {
      _createContainerAction = createContainerAction;
      return this;
    }

    /// <summary>
    ///   Функция для вызова валидации стороннего контейнера
    /// </summary>
    /// <param name="verifyContainerAction"></param>
    /// <returns></returns>
    public ICompanyHostBuilder VerifyContainer(Action<object> verifyContainerAction)
    {
      _verifyContainerAction = verifyContainerAction;
      return this;
    }

    /// <summary>
    ///   Регистрация сервисов в стороннем DI
    /// </summary>
    /// <param name="registerServices"></param>
    /// <returns></returns>
    public ICompanyHostBuilder RegisterServices(Action<IServiceCollection, object, IConfiguration> registerServices)
    {
      _servicesToRegisterInContainer.Add(registerServices);
      return this;
    }

    /// <summary>
    ///   Настройка сервера
    /// </summary>
    /// <param name="useServerAction"></param>
    /// <returns></returns>
    public ICompanyHostBuilder UseServer(Action<IWebHostBuilder> useServerAction)
    {
      _useServerAction = useServerAction;
      return this;
    }

    /// <summary>
    ///   Добавляет операцию, вызываемую после старта сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public ICompanyHostBuilder AddOnStartAction(Action<IConfiguration> action)
    {
      _onStartAction = action;
      return this;
    }

    /// <summary>
    ///   Добавляет операцию, вызываемую перед остановкой сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public ICompanyHostBuilder AddOnStoppingAction(Action<IConfiguration> action)
    {
      _onStoppingAction = action;
      return this;
    }

    /// <summary>
    ///   Добавляет операцию, вызываемую после остановки сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public ICompanyHostBuilder AddOnStoppedAction(Action<IConfiguration> action)
    {
      _onStoppedAction = action;
      return this;
    }

    /// <summary>
    ///   Добавляет операцию, вызываемую после старта сервиса, извлекаемую посредством IoC-контейнера и параметра
    ///   functionToGetAction
    /// </summary>
    /// <param name="functionToGetAction"></param>
    /// <returns></returns>
    public ICompanyHostBuilder AddOnStartAction(Func<object, Action> functionToGetAction)
    {
      _onStartAction = configuration =>
      {
        functionToGetAction(_container).Invoke();
      };
      return this;
    }

    /// <summary>
    ///   Добавляет операцию, вызываемую перед остановкой сервиса, извлекаемую посредством IoC-контейнера и параметра
    ///   functionToGetAction
    /// </summary>
    /// <param name="functionToGetAction"></param>
    /// <returns></returns>
    public ICompanyHostBuilder AddOnStoppingAction(Func<object, Action> functionToGetAction)
    {
      _onStoppingAction = configuration =>
      {
        functionToGetAction(_container).Invoke();
      };
      return this;
    }

    /// <summary>
    ///   Добавляет операцию, вызываемую после остановки сервиса, извлекаемую посредством IoC-контейнера и параметра
    ///   functionToGetAction
    /// </summary>
    /// <param name="functionToGetAction"></param>
    /// <returns></returns>
    public ICompanyHostBuilder AddOnStoppedAction(Func<object, Action> functionToGetAction)
    {
      _onStoppedAction = configuration =>
      {
        functionToGetAction(_container).Invoke();
      };
      return this;
    }

    /// <summary>
    /// Добавляет регистрацию обработчиков на IHostBuilder
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public ICompanyHostBuilder RegisterHostBuilderAction(Action<IHostBuilder> hostBuilder)
    {
      _hostBuilderAction = hostBuilder;
      return this;
    }
    
    /// <summary>
    /// Добавляет регистрацию обработчиков на IConfigurationBuilder
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <returns></returns>
    public ICompanyHostBuilder RegisterConfigurationBuilderAction(Action<IConfigurationBuilder> configurationBuilder)
    {
        _configurationBuilderAction = configurationBuilder;
        return this;
    }
  }
}