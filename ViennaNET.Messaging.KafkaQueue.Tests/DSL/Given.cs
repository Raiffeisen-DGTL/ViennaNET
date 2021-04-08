using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.KafkaQueue.Tests.DSL
{
  internal static class Given
  {
    public static KafkaQueueMessageAdapterBuilder KafkaQueueMessageAdapter => new KafkaQueueMessageAdapterBuilder();

    public static KafkaConnectionFactoryMock KafkaConnectionFactory => new KafkaConnectionFactoryMock();

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