using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Processing.Impl.Poll
{
  /// <inheritdoc />
  public class QueuePollingReactor : QueuePollingReactorBase
  {
    private readonly ILogger _logger;

    /// <summary>
    ///   Инициализирует компонент ссылками на <see cref="IMessageAdapter" />, <see cref="IHealthCheckingService" />,
    ///   коллекции <see cref="IMessageProcessor" />, <see cref="IMessageProcessorAsync" />
    /// </summary>
    /// <param name="messageAdapter">Адаптер для работы с очередью</param>
    /// <param name="messageProcessors">Процессоры для обработки сообщения</param>
    /// <param name="asyncMessageProcessors">Асинхронные процессоры для обработки сообщения</param>
    /// <param name="subscribeInterval">Интервал подписки</param>
    /// <param name="serviceHealthDependent">Признак использования диагностики</param>
    /// <param name="healthCheckingService">Ссылка на службу диагностики</param>
    /// <param name="messagingCallContextAccessor">Ссылка на объект для доступа к контексту вызова</param>
    /// <param name="logger">Интерфейс логгирования</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public QueuePollingReactor(
      IMessageAdapter messageAdapter,
      IEnumerable<IMessageProcessor> messageProcessors,
      IEnumerable<IMessageProcessorAsync> asyncMessageProcessors,
      int subscribeInterval,
      bool? serviceHealthDependent,
      IHealthCheckingService healthCheckingService,
      IMessagingCallContextAccessor messagingCallContextAccessor,
      ILogger<QueuePollingReactor> logger) : base(messageAdapter,
      messageProcessors,
      asyncMessageProcessors,
      subscribeInterval,
      serviceHealthDependent,
      healthCheckingService,
      messagingCallContextAccessor,
      logger)
    {
      _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ProcessReceivedMessage(BaseMessage message)
    {
      try
      {
        var processed = await GetProcessedMessageAsync(message);
        if (!processed)
        {
          processed = GetProcessedMessage(message);
        }

        if (!processed)
        {
          _logger.LogError("The message did not processed. Message body: {messageBody}", message.LogBody());
        }
      }
      catch (SystemException)
      {
        _logger.LogError("Process message failed: commit message");
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Error while executing process handler");
      }
    }
  }
}