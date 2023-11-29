using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using Polly.Timeout;
using ViennaNET.Extensions.Http.Logging;

namespace ViennaNET.Extensions.Http.DependencyInjection;

/// <summary>
///     Предоставляет методы расширения для регистрации HTTP клиента.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Константы, являются частью открытого API.")]
public static class HttpClientServiceCollectionExtensions
{
    /// <summary>
    ///     Уникальное название стратегии повтора по умолчанию.
    /// </summary>
    public const string DefaultRetryStrategy = "DefaultRetryStrategy";

    /// <summary>
    ///     Уникальное название стратегии прерывания запроса по истечению времени.
    /// </summary>
    public const string DefaultTimeoutStrategy = "DefaultTimeoutStrategy";

    /// <summary>
    ///     Уникальное название, стратегии повтора, не осуществляющей какие-либо повторные попытки.
    /// </summary>
    public const string NoOpStrategy = "NoOpStrategy";

    /// <summary>
    ///     Добавляет <see cref="IHttpClientFactory" /> и связанные службы в <see cref="IServiceCollection" />
    ///     и настраивает привязку между типом <typeparamref name="TClient" /> и именованным <see cref="HttpClient" />.
    ///     Имя клиента будет установлено на имя типа <typeparamref name="TClient" />.
    ///     Добавляет реестр политик <see cref="IPolicyRegistry{TKey}" /> с помощью, содержащий 3 политики:
    ///     <see cref="DefaultRetryStrategy" />, <see cref="DefaultTimeoutStrategy" />,
    ///     <see cref="NoOpStrategy" />, после чего регистрирует соответствующие обработчики и настраивая их с помощью
    ///     параметров <typeparamref name="TOption" />.
    /// </summary>
    /// <param name="services">Ссылка на объект <see cref="IServiceCollection" />.</param>
    /// <param name="configure">Действие, которое настраивает <typeparamref name="TOption" /></param>
    /// <returns>Ссылка на объект <see cref="IServiceCollection" />.</returns>
    public static IHttpClientBuilder AddHttpClient<TClient,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TImplementation, TOption>(
        this IServiceCollection services,
        Action<TOption>? configure = null)
        where TClient : class
        where TImplementation : class, TClient
        where TOption : ClientOptionsBase
    {
        var optBuilder = services.AddOptions<TOption>();

        if (configure is not null)
        {
            optBuilder.Configure(configure);
        }

        return services
            .AddDefaultPolicyRegistry<TOption>()
            .AddHttpClient<TClient, TImplementation>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<TOption>>().Value;

                client.BaseAddress = new Uri(options.BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(options.OverallTimeout);
            })
            .AddDefaultRetryPolicyHandlers<TOption>()
            .AddDefaultTimeoutPolicyHandlers<TOption>();
    }

    /// <summary>
    ///     Добавить <see cref="LoggingHttpMessageBodyHandler" />.
    /// </summary>
    /// <remarks>
    ///     Обработчик добавляет возможность записи полезной нагрузки запросов и ответов в систему ведения журналов.
    ///     Запись в журнал ведётся, только при включеном уровне ведения журнала <see cref="LogLevel.Trace" />.
    ///     <para>
    ///         Используйте эту функцию с осторожностью, чтобы избежать утечек конфиденциальной информации или сбоя в
    ///         в работе приложения (отказ в обслуживании) если тело сообщение очень большое.
    ///         <b>Не допускайте ситуации, когда <see cref="LogLevel.Trace" /> включен в производственной среде!</b>
    ///     </para>
    /// </remarks>
    /// <param name="builder">Ссылка на <see cref="IHttpClientBuilder" />.</param>
    /// <returns>Ссылка на <see cref="IHttpClientBuilder" />.</returns>
    public static IHttpClientBuilder AddLoggingHttpMessageBodyHandler(this IHttpClientBuilder builder)
    {
        builder.Services.TryAddTransient<LoggingHttpMessageBodyHandler>();
        builder.AddHttpMessageHandler<LoggingHttpMessageBodyHandler>();

        return builder;
    }

    /// <summary>
    ///     Добавить <see cref="IPolicyRegistry{TKey}" /> по умолчанию.
    /// </summary>
    /// <remarks>
    ///     Реестр политик по умолчанию, включает три политики: <see cref="DefaultRetryStrategy" />,
    ///     <see cref="DefaultTimeoutStrategy" /> и <see cref="NoOpStrategy" />. Параметры политик определяются
    ///     в <typeparamref name="TOption" />. Сам по себе реестр не влияет на работу HTTP клиента,
    ///     это просто способ организовать политики, для переиспользования и управления. Чтобы применить политики,
    ///     добавьте соответствующие обработчики <see cref="AddDefaultRetryPolicyHandlers{T}" />,
    ///     <see cref="AddDefaultTimeoutPolicyHandlers{T}" /> или самостоятельно с помощью
    ///     <see cref="PollyHttpClientBuilderExtensions" />.
    ///     Вы можете настроить политики как показано в примерах.
    ///     <para>
    ///         <b>Удалить политику</b>.
    ///         <code>
    ///             var registry = provider.GetService&lt;IPolicyRegistry&lt;string&gt;&gt;();
    ///             registry.Remove(DefaultTimeoutStrategy);
    ///         </code>
    ///         <br />
    ///         <b>Заменить политику</b>.
    ///         <code>
    ///             var registry = provider.GetService&lt;IPolicyRegistry&lt;string&gt;&gt;();
    ///             registry[DefaultRetryStrategy] = HttpPolicyExtensions.HandleTransientHttpError()
    ///                 .WaitAndRetryAsync(retryCount,
    ///                     retryAttempt => TimeSpan.FromSeconds(Math.Pow(retryDelay, retryAttempt)));
    ///         </code>
    ///         <br />
    ///         <b>Добавить политику</b>.
    ///         <code>
    ///             var registry = provider.GetService&lt;IPolicyRegistry&lt;string&gt;&gt;();
    ///             registry.Add("PolicyName", HttpPolicyExtensions.HandleTransientHttpError()
    ///                 .WaitAndRetryAsync(retryCount,
    ///                     retryAttempt => TimeSpan.FromSeconds(Math.Pow(retryDelay, retryAttempt))));
    ///         </code>
    ///     </para>
    /// </remarks>
    /// <param name="services">Ссылка на объект <see cref="IServiceCollection" />.</param>
    /// <typeparam name="TOption">Тип параметров унаследованный от <see cref="ClientOptionsBase" />.</typeparam>
    /// <returns>Ссылка на объект <see cref="IServiceCollection" />.</returns>
    public static IServiceCollection AddDefaultPolicyRegistry<TOption>(this IServiceCollection services)
        where TOption : ClientOptionsBase
    {
        return services.AddPolicyRegistry((provider, pairs) =>
        {
            var options = provider.GetRequiredService<IOptions<TOption>>().Value;

            pairs.Add(DefaultRetryStrategy, GetRetryPolicy(options.RetryCount, options.RetryDelay));
            pairs.Add(DefaultTimeoutStrategy, Policy.TimeoutAsync<HttpResponseMessage>(options.TryTimeout));
            pairs.Add(NoOpStrategy, Policy.NoOpAsync<HttpResponseMessage>());
        });
    }

    /// <summary>
    ///     Добавляет обработчики политик повтора по умолчанию, зарегистрированных в <see cref="IPolicyRegistry{T}" />.
    /// </summary>
    /// <remarks>
    ///     Для регистрации обработчиков, требуется реестр <see cref="IPolicyRegistry{T}" />, содержащий политики с
    ///     именами <see cref="DefaultRetryStrategy" />, <see cref="NoOpStrategy" />.
    ///     Для добавления такого реестра в контейнер внедрения зависимостей,
    ///     вызовите <see cref="AddDefaultPolicyRegistry{T}" /> или зарегистрируйте его самостоятельно.
    /// </remarks>
    /// <param name="clientBuilder">Ссылка на объект <see cref="IHttpClientBuilder" />.</param>
    /// <typeparam name="TOption">Тип параметров унаследованный от <see cref="ClientOptionsBase" />.</typeparam>
    /// <returns>Ссылка на объект <see cref="IHttpClientBuilder" />.</returns>
    public static IHttpClientBuilder AddDefaultRetryPolicyHandlers<TOption>(this IHttpClientBuilder clientBuilder)
        where TOption : ClientOptionsBase
    {
        return clientBuilder
            .AddPolicyHandler((provider, message) =>
            {
                var registry = provider.GetRequiredService<IReadOnlyPolicyRegistry<string>>();
                var options = provider.GetRequiredService<IOptions<TOption>>().Value;
                var retryPolicy = registry.Get<IAsyncPolicy<HttpResponseMessage>>(DefaultRetryStrategy);
                var noOpPolicy = registry.Get<IAsyncPolicy<HttpResponseMessage>>(NoOpStrategy);
                var onlyIdempotent = options.UseReplayPolicyOnlyIdempotentRequest;
                var isIdempotentReq = message.Method.IsIdempotent();

                return options.UseReplayPolicy switch
                {
                    true when !onlyIdempotent => retryPolicy,
                    true when onlyIdempotent && isIdempotentReq => retryPolicy,
                    true when onlyIdempotent && !isIdempotentReq => noOpPolicy,
                    _ => noOpPolicy
                };
            });
    }

    /// <summary>
    ///     Добавляет обработчики Тайм-аут политик по умолчанию, зарегистрированных в <see cref="IPolicyRegistry{T}" />.
    /// </summary>
    /// <remarks>
    ///     Для регистрации обработчиков, требуется реестр <see cref="IPolicyRegistry{T}" />, содержащий политики с
    ///     именами <see cref="DefaultTimeoutStrategy" />, <see cref="NoOpStrategy" />.
    ///     Для добавления такого реестра в контейнер внедрения зависимостей,
    ///     вызовите <see cref="AddDefaultPolicyRegistry{T}" /> или зарегистрируйте его самостоятельно.
    /// </remarks>
    /// <param name="clientBuilder">Ссылка на объект <see cref="IHttpClientBuilder" />.</param>
    /// <typeparam name="TOption">Тип параметров унаследованный от <see cref="ClientOptionsBase" />.</typeparam>
    /// <returns>Ссылка на объект <see cref="IHttpClientBuilder" />.</returns>
    public static IHttpClientBuilder AddDefaultTimeoutPolicyHandlers<TOption>(this IHttpClientBuilder clientBuilder)
        where TOption : ClientOptionsBase
    {
        return clientBuilder
            .AddPolicyHandler((provider, _) =>
            {
                var registry = provider.GetRequiredService<IReadOnlyPolicyRegistry<string>>();
                var options = provider.GetRequiredService<IOptions<TOption>>().Value;
                var timeoutPolicy = registry.Get<IAsyncPolicy<HttpResponseMessage>>(DefaultTimeoutStrategy);
                var noOpPolicy = registry.Get<IAsyncPolicy<HttpResponseMessage>>(NoOpStrategy);

                return options.UseTimeoutPolicy ? timeoutPolicy : noOpPolicy;
            });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount, int retryDelay)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(retryDelay, retryAttempt)));
    }

    private static bool IsIdempotent(this HttpMethod method)
    {
        return method == HttpMethod.Get ||
               method == HttpMethod.Head ||
               method == HttpMethod.Put ||
               method == HttpMethod.Delete;
    }
}