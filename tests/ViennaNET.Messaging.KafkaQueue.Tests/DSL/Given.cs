using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.KafkaQueue.Tests.DSL
{
  internal static class Given
  {
    public static KafkaQueueMessageAdapterBuilder KafkaQueueMessageAdapter => new();

    public static KafkaConnectionFactoryMock KafkaConnectionFactory => new();

    public static ILoggerFactory FakeLoggerFactory
    {
      get
      {
        var factory = new Mock<ILoggerFactory> { DefaultValue = DefaultValue.Mock };
        return factory.Object;
      }
    }

    public static ILogger<T> GetLogger<T>()
    {
      return Mock.Of<ILogger<T>>();
    }
  }
}