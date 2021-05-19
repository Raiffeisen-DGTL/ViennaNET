using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.KafkaQueue.Tests.DSL
{
  internal class KafkaQueueMessageAdapterBuilder
  {
    private KafkaQueueConfiguration _configuration;
    private IKafkaConnectionFactory _connectionFactory;

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