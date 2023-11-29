using System;
using System.Globalization;
using NUnit.Framework;
using RabbitMQ.Client.Events;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.RabbitMQQueue.Tests.DSL;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(RabbitMqQueueMessageConverter))]
  public class RabbitMqQueueMessageConverterTests
  {
    public static readonly object[][] MessagesWithContentType =
    {
      new object[] { new BytesMessage(), RabbitMqQueueMessageConverter.ContentType.Bytes.ToString("G") },
      new object[] { new TextMessage(), RabbitMqQueueMessageConverter.ContentType.Text.ToString("G") }
    };

    [TestCaseSource(nameof(MessagesWithContentType))]
    public void ConvertToProperties_ShouldReturnCorrect(BaseMessage message, string _)
    {
      message.Properties.Add("key1", "value1");
      message.LifeTime = TimeSpan.FromMinutes(10);
      message.ReplyQueue = "replyQueue";
      message.CorrelationId = "correlId";

      var properties = message.ConvertToProperties(Given.BasicProperties);

      Assert.Multiple(() =>
      {
        Assert.IsNotEmpty(properties.MessageId);
        Assert.AreEqual(message.Properties, properties.Headers);
        Assert.AreEqual(message.LifeTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture),
          properties.Expiration);
        Assert.Greater(properties.Timestamp.UnixTime, 0);
        Assert.IsNotEmpty(properties.ContentType);
        Assert.AreEqual(message.ReplyQueue, properties.ReplyTo);
        Assert.AreEqual(message.CorrelationId, properties.CorrelationId);
      });
    }

    [TestCaseSource(nameof(MessagesWithContentType))]
    public void ConvertToBaseMessage_BasicDelivery_ShouldReturnCorrect(BaseMessage messageForType, string contentType)
    {
      var properties = Given.BasicProperties;
      properties.ContentType = contentType;
      properties.MessageId = "messageId";
      properties.CorrelationId = "correlId";
      properties.ReplyTo = "replyQueue";
      var args = new BasicDeliverEventArgs(
        null,
        0,
        false,
        null,
        null,
        properties,
        new ReadOnlyMemory<byte>());

      var message = args.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf(messageForType.GetType(), message);
        Assert.AreEqual(properties.MessageId, message.MessageId);
        Assert.AreEqual(properties.CorrelationId, message.CorrelationId);
        Assert.AreEqual(properties.ReplyTo, message.ReplyQueue);
      });
    }
  }
}