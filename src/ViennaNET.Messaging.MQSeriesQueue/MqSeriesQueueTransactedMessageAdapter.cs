using IBM.XMS;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  /// <summary>
  ///   Адаптер с поддержкой транзакций, реализующий взаимодействие с очередью IBM MQ
  /// </summary>
  /// <remarks>Работает только в режиме поллинга</remarks>
  internal sealed class MqSeriesQueueTransactedMessageAdapter : MqSeriesQueueMessageAdapterBase,
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
        throw new MessagingException(ex, "Error while commit message. See inner exception for more details");
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
        throw new MessagingException(ex, "Error while rollback message. See inner exception for more details");
      }
    }

    /// <inheritdoc />
    public override bool SupportProcessingType(MessageProcessingType processingType)
    {
      return processingType == MessageProcessingType.ThreadStrategy;
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
