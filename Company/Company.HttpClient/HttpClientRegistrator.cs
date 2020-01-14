using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Company.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;

namespace Company.HttpClient
{
  /// <summary>
  /// Класс-строитель для создания и регистрации Http-клиентов в стандартном DI
  /// </summary>
  public class HttpClientRegistrator
  {
    private const int defaultTimeout = 30;

    protected HttpClientRegistrator()
    {
      _addHandlerActions = new List<Action<IServiceCollection, IHttpClientBuilder>>();
    }

    /// <summary>
    /// Создает экземпляр данного регистратора
    /// </summary>
    /// <returns></returns>
    public static HttpClientRegistrator Create()
    {
      return new HttpClientRegistrator();
    }

    protected string _url { get; set; }

    /// <summary>
    /// Устанавливает базовый адрес
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public HttpClientRegistrator WithUrl(string url)
    {
      _url = url;
      return this;
    }

    protected string _name { get; set; }

    /// <summary>
    /// Задает имя клиента. Должно быть уникально
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public HttpClientRegistrator WithName(string name)
    {
      _name = name;
      return this;
    }

    private int? _secondsTimeout { get; set; }

    /// <summary>
    /// Устанавливает таймаут. По-умолчанию 30 секунд
    /// </summary>
    /// <param name="secondsTimeout"></param>
    /// <returns></returns>
    public HttpClientRegistrator WithTimeout(int? secondsTimeout)
    {
      _secondsTimeout = secondsTimeout;
      return this;
    }

    private readonly List<Action<IServiceCollection, IHttpClientBuilder>> _addHandlerActions;

    /// <summary>
    /// Регистрирует стандартное middleware, без привязки к стороннему контейнеру
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public HttpClientRegistrator WithHandler<T>() where T : DelegatingHandler
    {
      _addHandlerActions.Add((services, clientBuilder) =>
      {
        services.TryAddTransient<T>();
        clientBuilder.AddHttpMessageHandler<T>();
      });
      return this;
    }

    private Action<IHttpClientBuilder> _configureBuilderAction;

    /// <summary>
    /// Дополнительно конфигурирует билдер Http-клиентов
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public HttpClientRegistrator ConfigureBuilder(Action<IHttpClientBuilder> builderAction)
    {
      _configureBuilderAction = builderAction;

      return this;
    }

    /// <summary>
    /// Завершает регистрацию Http-клиента в приложении AspNetCore
    /// </summary>
    /// <param name="services"></param>
    public void Register(IServiceCollection services)
    {
      ValidateClientBuilder();

      var baseBuilder = BuildBaseHttpClient(services);
      foreach (var handlerAction in _addHandlerActions)
      {
        handlerAction(services, baseBuilder);
      }
    }

    private IHttpClientBuilder BuildBaseHttpClient(IServiceCollection services)
    {
      var builder = services.AddHttpClient(_name, client =>
                     {
                       client.BaseAddress = new Uri(_url);
                       client.Timeout = TimeSpan.FromSeconds(_secondsTimeout ?? defaultTimeout);
                     });
      _configureBuilderAction?.Invoke(builder);
      builder.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(Enumerable.Repeat(TimeSpan.FromSeconds(1), 3)));

      return builder;
    }

    private void ValidateClientBuilder()
    {
      _name.ThrowIfNull($"httpClient.Name");
      _url.ThrowIfNull($"httpClient.Url");
    }
  }
}
