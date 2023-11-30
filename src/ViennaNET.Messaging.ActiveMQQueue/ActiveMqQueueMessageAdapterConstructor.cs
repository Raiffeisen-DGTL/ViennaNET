using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.ActiveMQQueue
{
  /// <inheritdoc />
  public class ActiveMqQueueMessageAdapterConstructor : QueueMessageAdapterConstructorBase<ActiveMqConfiguration,
    ActiveMqQueueConfiguration>
  {
    private readonly IActiveMqConnectionFactory _connectionFactory;
    private readonly ILoggerFactory _loggerFactory;

    /// <inheritdoc />
    public ActiveMqQueueMessageAdapterConstructor(IConfiguration configuration,
      ILoggerFactory loggerFactory,
      IActiveMqConnectionFactory connectionFactory)
      : base(configuration, "activemq")
    {
      _loggerFactory = loggerFactory;
      _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    protected override IMessageAdapter CreateAdapter(ActiveMqQueueConfiguration queueConfiguration)
    {
      queueConfiguration.ThrowIfNull(nameof(queueConfiguration));

      if (queueConfiguration.TransactionEnabled)
      {
        return new ActiveMqQueueTransactedMessageAdapter(_connectionFactory,
          queueConfiguration,
          _loggerFactory.CreateLogger<ActiveMqQueueTransactedMessageAdapter>());
      }

      if (queueConfiguration.ProcessingType == MessageProcessingType.Subscribe ||
          queueConfiguration.ProcessingType == MessageProcessingType.SubscribeAndReply)
      {
        return new ActiveMqQueueSubscribingMessageAdapter(
          _connectionFactory,
          queueConfiguration,
          _loggerFactory.CreateLogger<ActiveMqQueueSubscribingMessageAdapter>());
      }

      return new ActiveMqQueueMessageAdapter(
        _connectionFactory,
        queueConfiguration,
        _loggerFactory.CreateLogger<ActiveMqQueueMessageAdapter>());
    }
  }
}