using System;
using System.Collections.Generic;
using Apache.NMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.ActiveMQQueue.Tests.DSL;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(ActiveMqQueueMessageConverter))]
  public class ActiveMqQueueMessageConverterTests
  {
    [Test]
    public void ConvertToBaseMessage_CorrectBytesMessage_CorrectResult()
    {
      var body = new byte[] { 97, 98, 99 };
      var message = Given.BytesMessage.WithBody(body).Please();

      var result = message.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<BytesMessage>(result, nameof(BytesMessage));
        var bytesMessage = (BytesMessage)result;
        CollectionAssert.AreEqual(body, bytesMessage.Body, nameof(bytesMessage.Body));
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

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<TextMessage>(result, nameof(TextMessage));
        var textMessage = (TextMessage)result;
        Assert.AreEqual(message.Text, textMessage.Body, nameof(textMessage.Body));
        Assert.IsNull(textMessage.ApplicationTitle, nameof(textMessage.ApplicationTitle));
        Assert.AreEqual(message.NMSCorrelationID, textMessage.CorrelationId, nameof(textMessage.CorrelationId));
        Assert.AreEqual(message.NMSTimeToLive, textMessage.LifeTime, nameof(textMessage.LifeTime));
        Assert.AreEqual(message.NMSMessageId, textMessage.MessageId, nameof(textMessage.MessageId));
        CollectionAssert.AreEqual(testProps, result.Properties, nameof(textMessage.Properties));
        Assert.AreEqual(testReplyQeueName, textMessage.ReplyQueue, nameof(textMessage.ReplyQueue));
        Assert.AreEqual(message.NMSTimestamp, textMessage.SendDateTime, nameof(textMessage.SendDateTime));
      });
    }

    [Test]
    public void ConvertToBaseMessage_UnknownMessage_TextMessageCreated()
    {
      var message = Mock.Of<IMessage>();

      var result = message.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<TextMessage>(result, nameof(TextMessage));
        var textMessage = (TextMessage)result;
        Assert.IsEmpty(textMessage.Body, nameof(textMessage.Body));
      });
    }

    [Test]
    public void ConvertToMqMessage_CorrectBytesMessage_CorrectResult()
    {
      var message = new BytesMessage
      {
        Body = new byte[] { 97, 98, 99 },
        MessageId = "abc",
        CorrelationId = "def",
        LifeTime = TimeSpan.FromMinutes(10),
        ReplyQueue = "Reply",
        Properties = { { "Prop1", 12 }, { "Prop2", "def" } }
      };
      var session = Given.Session.Please();

      var result = message.ConvertToMqMessage(session);

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<IBytesMessage>(result, nameof(IBytesMessage));
        var bytesMessage = (IBytesMessage)result;
        Assert.AreEqual(message.Body.Length, bytesMessage.BodyLength, nameof(bytesMessage.BodyLength));
        var bytes = new byte[message.Body.Length];
        bytesMessage.ReadBytes(bytes);
        CollectionAssert.AreEqual(message.Body, bytes, nameof(message.Body));
        Assert.AreEqual("abc", bytesMessage.NMSMessageId, nameof(bytesMessage.NMSMessageId));
        Assert.AreEqual(TimeSpan.FromMinutes(10), bytesMessage.NMSTimeToLive, nameof(bytesMessage.NMSTimeToLive));
        Assert.AreEqual("def", bytesMessage.NMSCorrelationID, nameof(bytesMessage.NMSCorrelationID));
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
        LifeTime = TimeSpan.FromMinutes(10)
      };
      var session = Given.Session.Please();

      var result = message.ConvertToMqMessage(session);

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<ITextMessage>(result, nameof(ITextMessage));
        var textMessage = (ITextMessage)result;
        Assert.AreEqual(message.Body, textMessage.Text, nameof(textMessage.Text));
      });
    }

    [Test]
    public void ConvertToMqMessage_EmptyCorrId_MessageIdUsed()
    {
      var message = new TextMessage
      {
        Body = "Body",
        MessageId = "abc",
        CorrelationId = null,
        LifeTime = TimeSpan.FromMinutes(10)
      };
      var session = Given.Session.Please();

      var result = message.ConvertToMqMessage(session);

      Assert.AreEqual(message.MessageId, result.NMSCorrelationID);
    }

    [Test]
    public void ConvertToMqMessage_EmptyMessageId_GuidGenerated()
    {
      var message = new TextMessage
      {
        Body = "Body",
        MessageId = null,
        CorrelationId = "def",
        LifeTime = TimeSpan.FromMinutes(10)
      };
      var session = Given.Session.Please();

      var result = message.ConvertToMqMessage(session);

      Assert.IsTrue(Guid.TryParse(result.NMSMessageId, out _));
    }

    [Test]
    public void ConvertToMqMessage_UnknownMessageType_Exception()
    {
      var message = Mock.Of<BaseMessage>();
      var session = Given.Session.Please();

      Assert.Throws<ArgumentException>(() => message.ConvertToMqMessage(session));
    }
  }
}