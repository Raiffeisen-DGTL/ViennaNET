using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Diagnostic;
using ViennaNET.Logging;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Processing.Impl.Poll
{
  /// <inheritdoc />
  public class QueueTransactedPollingReactor : QueuePollingReactorBase
  {
    private readonly IMessageAdapterWithTransactions _transactedAdapter;

    /// <summary>
    ///   Инициализирует компонент ссылками на <see cref="IMessageAdapter" />, <see cref="IHealthCheckingService" />,
    ///   коллекции <see cref="IMessageProcessor" />, <see cref="IMessageProcessorAsync" />
    /// </summary>
    /// <param name="messageAdapter">Адаптер для работы с очередью</param>
    /// <param name="messageProcessors">Процессоры для обработки сообщения</param>
    /// <param name="asyncMessageProcessors">Асинхронные процессоры для обработки сообщения</param>
    /// <param name="subscribeInterval">Интервал подписки</param>
    /// <param name="pollingId">Идентификатор потока для опроса</param>
    /// <param name="serviceHealthDependent">Признак использования диагностики</param>
    /// <param name="healthCheckingService">Ссылка на службу диагностики</param>
    /// <param name="messagingCallContextAccessor">Ссылка на объект для доступа к контексту вызова</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public QueueTransactedPollingReactor(
      IMessageAdapterWithTransactions messageAdapter, IEnumerable<IMessageProcessor> messageProcessors,
      IEnumerable<IMessageProcessorAsync> asyncMessageProcessors, int subscribeInterval, string pollingId, bool? serviceHealthDependent,
      IHealthCheckingService healthCheckingService, IMessagingCallContextAccessor messagingCallContextAccessor) : base(messageAdapter,
                                                                                                              messageProcessors,
                                                                                                              asyncMessageProcessors,
                                                                                                              subscribeInterval,
                                                                                                              pollingId,
                                                                                                              serviceHealthDependent,
                                                                                                              healthCheckingService,
                                                                                                              messagingCallContextAccessor)
    {
      _transactedAdapter = messageAdapter.ThrowIfNull(nameof(messageAdapter));
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
          Logger.LogError($"The message did not processed. Message body: {message.LogBody()}");
        }

        _transactedAdapter.CommitIfTransacted(message);
      }
      catch (SystemException)
      {
        Logger.LogError("Process message failed: commit message");
        _transactedAdapter.CommitIfTransacted(message);
      }
      catch (Exception e)
      {
        Logger.LogError(e, "Error while executing process handler");
        _transactedAdapter.RollbackIfTransacted();
      }
    }
  }
}
