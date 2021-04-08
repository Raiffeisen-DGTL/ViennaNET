using System;
using System.Collections.Generic;
using System.Linq;
using IBM.XMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue.Tests.DSL;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueMessageConverter))]
  public class MqSeriesQueueMessageConverterTests
  {
    [Test]
    public void ConvertToBaseMessage_CorrectBytesMessage_CorrectResult()
    {
      var body = new byte[] {97, 98, 99};
      var message = Given.BytesMessage.WithBody(body).Please();

      var result = message.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<BytesMessage>(result);
        var bytesMessage = (BytesMessage)result;
        CollectionAssert.AreEqual(body, bytesMessage.Body);
      });
    }

    [Test]
    public void ConvertToBaseMessage_CorrectTextMessage_CorrectResult()
    {
      var props = new Dictionary<string, object> {{"Param1", 12}, {"Param2", "def"}};
      var message = Given.TextMessage.WithProperties(props).WithReplyQueue("Test").Please();
      message.JMSTimestamp = 23456;
      message.Text = "abc";
      message.JMSCorrelationID = "7891";
      message.JMSExpiration = message.JMSTimestamp + 333;
      message.JMSMessageID = "1234";

      var result = message.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<TextMessage>(result);
        var textMessage = (TextMessage)result;
        Assert.AreEqual("abc", textMessage.Body);
        Assert.IsNull(textMessage.ApplicationTitle);
        Assert.AreEqual("7891", textMessage.CorrelationId);
        Assert.AreEqual(TimeSpan.FromMilliseconds(333), textMessage.LifeTime);
        Assert.AreEqual("1234", textMessage.MessageId);
        CollectionAssert.AreEqual(props, result.Properties);
        Assert.AreEqual("Test", textMessage.ReplyQueue);
        Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 23, 456), textMessage.SendDateTime);
      });
    }

    [Test]
    public void ConvertToBaseMessage_SendDateNewerThanExpirationDate_ZeroLifeTime()
    {
      var message = Given.TextMessage.Please();
      message.JMSTimestamp = 23789;
      message.JMSExpiration = 23345;

      var result = message.ConvertToBaseMessage();

      Assert.AreEqual(TimeSpan.Zero, result.LifeTime);
    }

    [Test]
    public void ConvertToBaseMessage_UnknownMessage_TextMessageCreated()
    {
      var message = Mock.Of<IMessage>(m => m.PropertyNames == Enumerable.Empty<string>().GetEnumerator());

      var result = message.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<TextMessage>(result);
        var textMessage = (TextMessage)result;
        Assert.IsEmpty(textMessage.Body);
      });
    }

    [Test]
    public void ConvertToMqMessage_CorrectBytesMessage_CorrectResult()
    {
      var message = new BytesMessage
      {
        Body = new byte[] {97, 98, 99},
        MessageId = "abc",
        CorrelationId = "def",
        LifeTime = TimeSpan.FromMinutes(10),
        ReplyQueue = "Reply",
        Properties = {{"Prop1", 12}, {"Prop2", "def"}}
      };
      var session = Given.Session.Please();

      var result = message.ConvertToMqMessage(session);

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<IBytesMessage>(result);
        var bytesMessage = (IBytesMessage)result;
        Assert.AreEqual(message.Body.Length, bytesMessage.BodyLength);
        var bytes = new byte[message.Body.Length];
        bytesMessage.ReadBytes(bytes);
        CollectionAssert.AreEqual(message.Body, bytes);
        Assert.AreEqual("abc", bytesMessage.JMSMessageID);
        Assert.AreEqual(10 * 60 * 1000, bytesMessage.JMSExpiration);
        Assert.AreEqual("def", bytesMessage.JMSCorrelationID);
        Assert.AreEqual(message.ReplyQueue, bytesMessage.JMSReplyTo.Name);
        CollectionAssert.AreEquivalent(message.Properties, bytesMessage.GetProperties());
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

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<ITextMessage>(result);
        var textMessage = (ITextMessage)result;
        Assert.AreEqual("Body", textMessage.Text);
      });
    }

    [Test]
    public void ConvertToMqMessage_EmptyCorrId_MessageIdUsed()
    {
      var message = new TextMessage
      {
        Body = "Body", MessageId = "abc", CorrelationId = null, LifeTime = TimeSpan.FromMinutes(10)
      };
      var session = Given.Session.Please();

      var result = message.ConvertToMqMessage(session);

      Assert.AreEqual(message.MessageId, result.JMSCorrelationID);
    }

    [Test]
    public void ConvertToMqMessage_EmptyMessageId_GuidGenerated()
    {
      var message = new TextMessage
      {
        Body = "Body", MessageId = null, CorrelationId = "def", LifeTime = TimeSpan.FromMinutes(10)
      };
      var session = Given.Session.Please();

      var result = message.ConvertToMqMessage(session);

      Assert.IsTrue(Guid.TryParse(result.JMSMessageID, out _));
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