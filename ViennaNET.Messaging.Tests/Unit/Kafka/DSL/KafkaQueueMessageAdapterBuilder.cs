using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Messaging.KafkaQueue;

namespace ViennaNET.Messaging.Tests.Unit.Kafka.DSL
{
  internal class KafkaQueueMessageAdapterBuilder
  {
    private IKafkaConnectionFactory _connectionFactory;
    private KafkaQueueConfiguration _configuration;

    public KafkaQueueMessageAdapterBuilder WithConfiguration(KafkaQueueConfiguration config)
    {
      _configuration = config;
      return this;
    }

    public KafkaQueueMessageAdapterBuilder WithConnectionFactory(IKafkaConnectionFactory connectionFactory)
    {
      _connectionFactory = connectionFactory;
      return this;
    }

    public KafkaQueueMessageAdapter Please()
    {
      return new KafkaQueueMessageAdapter(
        _configuration ?? new KafkaQueueConfiguration(),
        _connectionFactory ?? new KafkaConnectionFactoryMock(),
        Mock.Of<ILogger<KafkaQueueMessageAdapter>>());
    }
  }
}
