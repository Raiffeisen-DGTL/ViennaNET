using System.Globalization;
using NUnit.Framework;
using RabbitMQ.Client.Events;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.RabbitMQQueue.Tests.DSL;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests;

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
            Assert.That(properties.MessageId, Is.Not.Empty);
            Assert.That(message.Properties, Is.EqualTo(properties.Headers));
            Assert.That(message.LifeTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture),
                Is.EqualTo(properties.Expiration));
            Assert.That(properties.Timestamp.UnixTime, Is.GreaterThan(0));
            Assert.That(properties.ContentType, Is.Not.Empty);
            Assert.That(message.ReplyQueue, Is.EqualTo(properties.ReplyTo));
            Assert.That(message.CorrelationId, Is.EqualTo(properties.CorrelationId));
        });
    }

    [TestCaseSource(nameof(MessagesWithContentType))]
    public void ConvertToBaseMessage_BasicDelivery_ShouldReturnCorrect(BaseMessage messageForType,
        string contentType)
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
            Assert.That(message, Is.InstanceOf(messageForType.GetType()));
            Assert.That(properties.MessageId, Is.EqualTo(message.MessageId));
            Assert.That(properties.CorrelationId, Is.EqualTo(message.CorrelationId));
            Assert.That(properties.ReplyTo, Is.EqualTo(message.ReplyQueue));
        });
    }
}