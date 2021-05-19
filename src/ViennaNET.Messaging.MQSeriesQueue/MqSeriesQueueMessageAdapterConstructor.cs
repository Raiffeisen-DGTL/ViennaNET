using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  /// <inheritdoc />
  public class MqSeriesQueueMessageAdapterConstructor : QueueMessageAdapterConstructorBase<MqSeriesConfiguration,
    MqSeriesQueueConfiguration>
  {
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMqSeriesQueueConnectionFactoryProvider _connectionFactory = new MqSeriesQueueConnectionFactoryProvider();

    /// <inheritdoc />
    public MqSeriesQueueMessageAdapterConstructor(IConfiguration configuration, ILoggerFactory loggerFactory) : base(configuration, "mqseries")
    {
      _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    protected override IMessageAdapter CreateAdapter(MqSeriesQueueConfiguration queueConfiguration)
    {
      queueConfiguration.ThrowIfNull(nameof(queueConfiguration));

      return queueConfiguration.TransactionEnabled ||
             queueConfiguration.ProcessingType == MessageProcessingType.ThreadStrategy
        ? new MqSeriesQueueTransactedMessageAdapter(
          _connectionFactory,
          queueConfiguration,
          _loggerFactory.CreateLogger<MqSeriesQueueTransactedMessageAdapter>())
        : new MqSeriesQueueSubscribingMessageAdapter(
          _connectionFactory,
          queueConfiguration,
          _loggerFactory.CreateLogger<MqSeriesQueueSubscribingMessageAdapter>());
    }

    /// <inheritdoc />
    protected override void CheckConfigurationParameters(MqSeriesQueueConfiguration configuration)
    {
      configuration.Id.ThrowIfNullOrWhiteSpace(nameof(configuration.Id));
      configuration.ClientId.ThrowIfNullOrWhiteSpace(nameof(configuration.ClientId));
      configuration.Server.ThrowIfNullOrWhiteSpace(nameof(configuration.Server));
      configuration.QueueName.ThrowIfNullOrWhiteSpace(nameof(configuration.QueueName));
      configuration.QueueManager.ThrowIfNullOrWhiteSpace(nameof(configuration.QueueManager));
      if (configuration.UseQueueString)
      {
        configuration.QueueString.ThrowIfNullOrWhiteSpace(nameof(configuration.QueueString));
      }
    }
  }
}