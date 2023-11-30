﻿using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ViennaNET.WebApi.Abstractions
{
  public interface ICompanyHostBuilder
  {
    /// <summary>
    ///   Собирает сервис согласно зарегистрированным конфигураторам
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    IHost BuildWebHost(string[] args);

    /// <summary>
    ///   Позволяет дополнительно сконфигурировать MVC Builder
    /// </summary>
    /// <param name="configAction"></param>
    /// <returns></returns>
    ICompanyHostBuilder AddMvcBuilderConfiguration(Action<IMvcCoreBuilder, IConfiguration> configAction);

    /// <summary>
    ///   Позволяет сконфигурировать создание MVC Builder
    /// </summary>
    /// <param name="configureMvcOptions"></param>
    /// <returns></returns>
    ICompanyHostBuilder AddMvcOptionsConfiguration(Action<MvcOptions> configureMvcOptions);

    /// <summary>
    ///   Конфигурирование внутреннего билдера приложения, здесь добавляются Middleware, применяется Cors и т.п.
    /// </summary>
    /// <param name="appConfigurationAction"></param>
    /// <param name="initBeforeContainer">флаг вызова действия до инициализации контейнера</param>
    /// <returns></returns>
    ICompanyHostBuilder ConfigureApp(
      Action<IApplicationBuilder, IConfiguration, IHostEnvironment, object> appConfigurationAction,
      bool initBeforeContainer = false);

    /// <summary>
    ///   Регистрирует стандартное middleware, без привязки к стороннему контейнеру
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    ICompanyHostBuilder AddMiddleware<T>() where T : IMiddleware;

    /// <summary>
    ///   Кофигурирует сторонний контейнер для замещения встроенного
    /// </summary>
    /// <param name="configureContainerFunc"></param>
    /// <returns></returns>
    ICompanyHostBuilder ConfigureContainer(Func<IServiceCollection, IConfiguration, object> configureContainerFunc);

    /// <summary>
    ///   Кофигурирует сторонний контейнер без замещения встроенного
    /// </summary>
    /// <param name="configureContainerAction"></param>
    /// <returns></returns>
    ICompanyHostBuilder ConfigureContainer(Action<IServiceCollection, IConfiguration, object> configureContainerAction);

    /// <summary>
    ///   Инициализация стороннего контейнера
    /// </summary>
    /// <param name="initializeContainerAction"></param>
    /// <returns></returns>
    ICompanyHostBuilder InitializeContainer(
      Action<IApplicationBuilder, object, IConfiguration> initializeContainerAction);

    /// <summary>
    ///   Регистрация сервисов в стандартном DI
    /// </summary>
    /// <param name="registerServices"></param>
    /// <returns></returns>
    ICompanyHostBuilder RegisterServices(Action<IServiceCollection, IConfiguration> registerServices);

    /// <summary>
    ///   Регистрация сервисов в стандартном и/или стороннем DI
    /// </summary>
    /// <param name="registerServices"></param>
    /// <returns></returns>
    ICompanyHostBuilder RegisterServices(Action<IServiceCollection, object, IConfiguration> registerServices);

    /// <summary>
    ///   Функция для создания объекта DI-контейнера. Если не была вызвана, то используется встроенный контейнер
    /// </summary>
    /// <param name="createContainerAction"></param>
    /// <returns></returns>
    ICompanyHostBuilder CreateContainer(Func<object> createContainerAction);

    /// <summary>
    ///   Функция для вызова валидации стороннего контейнера
    /// </summary>
    /// <param name="verifyContainerAction"></param>
    /// <returns></returns>
    ICompanyHostBuilder VerifyContainer(Action<object> verifyContainerAction);

    /// <summary>
    ///   Настройка сервера
    /// </summary>
    /// <param name="useServerAction"></param>
    /// <returns></returns>
    ICompanyHostBuilder UseServer(Action<IWebHostBuilder> useServerAction);

    /// <summary>
    ///   Добавляет операцию, вызываемую после старта сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    ICompanyHostBuilder AddOnStartAction(Action<IConfiguration> action);

    /// <summary>
    ///   Добавляет операцию, вызываемую после старта сервиса, извлекаемую посредством IoC-контейнера и параметра
    ///   functionToGetAction
    /// </summary>
    /// <param name="functionToGetAction"></param>
    /// <returns></returns>
    ICompanyHostBuilder AddOnStartAction(Func<object, Action> functionToGetAction);

    /// <summary>
    ///   Добавляет операцию, вызываемую перед остановкой сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    ICompanyHostBuilder AddOnStoppingAction(Action<IConfiguration> action);

    /// <summary>
    ///   Добавляет операцию, вызываемую перед остановкой сервиса, извлекаемую посредством IoC-контейнера и параметра
    ///   functionToGetAction
    /// </summary>
    /// <param name="functionToGetAction"></param>
    /// <returns></returns>
    ICompanyHostBuilder AddOnStoppingAction(Func<object, Action> functionToGetAction);

    /// <summary>
    ///   Добавляет операцию, вызываемую после остановки сервиса
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    ICompanyHostBuilder AddOnStoppedAction(Action<IConfiguration> action);

    /// <summary>
    ///   Добавляет операцию, вызываемую после остановки сервиса, извлекаемую посредством IoC-контейнера и параметра
    ///   functionToGetAction
    /// </summary>
    /// <param name="functionToGetAction"></param>
    /// <returns></returns>
    ICompanyHostBuilder AddOnStoppedAction(Func<object, Action> functionToGetAction);

    /// <summary>
    ///   Добавляет обработчик ошибок для определенного исключения
    /// </summary>
    /// <typeparam name="TException">Тип исключения</typeparam>
    /// <param name="statusCode">HTTP-код ошибки</param>
    ICompanyHostBuilder AddExceptionHandler<TException>(int statusCode) where TException : Exception;

    /// <summary>
    ///   Добавляет обработчик ошибок для определенного исключения
    /// </summary>
    /// <typeparam name="TException">Тип исключения</typeparam>
    /// <param name="statusCode">HTTP-код ошибки</param>
    public ICompanyHostBuilder AddExceptionHandler<TException>(HttpStatusCode statusCode) where TException : Exception;

    /// <summary>
    ///   Добавляет обработчик ошибок для определенного исключения
    /// </summary>
    /// <param name="exception">Тип исключения</param>
    /// <param name="statusCode">HTTP-код ошибки</param>
    ICompanyHostBuilder AddExceptionHandler(Type exception, int statusCode);

    /// <summary>
    ///   Добавляет обработчик ошибок для определенного исключения
    /// </summary>
    /// <param name="exception">Тип исключения</param>
    /// <param name="statusCode">HTTP-код ошибки</param>
    ICompanyHostBuilder AddExceptionHandler(Type exception, HttpStatusCode statusCode);

    /// <summary>
    /// Добавляет регистрацию обработчиков на IHostBuilder
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    ICompanyHostBuilder RegisterHostBuilderAction(Action<IHostBuilder> hostBuilder);

    /// <summary>
    /// Добавляет регистрацию обработчиков на IConfigurationBuilder
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <returns></returns>
    ICompanyHostBuilder RegisterConfigurationBuilderAction(Action<IConfigurationBuilder> configurationBuilder);
  }
}