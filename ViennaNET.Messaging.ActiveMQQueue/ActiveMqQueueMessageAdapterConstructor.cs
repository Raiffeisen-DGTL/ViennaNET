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
    private readonly ILoggerFactory _loggerFactory;
    private readonly IActiveMqConnectionFactory _connectionFactory;

    /// <inheritdoc />
    public ActiveMqQueueMessageAdapterConstructor(IConfiguration configuration,
      ILoggerFactory loggerFactory,
      IActiveMqConnectionFactory connectionFactory)
      : base(configuration, "activemq")
    {
      _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
      _connectionFactory = connectionFactory.ThrowIfNull(nameof(connectionFactory));
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
      else if (queueConfiguration.ProcessingType == MessageProcessingType.Subscribe ||
               queueConfiguration.ProcessingType == MessageProcessingType.SubscribeAndReply)
      {
        return new ActiveMqQueueSubscribingMessageAdapter(
          _connectionFactory,
          queueConfiguration,
          _loggerFactory.CreateLogger<ActiveMqQueueSubscribingMessageAdapter>());
      }
      else
      {
        return new ActiveMqQueueMessageAdapter(
          _connectionFactory,
          queueConfiguration,
          _loggerFactory.CreateLogger<ActiveMqQueueMessageAdapter>());

      }
    }

    /// <inheritdoc />
    protected override void CheckConfigurationParameters(ActiveMqQueueConfiguration configuration)
    {
      configuration.Id.ThrowIfNullOrWhiteSpace(nameof(configuration.Id));
      configuration.Server.ThrowIfNullOrWhiteSpace(nameof(configuration.Server));
      configuration.QueueName.ThrowIfNullOrWhiteSpace(nameof(configuration.QueueName));
      if (configuration.UseQueueString)
      {
        configuration.QueueString.ThrowIfNullOrWhiteSpace(nameof(configuration.QueueString));
      }
    }
  }
}
