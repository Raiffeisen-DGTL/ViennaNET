using System;
using System.Collections.Generic;
using EasyNetQ;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.RabbitMQQueue;

namespace ViennaNET.Messaging.Tests.Unit.RabbitMq
{
  [TestFixture(Category = "Unit", TestOf = typeof(RabbitMqQueueMessageConverter))]
  public class RabbitMqQueueMessageConverterTests
  {
    private const string MessageId = "abc";
    private const string CorrelationId = "def";
    private const string ReplyQueue = "ReplyQueue";
    private const string Expiration = "3600000";
    private const string TextContentType = "Text";
    private const string BytesContentType = "Bytes";
    private const string QueueId = "id";

    private class TestMessage : BaseMessage
    {
      public override string LogBody()
      {
        throw new NotImplementedException();
      }

      public override bool IsEmpty()
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void ConvertToBaseMessage_CorrectBodyAndMessageProperties_CorrectResult()
    {
      var body = new byte[0];
      var messageProperties = new MessageProperties
      {
        MessageId = MessageId,
        CorrelationId = CorrelationId,
        ReplyTo = ReplyQueue,
        Expiration = Expiration,
        Timestamp = new DateTime(2014, 10, 9).ToFileTimeUtc(),
        ContentType = TextContentType
      };

      var message = body.ConvertToBaseMessage(messageProperties);

      Assert.Multiple(() =>
      {
        Assert.That(message.MessageId == MessageId);
        Assert.That(message.CorrelationId == CorrelationId);
        Assert.That(message.Properties, Is.Empty);
        Assert.That(message.ReplyQueue == ReplyQueue);
        Assert.That(message.LifeTime == TimeSpan.Parse(messageProperties.Expiration));
        Assert.That(message.SendDateTime == new DateTime(2014, 10, 9));
      });
    }

    [Test]
    public void ConvertToBaseMessage_MessagePropertiesWithBytesContent_BytesMessage()
    {
      var body = new byte[0];
      var messageProperties = new MessageProperties { ContentType = BytesContentType, Expiration = Expiration };

      var message = body.ConvertToBaseMessage(messageProperties);

      Assert.That(message is BytesMessage);
    }

    [Test]
    public void ConvertToBaseMessage_MessagePropertiesWithHeaders_PropertiesFilled()
    {
      var body = new byte[0];
      var messageProperties = new MessageProperties
      {
        Expiration = Expiration,
        Headers = new Dictionary<string, object> { { "a", 1 }, { "b", MessageId }, { "c", new byte[] { 97, 98, 99 } } }
      };

      var message = body.ConvertToBaseMessage(messageProperties);

      Assert.Multiple(() =>
      {
        Assert.That(message.Properties["a"], Is.EqualTo(string.Empty));
        Assert.That(message.Properties["b"], Is.EqualTo(MessageId));
        Assert.That(message.Properties["c"], Is.EqualTo(MessageId));
      });
    }

    [Test]
    public void ConvertToBaseMessage_MessagePropertiesWithTextContent_TextMessage()
    {
      var body = new byte[0];
      var messageProperties = new MessageProperties { ContentType = TextContentType, Expiration = Expiration };

      var message = body.ConvertToBaseMessage(messageProperties);

      Assert.That(message is TextMessage);
    }

    [Test]
    public void ConvertToBaseMessage_MessagePropertiesWithUnknownContent_TextMessage()
    {
      var body = new byte[0];
      var messageProperties = new MessageProperties { ContentType = "Unknown", Expiration = Expiration };

      var message = body.ConvertToBaseMessage(messageProperties);

      Assert.That(message is TextMessage);
    }

    [Test]
    public void ConvertToProperties_BytesMessage_TypeIsText()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration { Id = QueueId, Lifetime = TimeSpan.Zero };
      var message = new BytesMessage();

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(properties.ContentType == BytesContentType);
    }

    [Test]
    public void ConvertToProperties_ConfigurationWithoutLifeTime_DefaultExpirationReturned()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration { Id = QueueId };
      var message = new TextMessage();

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(properties.Expiration == Expiration);
    }

    [Test]
    public void ConvertToProperties_CorrectMessage_CorrectResult()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration { Id = QueueId, Lifetime = TimeSpan.Zero };
      var message = new TextMessage { MessageId = MessageId, CorrelationId = CorrelationId, ReplyQueue = ReplyQueue };
      message.Properties.Add("a", 1);

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.Multiple(() =>
      {
        Assert.That(properties.MessageId == MessageId);
        Assert.That(properties.CorrelationId == CorrelationId);
        Assert.That(properties.Headers == message.Properties);
        Assert.That(properties.DeliveryMode == 2);
        Assert.That(properties.ReplyTo == ReplyQueue);
        Assert.That(properties.Expiration == ((int)queueConfiguration.Lifetime.Value.TotalMilliseconds).ToString());
      });
    }

    [Test]
    public void ConvertToProperties_NoMessageId_MessageIdFilledWithNewGuid()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration { Id = QueueId, Lifetime = TimeSpan.Zero };
      var message = new TextMessage();

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(Guid.TryParse(properties.MessageId, out _));
    }

    [Test]
    public void ConvertToProperties_TextMessage_TypeIsText()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration { Id = QueueId, Lifetime = TimeSpan.Zero };
      var message = new TextMessage();

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(properties.ContentType == TextContentType);
    }

    [Test]
    public void ConvertToProperties_UnknownMessage_Throws()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration { Id = QueueId, Lifetime = TimeSpan.Zero };
      var message = new TestMessage();

      Assert.Throws<ArgumentException>(() => message.ConvertToProperties(queueConfiguration));
    }
  }
}