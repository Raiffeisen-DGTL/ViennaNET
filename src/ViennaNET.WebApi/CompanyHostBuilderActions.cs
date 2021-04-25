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
  public sealed partial class ViennaHostBuilder : IViennaHostBuilder
  {
    /// <summary>
    /// Позволяет дополнительно сконфигурировать MVC Builder
    /// </summary>
    /// <param name="configAction"></param>
    /// <returns></returns>
    public IViennaHostBuilder AddMvcBuilderConfiguration(Action<IMvcCoreBuilder, IConfiguration> configAction)
    {
      _mvcBuilderActions.Add(configAction);
      return this;
    }

    /// <summary>
    /// Позволяет сконфигурировать создание MVC Builder
    /// </summary>
    /// <param name="configureMvcOptions"></param>
    /// <returns></returns>
    public IViennaHostBuilder AddMvcOptionsConfiguration(Action<MvcOptions> configureMvcOptions)
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
    public IViennaHostBuilder ConfigureApp(
      Action<IApplicationBuilder, IConfiguration, IHostEnvironment, object> appConfigurationAction, bool initBeforeContainer = false)
    {
      _appActions.Add((appConfigurationAction, initBeforeContainer));
      return this;
    }

    /// <summary>
    /// Регистрирует стандартное middleware, без привязки к стороннему контейнеру
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IViennaHostBuilder AddMiddleware<T>() where T : IMiddleware
    {
      _addMiddlewareActions.Add(app => app.UseMiddleware<T>());
      return this;
    }

    /// <summary>
    /// Кофигурирует сторонний контейнер для замещения встроенного
    /// </summary>
    /// <param name="configureContainerFunc"></param>
    /// <returns></returns>
    public IViennaHostBuilder ConfigureContainer(Func<IServiceCollection, IConfiguration, object> configureContainerFunc)
    {
      _configureContainerFunc = configureContainerFunc;
      return this;
    }

    /// <summary>
    /// Кофигурирует сторонний контейнер без замещения встроенного
    /// </summary>
    /// <param name="configureContainerAction"></param>
    /// <returns></returns>
    public IViennaHostBuilder ConfigureContainer(Action<IServiceCollection, IConfiguration, object> configureContainerAction)
    {
      _configureContainerAction = configureContainerAction;
      return this;
    }

    /// <summary>
    /// Инициализация стороннего контейнера
    /// </summary>
    /// <param name="initializeContainerAction"></param>
    /// <returns></returns>
    public IViennaHostBuilder InitializeContainer(Action<IApplicationBuilder, object, IConfiguration> initializeContainerAction)
    {
      _initializeContainerAction = initializeContainerAction;
      return this;
    }

    /// <summary>
    /// Регистрация сервисов в стандартном DI
    /// </summary>
    /// <param name="registerServices"></param>
    /// <returns></returns>
    public IViennaHostBuilder RegisterServices(Action<IServiceCollection, IConfiguration> registerServices)
    {
      _servicesToRegister.Add(registerServices);
      return this;
    }

    /// <summary>
    /// Функция для создания объекта DI-контейнера. Если не была вызвана, то используется встроенный контейнер
    /// </summary>
    /// <param name="createContainerAction"></param>
    /// <returns></returns>
    public IViennaHostBuilder CreateContainer(Func<object> createContainerAction)
    {
      _createContainerAction = createContainerAction;
      return this;
    }

    /// <summary>
    /// Функция для вызова валидации стороннего контейнера
    /// </summary>
    /// <param name="verifyContainerAction"></param>
    /// <returns></returns>
    public IViennaHostBuilder VerifyContainer(Action<object> verifyContainerAction)
    {
      _verifyContainerAction = verifyContainerAction;
      return this;
    }

    /// <summary>
    /// Регистрация сервисов в стороннем DI
    /// </summary>
    /// <param name="registerServices"></param>
    /// <returns></returns>
    public IViennaHostBuilder RegisterServices(Action<IServiceCollection, object, IConfiguration> registerServices)
    {
      _servicesToRegisterInContainer.Add(registerServices);
      return this;
    }

    /// <summary>
    /// Настройка сервера
    /// </summary>
    /// <param name="useServerAction"></param>
    /// <returns></returns>
    public IViennaHostBuilder UseServer(Action<IWebHostBuilder> useServerAction)
    {
      _useServerAction = useServerAction;
      return this;
    }

    /// <summary>
    /// Добавляет операцию, вызываемую после старта сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public IViennaHostBuilder AddOnStartAction(Action<IConfiguration> action)
    {
      _onStartAction = action;
      return this;
    }

    /// <summary>
    /// Добавляет операцию, вызываемую после остановки сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public IViennaHostBuilder AddOnStopAction(Action<IConfiguration> action)
    {
      _onStopAction = action;
      return this;
    }

    /// <summary>
    /// Добавляет операцию, вызываемую после старта сервиса, извлекаемую посредством IoC-контейнера и параметра functionToGetAction
    /// </summary>
    /// <param name="functionToGetAction"></param>
    /// <returns></returns>
    public IViennaHostBuilder AddOnStartAction(Func<object, Action> functionToGetAction)
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
    public IViennaHostBuilder AddOnStopAction(Func<object, Action> functionToGetAction)
    {
      _onStopAction = configuration =>
      {
        functionToGetAction(_container).Invoke();
      };
      return this;
    }
  }
}
