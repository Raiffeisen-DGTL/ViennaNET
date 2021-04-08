using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests.DSL
{
  internal class RabbitMqQueueMessageAdapterBuilder
  {
    private IAdvancedBusFactory _busFactory;
    private RabbitMqQueueConfiguration _configuration;

    public RabbitMqQueueMessageAdapterBuilder WithConfiguration(RabbitMqQueueConfiguration configuration)
    {
      _configuration = configuration;
      return this;
    }

    public RabbitMqQueueMessageAdapterBuilder WithBusFactory(IAdvancedBusFactory factory)
    {
      _busFactory = factory;
      return this;
    }

    public RabbitMqQueueMessageAdapter Please()
    {
      return new RabbitMqQueueMessageAdapter(
        _busFactory ?? Given.AdvancedBusFactory,
        _configuration ?? new RabbitMqQueueConfiguration(),
        Mock.Of<ILogger<RabbitMqQueueMessageAdapter>>());
    }
  }
}