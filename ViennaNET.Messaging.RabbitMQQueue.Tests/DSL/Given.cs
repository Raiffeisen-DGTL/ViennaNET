using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests.DSL
{
  internal static class Given
  {
    public static RabbitMqQueueMessageAdapterBuilder RabbitMqQueueMessageAdapter =>
      new RabbitMqQueueMessageAdapterBuilder();

    public static AdvancedBusFactoryMock AdvancedBusFactory => new AdvancedBusFactoryMock();

    public static ILoggerFactory FakeLoggerFactory
    {
      get
      {
        var factory = new Mock<ILoggerFactory> {DefaultValue = DefaultValue.Mock};
        return factory.Object;
      }
    }
  }
}