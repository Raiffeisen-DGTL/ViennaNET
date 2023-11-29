using System;
using Apache.NMS;
using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
  internal static class Given
  {
    public static ActiveMqConnectionFactoryBuilder ConnectionFactoryProvider => new();

    public static ActiveMqQueueMessageAdapterBuilder MessageAdapter => new();

    public static ActiveMqQueueSubscribingMessageAdapterBuilder SubscribingMessageAdapter =>
      new();

    public static ActiveMqQueueTransactedMessageAdapterBuilder TransactedMessageAdapter =>
      new();

    public static BytesMessageBuilder BytesMessage => new();

    public static TextMessageBuilder TextMessage => new();

    public static SessionBuilder Session => new();

    public static ILoggerFactory FakeLoggerFactory
    {
      get
      {
        var factory = new Mock<ILoggerFactory> { DefaultValue = DefaultValue.Mock };
        return factory.Object;
      }
    }

    public static IActiveMqConnectionFactory FakeConnectionFactory
    {
      get
      {
        var factory = new Mock<IActiveMqConnectionFactory> { DefaultValue = DefaultValue.Mock };
        return factory.Object;
      }
    }

    public static Func<TimeSpan, IMessage> MessageProducer(string messageId, string correlationId)
    {
      return timeout =>
      {
        var msg = Mock.Of<IMessage>();

        msg.NMSMessageId = messageId;
        msg.NMSCorrelationID = correlationId;

        return msg;
      };
    }

    public static Action<IMessage> MessageSender(string messageId, string correlationId)
    {
      return msg =>
      {
        msg.NMSMessageId = messageId;
        msg.NMSCorrelationID = correlationId;
      };
    }
  }
}