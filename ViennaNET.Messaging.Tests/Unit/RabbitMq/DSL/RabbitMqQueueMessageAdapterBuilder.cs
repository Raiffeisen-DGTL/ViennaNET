using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Messaging.RabbitMQQueue;

namespace ViennaNET.Messaging.Tests.Unit.RabbitMq.DSL
{
  internal class RabbitMqQueueMessageAdapterBuilder
  {
    private RabbitMqQueueConfiguration _configuration;
    private IAdvancedBusFactory _busFactory;

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
