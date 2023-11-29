using Apache.NMS;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.ActiveMQQueue
{
  /// <summary>
  ///   Адаптер очереди ActiveMQ
  /// </summary>
  public class ActiveMqQueueTransactedMessageAdapter : ActiveMqQueueMessageAdapter, IMessageAdapterWithTransactions
  {
    private readonly ActiveMqQueueConfiguration _queueConfiguration;

    /// <summary>
    ///   Адаптер с поддержкой транзакций, реализующий взаимодействие с очередью ActiveMQ
    /// </summary>
    /// <param name="connectionFactory">Фабрика подключений</param>
    /// <param name="queueConfiguration">Параметры очереди</param>
    /// <param name="logger">Сервис логирования</param>
    public ActiveMqQueueTransactedMessageAdapter(IActiveMqConnectionFactory connectionFactory,
      ActiveMqQueueConfiguration queueConfiguration,
      ILogger logger)
      : base(connectionFactory, queueConfiguration, logger)
    {
      _queueConfiguration = queueConfiguration;
    }

    /// <inheritdoc />
    public void CommitIfTransacted(BaseMessage message)
    {
      try
      {
        if (_queueConfiguration.TransactionEnabled)
        {
          Session.Commit();
        }
      }
      catch (NMSException ex)
      {
        throw new MessagingException(
          ex,
          $"Error committing message for queue {_queueConfiguration.Id}. See inner exception");
      }
    }

    /// <inheritdoc />
    public void RollbackIfTransacted()
    {
      try
      {
        if (_queueConfiguration.TransactionEnabled)
        {
          Session.Rollback();
        }
      }
      catch (NMSException ex)
      {
        throw new MessagingException(
          ex,
          $"Error rolling back message for queue {_queueConfiguration.Id}. See inner exception");
      }
    }

    /// <inheritdoc />
    protected override ISession CreateSession(IConnection connection)
    {
      return connection.CreateSession(AcknowledgementMode.Transactional);
    }
  }
}