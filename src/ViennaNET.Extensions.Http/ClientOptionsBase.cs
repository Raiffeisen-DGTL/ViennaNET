using System.Diagnostics.CodeAnalysis;
using Polly;
using Polly.Retry;
using ViennaNET.Extensions.Http.Authentication;

namespace ViennaNET.Extensions.Http;

/// <summary>
///     Базовый класс, для параметров типизированных HTTP клиентов.
/// </summary>
public abstract class ClientOptionsBase
{
    /// <summary>
    ///     Префикс для конфигурации конечных точек по умолчанию.
    /// </summary>
    public const string DefaultSectionRootName = "Endpoints:";

    /// <summary>
    ///     Базовый адрес.
    /// </summary>
#pragma warning disable CS8618
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Используется в JsonSerializer")]
    public string BaseAddress { get; set; }
#pragma warning restore CS8618

    /// <summary>
    ///     Если <see langword="true" />, <see cref="HttpClientHandler" /> будет использовать политику повтора:
    ///     <see cref="AsyncRetryPolicy{TResult}" />, который будет повторять запрос <see cref="RetryCount" /> раз,
    ///     если первая попытка потерпела неудачу.
    ///     <b>По умолчанию <see langword="false" /></b>.
    /// </summary>
    /// <remarks>
    ///     При каждой повторной попытке продолжительность ожидания равна
    ///     <c>TimeSpan.FromSeconds(Math.Pow(retryDelay, retryAttempt))</c>,
    ///     где retryAttempt - это номер попытки, а <c>retryDelay</c> = <see cref="RetryDelay" />.
    /// </remarks>
    public bool UseReplayPolicy { get; set; } = false;


    /// <summary>
    ///     Если <see langword="true" /> и <see cref="UseReplayPolicy" /> = <see langword="true" />,
    ///     тогда <see cref="HttpClientHandler" /> будет использовать политику повтора:
    ///     <see cref="AsyncRetryPolicy{TResult}" />, только для идемпотентных запросов (GET, HEAD, PUT, DELETE).
    ///     <b>По умолчанию <see langword="true" /></b>.
    /// </summary>
    public bool UseReplayPolicyOnlyIdempotentRequest { get; set; } = true;

    /// <summary>
    ///     Если <see langword="true" />, <see cref="HttpClientHandler" /> будет использовать политику
    ///     <see cref="AsyncPolicy{TResult}" /> которая будет асинхронно ожидать завершения делегата в течение
    ///     заданного периода времени <see cref="TryTimeout" />.
    ///     <b>По умолчанию <see langword="false" /></b>.
    /// </summary>
    /// <remarks>
    ///     Будет вызвано исключение TimeoutRejectedException, если делегат не завершится в течение
    ///     настроенного времени ожидания.
    /// </remarks>
    public bool UseTimeoutPolicy { get; set; } = false;

    /// <summary>
    ///     Общее время ожидания для всех попыток. В секундах.
    ///     <b>По умолчанию 2 минуты (120 сек)</b>.
    /// </summary>
    /// <remarks>
    ///     Значение используется для инициализации <see cref="HttpClient.Timeout" />.
    /// </remarks>
    public int OverallTimeout { get; set; } = 60 * 2;

    /// <summary>
    ///     Время ожидания для каждой отдельной попыти. В секундах.
    ///     <b>По умолчанию 10 сек</b>.
    /// </summary>
    /// <remarks>
    ///     Общее время выполнения запроса ограничено значением <see cref="OverallTimeout" />.
    ///     Независимо от количества повторных попыток.
    /// </remarks>
    public int TryTimeout { get; set; } = 10;

    /// <summary>
    ///     Время задержки первой повторной попытки в секундах.
    ///     <b>По умолчанию 2 сек</b>.
    /// </summary>
    /// <remarks>
    ///     Последующие попытки будут выполняться с экспоненциальной задержкой.
    /// </remarks>
    public int RetryDelay { get; set; } = 2;

    /// <summary>
    ///     Максимальное количество повторных попыток.
    ///     <b>По умолчанию 6 попыток</b>.
    /// </summary>
    public int RetryCount { get; set; } = 6;
    
    /// <summary>
    /// 
    /// </summary>
    public AuthenticationOptions? Authentication { get; set; }
}