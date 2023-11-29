using System;
using System.Text;
using IBM.XMS;
using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests.DSL
{
  internal static class Given
  {
    public static MqSeriesConnectionFactoryProviderBuilder ConnectionFactoryProvider =>
      new();

    public static MqSeriesQueueSubscribingMessageAdapterBuilder SubscribingMessageAdapter =>
      new();

    public static MqSeriesQueueTransactedMessageAdapterBuilder TransactedMessageAdapter =>
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

    public static Func<long, IMessage> MessageProducer(string messageId, string correlationId)
    {
      return timeout =>
      {
        var msg = Mock.Of<IMessage>(x => x.PropertyNames == new object[] { }.GetEnumerator());

        msg.JMSMessageID = messageId;
        msg.JMSCorrelationID = correlationId;
        msg.JMSCorrelationIDAsBytes = Encoding.ASCII.GetBytes(correlationId);
        msg.JMSTimestamp = timeout;

        return msg;
      };
    }

    public static Action<IMessage> MessageSender(string messageId, string correlationId)
    {
      return msg =>
      {
        msg.JMSMessageID = messageId;
        msg.JMSCorrelationID = correlationId;
        msg.JMSCorrelationIDAsBytes = Encoding.ASCII.GetBytes(correlationId);
      };
    }
  }
}