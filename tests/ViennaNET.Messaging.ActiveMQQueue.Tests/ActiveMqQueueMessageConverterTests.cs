using Apache.NMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.ActiveMQQueue.Tests.DSL;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests;

[TestFixture(Category = "Unit", TestOf = typeof(ActiveMqQueueMessageConverter))]
public class ActiveMqQueueMessageConverterTests
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
            Assert.That(bytesMessage.Body, Is.EqualTo(body).AsCollection, nameof(bytesMessage.Body));
        });
    }

    [Test]
    public void ConvertToBaseMessage_CorrectTextMessage_CorrectResult()
    {
        var testReplyQeueName = "Test";
        var testProps = new Dictionary<string, object> { { "Param1", 12 }, { "Param2", "def" } };
        var message = Given.TextMessage.WithProperties(testProps).WithReplyQueue(testReplyQeueName).Please();
        
        message.Text = "abc";
        message.NMSCorrelationID = "7891";
        message.NMSTimestamp = new DateTime(1970, 1, 1, 0, 0, 23, 456);
        message.NMSMessageId = "1234";
        message.NMSTimeToLive = TimeSpan.FromMilliseconds(333);

        var result = message.ConvertToBaseMessage();
        var textMessage = (TextMessage)result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<TextMessage>());
            Assert.That(message.Text, Is.EqualTo(textMessage.Body));
            Assert.That(textMessage.ApplicationTitle, Is.Null);
            Assert.That(message.NMSCorrelationID, Is.EqualTo(textMessage.CorrelationId));
            Assert.That(message.NMSTimeToLive, Is.EqualTo(textMessage.LifeTime));
            Assert.That(message.NMSMessageId, Is.EqualTo(textMessage.MessageId));
            Assert.That(result.Properties, Is.EqualTo(testProps).AsCollection);
            Assert.That(testReplyQeueName, Is.EqualTo(textMessage.ReplyQueue));
            Assert.That(message.NMSTimestamp, Is.EqualTo(textMessage.SendDateTime));
        });
    }

    [Test]
    public void ConvertToBaseMessage_UnknownMessage_TextMessageCreated()
    {
        var message = Mock.Of<IMessage>();
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
            Assert.That(buffer, Is.EqualTo(message.Body).AsCollection, nameof(message.Body));
            Assert.That(bytesMessage.NMSMessageId, Is.EqualTo("abc"));
            Assert.That(TimeSpan.FromMinutes(10), Is.EqualTo(bytesMessage.NMSTimeToLive));
            Assert.That(bytesMessage.NMSCorrelationID, Is.EqualTo("def"));
        });
    }

    [Test]
    public void ConvertToMqMessage_BytesMessage_EmptyHeaderRemoved()
    {
        var message = new BytesMessage
        {
            Body = "abc"u8.ToArray(),
            MessageId = "abc",
            CorrelationId = "def",
            LifeTime = TimeSpan.FromMinutes(10),
            ReplyQueue = "Reply",
            Properties = { { "Prop1", null! }, { "Prop2", "def" } }
        };
        var session = Given.Session.Please();

        var result = message.ConvertToMqMessage(session);

        Assert.Multiple(() =>
        {
            Assert.That(result.Properties.Count, Is.EqualTo(1));
            Assert.That(result.Properties.Keys, Has.Member("Prop2"));
        });
    }

    [Test]
    public void ConvertToMqMessage_CorrectTextMessage_CorrectResult()
    {
        var message = new TextMessage
        {
            Body = "Body",
            MessageId = "abc",
            CorrelationId = "def",
            LifeTime = TimeSpan.FromMinutes(10),
            Properties = { { "Prop1", 12 }, { "Prop2", "def" } }
        };
        var session = Given.Session.Please();

        var result = message.ConvertToMqMessage(session);
        var textMessage = (ITextMessage)result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ITextMessage>());
            Assert.That(message.Body, Is.EqualTo(textMessage.Text));
        });
    }

    [Test]
    public void ConvertToMqMessage_TextMessage_ConfigurationErrorsException()
    {
        var message = new TextMessage
        {
            Body = "Body",
            MessageId = "abc",
            CorrelationId = "def",
            LifeTime = TimeSpan.FromMinutes(10),
            Properties = { { "", "def" } }
        };
        var session = Given.Session.Please();

        Assert.That(() => message.ConvertToMqMessage(session), Throws.ArgumentException);
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

        Assert.That(message.MessageId, Is.EqualTo(result.NMSCorrelationID));
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

        Assert.That(Guid.TryParse(result.NMSMessageId, out _), Is.True);
    }

    [Test]
    public void ConvertToMqMessage_UnknownMessageType_Exception()
    {
        var message = Mock.Of<BaseMessage>();
        var session = Given.Session.Please();

        Assert.That(() => message.ConvertToMqMessage(session), Throws.ArgumentException);
    }
}