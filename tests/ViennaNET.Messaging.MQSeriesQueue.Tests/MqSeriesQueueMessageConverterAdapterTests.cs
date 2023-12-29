using IBM.XMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue.Tests.DSL;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests;

[TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueMessageConverter))]
public class MqSeriesQueueMessageConverterTests
{
    [Test]
    public void ConvertToBaseMessage_CorrectBytesMessage_CorrectResult()
    {
        var body = "abc"u8.ToArray();
        var message = Given.BytesMessage.WithBody(body).Please();

        var result = message.ConvertToBaseMessage();
        var bytesMessage = (BytesMessage)result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<BytesMessage>());
            Assert.That(body, Is.EquivalentTo(bytesMessage.Body));
        });
    }

    [Test]
    public void ConvertToBaseMessage_CorrectTextMessage_CorrectResult()
    {
        var props = new Dictionary<string, object> { { "Param1", 12 }, { "Param2", "def" } };
        var message = Given.TextMessage.WithProperties(props).WithReplyQueue("Test").Please();
        message.JMSTimestamp = 23456;
        message.Text = "abc";
        message.JMSCorrelationID = "7891";
        message.JMSExpiration = message.JMSTimestamp + 333;
        message.JMSMessageID = "1234";

        var result = message.ConvertToBaseMessage();
        var textMessage = (TextMessage)result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<TextMessage>());

            Assert.That(textMessage.Body, Is.EqualTo("abc"));
            Assert.That(textMessage.ApplicationTitle, Is.Null);
            Assert.That(textMessage.CorrelationId, Is.EqualTo("7891"));
            Assert.That(textMessage.LifeTime, Is.EqualTo(TimeSpan.FromMilliseconds(333)));
            Assert.That(textMessage.MessageId, Is.EqualTo("1234"));
            Assert.That(props, Is.EquivalentTo(result.Properties));
            Assert.That(textMessage.ReplyQueue, Is.EqualTo("Test"));
            Assert.That(textMessage.SendDateTime, Is.EqualTo(new DateTime(1970, 1, 1, 0, 0, 23, 456)));
        });
    }

    [Test]
    public void ConvertToBaseMessage_SendDateNewerThanExpirationDate_ZeroLifeTime()
    {
        var message = Given.TextMessage.Please();
        message.JMSTimestamp = 23789;
        message.JMSExpiration = 23345;

        var result = message.ConvertToBaseMessage();

        Assert.That(result.LifeTime, Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void ConvertToBaseMessage_UnknownMessage_TextMessageCreated()
    {
        var message = Mock.Of<IMessage>(m => m.PropertyNames == Enumerable.Empty<string>().GetEnumerator());

        var result = message.ConvertToBaseMessage();
        var textMessage = (TextMessage)result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<TextMessage>());
            Assert.That(textMessage.Body, Is.Empty);
        });
    }

    [Test]
    public void ConvertToMqMessage_CorrectBytesMessage_CorrectResult()
    {
        var message = new BytesMessage
        {
            Body = "abc"u8.ToArray(),
            MessageId = "abc",
            CorrelationId = "def",
            LifeTime = TimeSpan.FromMinutes(10),
            ReplyQueue = "Reply",
            Properties = { { "Prop1", 12 }, { "Prop2", "def" } }
        };
        var session = Given.Session.Please();
        var result = message.ConvertToMqMessage(session);
        var bytesMessage = (IBytesMessage)result;
        var buffer = new byte[message.Body.Length];
        bytesMessage.ReadBytes(buffer);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<IBytesMessage>());
            Assert.That(message.Body, Has.Length.EqualTo(bytesMessage.BodyLength));
            Assert.That(message.Body, Is.EqualTo(buffer));
            Assert.That(bytesMessage.JMSMessageID, Is.EqualTo("abc"));
            Assert.That(bytesMessage.JMSExpiration, Is.EqualTo(10 * 60 * 1000));
            Assert.That(bytesMessage.JMSCorrelationID, Is.EqualTo("def"));
            Assert.That(message.ReplyQueue, Is.EqualTo(bytesMessage.JMSReplyTo.Name));
            Assert.That(message.Properties, Is.EqualTo(bytesMessage.GetProperties()));
        });
    }

    [Test]
    public void ConvertToMqMessage_CorrectTextMessage_CorrectResult()
    {
        var message = new TextMessage
        {
            Body = "Body", MessageId = "abc", CorrelationId = "def", LifeTime = TimeSpan.FromMinutes(10)
        };
        var session = Given.Session.Please();
        var result = message.ConvertToMqMessage(session);
        var textMessage = (ITextMessage)result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ITextMessage>());
            Assert.That(textMessage.Text, Is.EqualTo("Body"));
        });
    }

    [Test]
    public void ConvertToMqMessage_EmptyCorrId_MessageIdUsed()
    {
        var message = new TextMessage
        {
            Body = "Body", MessageId = "abc", CorrelationId = null!, LifeTime = TimeSpan.FromMinutes(10)
        };
        var session = Given.Session.Please();
        var result = message.ConvertToMqMessage(session);

        Assert.That(message.MessageId, Is.EqualTo(result.JMSCorrelationID));
    }

    [Test]
    public void ConvertToMqMessage_EmptyMessageId_GuidGenerated()
    {
        var message = new TextMessage
        {
            Body = "Body", MessageId = null!, CorrelationId = "def", LifeTime = TimeSpan.FromMinutes(10)
        };
        var session = Given.Session.Please();
        var result = message.ConvertToMqMessage(session);

        Assert.That(Guid.TryParse(result.JMSMessageID, out _), Is.True);
    }

    [Test]
    public void ConvertToMqMessage_UnknownMessageType_Exception()
    {
        var message = Mock.Of<BaseMessage>();
        var session = Given.Session.Please();

        Assert.That(() => message.ConvertToMqMessage(session), Throws.ArgumentException);
    }
}