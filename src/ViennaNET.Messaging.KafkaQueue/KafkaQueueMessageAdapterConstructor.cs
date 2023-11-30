using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Factories.Impl;

namespace ViennaNET.Messaging.KafkaQueue
{
  /// <inheritdoc />
  public class
    KafkaQueueMessageAdapterConstructor
    : QueueMessageAdapterConstructorBase<KafkaConfiguration, KafkaQueueConfiguration>
  {
    private readonly IKafkaConnectionFactory _connectionFactory = new KafkaConnectionFactory();
    private readonly ILoggerFactory _loggerFactory;

    /// <inheritdoc />
    public KafkaQueueMessageAdapterConstructor(IConfiguration configuration, ILoggerFactory loggerFactory)
      : base(configuration, "kafka")
    {
      _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    protected override IMessageAdapter CreateAdapter(KafkaQueueConfiguration queueConfiguration)
    {
      if (queueConfiguration.TransactionEnabled)
      {
        return new KafkaQueueTransactedMessageAdapter(
          queueConfiguration,
          _connectionFactory,
          _loggerFactory.CreateLogger<KafkaQueueTransactedMessageAdapter>());
      }

      return new KafkaQueueMessageAdapter(
        queueConfiguration,
        _connectionFactory,
        _loggerFactory.CreateLogger<KafkaQueueMessageAdapter>());
    }
  }
}