using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ViennaNET.Extensions.Http.Logging;

/// <summary>
///     Обработчик осущсетвляющий запись тела сообщения в систему ведения журнала.
/// </summary>
/// <remarks>
///     Запись тела сообщения включена только на уровне <see cref="Trace" />.
/// </remarks>
public class LoggingHttpMessageBodyHandler : DelegatingHandler
{
    private const int MaxContentLength = 100_000;
    private readonly ILogger<LoggingHttpMessageBodyHandler> _logger;

    /// <summary>
    ///     Создаёт новый экземпляр <see cref="LoggingHttpMessageBodyHandler" />.
    /// </summary>
    /// <param name="logger">Ссылка на объект <see cref="ILogger{T}" />.</param>
    public LoggingHttpMessageBodyHandler(ILogger<LoggingHttpMessageBodyHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Создаёт новый экземпляр класса <see cref="LoggingHttpMessageBodyHandler" />
    ///     с указанным внутренним обработчиком.
    /// </summary>
    /// <param name="innerHandler">Ссылка на объект <see cref="HttpMessageHandler" />.</param>
    /// <param name="logger">Ссылка на объект <see cref="ILogger{T}" />.</param>
    public LoggingHttpMessageBodyHandler(HttpMessageHandler innerHandler,
        ILogger<LoggingHttpMessageBodyHandler> logger) : base(innerHandler)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await Log.RequestPayload(_logger, request);
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await Log.ResponsePayload(_logger, response);
        return response;
    }

    private static class Log
    {
        private const string RequestMessage = "Полезная нагрузка запроса:\n {Payload}";
        private const string ResponseMessage = "Полезная нагрузка ответа:\n {Payload}";

        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, string, Exception?> _requestPayload =
            LoggerMessage.Define<string>(LogLevel.Trace, EventIds.RequestPayloadId, RequestMessage);

        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, string, Exception?> _responsePayload =
            LoggerMessage.Define<string>(LogLevel.Trace, EventIds.ResponsePayloadId, ResponseMessage);

        public static async Task RequestPayload(ILogger logger, HttpRequestMessage request)
        {
            if (request.Content is { } content)
            {
                var payload = await content.ReadAsStringAsync().ConfigureAwait(false);

                if (payload.Length is > 0 and < MaxContentLength)
                {
                    _requestPayload(logger, payload, null);
                }
            }
        }

        public static async Task ResponsePayload(ILogger logger, HttpResponseMessage response)
        {
            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (payload.Length is > 0 and < MaxContentLength)
            {
                _responsePayload(logger, payload, null);
            }
        }

        private static class EventIds
        {
            public static readonly EventId RequestPayloadId = new(104, nameof(RequestPayload));
            public static readonly EventId ResponsePayloadId = new(105, nameof(ResponsePayload));
        }
    }
}