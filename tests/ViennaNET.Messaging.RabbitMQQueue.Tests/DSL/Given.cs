using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests.DSL
{
  internal static class Given
  {
    public static RabbitMqQueueMessageAdapterBuilder RabbitMqQueueMessageAdapter =>
      new();

    public static ConnectionFactoryMock ConnectionFactoryMock => new();

    public static IBasicProperties BasicProperties
    {
      get
      {
        var retVal = new Mock<IBasicProperties>();
        retVal.SetupAllProperties();
        return retVal.Object;
      }
    }

    public static ILoggerFactory FakeLoggerFactory
    {
      get
      {
        var factory = new Mock<ILoggerFactory> { DefaultValue = DefaultValue.Mock };
        return factory.Object;
      }
    }
  }
}