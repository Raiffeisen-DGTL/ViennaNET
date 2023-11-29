using IBM.XMS;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  /// <summary>
  ///   Адаптер с поддержкой транзакций, реализующий взаимодействие с очередью IBM MQ
  /// </summary>
  /// <remarks>Работает только в режиме поллинга</remarks>
  internal sealed class MqSeriesQueueTransactedMessageAdapter : MqSeriesQueueMessageAdapter,
    IMessageAdapterWithTransactions
  {
    /// <inheritdoc />
    public MqSeriesQueueTransactedMessageAdapter(
      IMqSeriesQueueConnectionFactoryProvider connectionFactoryProvider,
      MqSeriesQueueConfiguration configuration,
      ILogger<MqSeriesQueueTransactedMessageAdapter> logger)
      : base(connectionFactoryProvider, configuration, logger)
    {
    }

    /// <inheritdoc />
    public void CommitIfTransacted(BaseMessage message)
    {
      try
      {
        if (_configuration.TransactionEnabled)
        {
          GetSession().Commit();
        }
      }
      catch (XMSException ex)
      {
        throw new MessagingException(
          ex,
          $"Error commiting message in queue {_configuration.Id}. See inner exception");
      }
    }

    /// <inheritdoc />
    public void RollbackIfTransacted()
    {
      try
      {
        if (_configuration.TransactionEnabled)
        {
          GetSession().Rollback();
        }
      }
      catch (XMSException ex)
      {
        throw new MessagingException(
          ex,
          $"Error rolling back message in queue {_configuration.Id}. See inner exception");
      }
    }

    /// <inheritdoc />
    protected override ISession CreateSession()
    {
      var connection = GetConnection();
      return _configuration.TransactionEnabled
        ? connection.CreateSession(true, AcknowledgeMode.SessionTransacted)
        : connection.CreateSession(false, AcknowledgeMode.AutoAcknowledge);
    }
  }
}