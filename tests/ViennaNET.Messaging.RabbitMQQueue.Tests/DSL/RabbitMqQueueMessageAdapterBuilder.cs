using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests.DSL
{
  internal class RabbitMqQueueMessageAdapterBuilder
  {
    private RabbitMqQueueConfiguration _configuration;
    private IRabbitMqConnectionFactory _connectionFactory;

    public RabbitMqQueueMessageAdapterBuilder WithConfiguration(RabbitMqQueueConfiguration configuration)
    {
      _configuration = configuration;
      return this;
    }

    public RabbitMqQueueMessageAdapterBuilder WithConnectionFactory(IRabbitMqConnectionFactory factory)
    {
      _connectionFactory = factory;
      return this;
    }

    public RabbitMqQueueMessageAdapter Please()
    {
      return new(
        _configuration ?? new RabbitMqQueueConfiguration(),
        _connectionFactory ?? Given.ConnectionFactoryMock,
        Mock.Of<ILogger<RabbitMqQueueMessageAdapter>>());
    }
  }
}