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
    private readonly IMqSeriesQueueConnectionFactoryProvider _connectionFactory =
      new MqSeriesQueueConnectionFactoryProvider();

    private readonly ILoggerFactory _loggerFactory;

    /// <inheritdoc />
    public MqSeriesQueueMessageAdapterConstructor(IConfiguration configuration, ILoggerFactory loggerFactory) : base(
      configuration, "mqseries")
    {
      _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    protected override IMessageAdapter CreateAdapter(MqSeriesQueueConfiguration queueConfiguration)
    {
      queueConfiguration.ThrowIfNull(nameof(queueConfiguration));

      if (queueConfiguration.TransactionEnabled)
      {
        return new MqSeriesQueueTransactedMessageAdapter(_connectionFactory,
          queueConfiguration,
          _loggerFactory.CreateLogger<MqSeriesQueueTransactedMessageAdapter>());
      }

      if (queueConfiguration.ProcessingType == MessageProcessingType.Subscribe ||
          queueConfiguration.ProcessingType == MessageProcessingType.SubscribeAndReply)
      {
        return new MqSeriesQueueSubscribingMessageAdapter(
          _connectionFactory,
          queueConfiguration,
          _loggerFactory.CreateLogger<MqSeriesQueueSubscribingMessageAdapter>());
      }

      return new MqSeriesQueueMessageAdapter(
        _connectionFactory,
        queueConfiguration,
        _loggerFactory.CreateLogger<MqSeriesQueueMessageAdapter>());
    }
  }
}