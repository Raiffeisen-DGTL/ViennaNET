using System;
using Apache.NMS;
using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
  internal static class Given
  {
    public static ActiveMqConnectionFactoryBuilder ConnectionFactoryProvider => new ActiveMqConnectionFactoryBuilder();

    public static ActiveMqQueueMessageAdapterBuilder MessageAdapter => new ActiveMqQueueMessageAdapterBuilder();

    public static ActiveMqQueueSubscribingMessageAdapterBuilder SubscribingMessageAdapter =>
      new ActiveMqQueueSubscribingMessageAdapterBuilder();

    public static ActiveMqQueueTransactedMessageAdapterBuilder TransactedMessageAdapter =>
      new ActiveMqQueueTransactedMessageAdapterBuilder();

    public static BytesMessageBuilder BytesMessage => new BytesMessageBuilder();

    public static TextMessageBuilder TextMessage => new TextMessageBuilder();

    public static SessionBuilder Session => new SessionBuilder();

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