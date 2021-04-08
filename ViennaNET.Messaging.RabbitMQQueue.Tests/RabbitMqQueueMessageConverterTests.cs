using System;
using System.Collections.Generic;
using EasyNetQ;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests
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
        Assert.That(message.MessageId, Is.EqualTo(MessageId));
        Assert.That(message.CorrelationId, Is.EqualTo(CorrelationId));
        Assert.That(message.Properties, Is.Empty);
        Assert.That(message.ReplyQueue, Is.EqualTo(ReplyQueue));
        Assert.That(message.LifeTime, Is.EqualTo(TimeSpan.Parse(messageProperties.Expiration)));
        Assert.That(message.SendDateTime, Is.EqualTo(new DateTime(2014, 10, 9)));
      });
    }

    [Test]
    [TestCase(BytesContentType, typeof(BytesMessage))]
    [TestCase(TextContentType, typeof(TextMessage))]
    [TestCase("Unknown", typeof(TextMessage))]
    public void ConvertToBaseMessage_MessagePropertiesWithContentType_CorrectMessageType(string contentType,
      Type expectedType)
    {
      var body = new byte[0];
      var messageProperties = new MessageProperties {ContentType = contentType};

      var message = body.ConvertToBaseMessage(messageProperties);

      Assert.That(message, Is.InstanceOf(expectedType));
    }

    [Test]
    public void ConvertToBaseMessage_MessagePropertiesWithHeaders_PropertiesFilled()
    {
      var body = new byte[0];
      var messageProperties = new MessageProperties
      {
        Expiration = Expiration,
        Headers = new Dictionary<string, object> {{"a", 1}, {"b", MessageId}, {"c", new byte[] {97, 98, 99}}}
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
    public void ConvertToBaseMessage_MessagePropertiesWithoutExpiration_LifetimeNotFilled()
    {
      var body = new byte[0];
      var messageProperties = new MessageProperties {ContentType = TextContentType};

      var message = body.ConvertToBaseMessage(messageProperties);

      Assert.That(message.LifeTime, Is.EqualTo(default(TimeSpan)));
    }

    [Test]
    public void ConvertToProperties_BytesMessage_TypeIsText()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration {Id = QueueId, Lifetime = TimeSpan.Zero};
      var message = new BytesMessage();

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(properties.ContentType, Is.EqualTo(BytesContentType));
    }

    [Test]
    public void ConvertToProperties_ConfigurationWithLifeTime_ExpirationFromConfigurationReturned()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration {Id = QueueId, Lifetime = TimeSpan.FromMilliseconds(100)};
      var message = new TextMessage();

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(properties.Expiration, Is.EqualTo("100"));
    }

    [Test]
    public void ConvertToProperties_ConfigurationWithoutLifeTime_DefaultExpirationReturned()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration {Id = QueueId};
      var message = new TextMessage();

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(properties.Expiration, Is.EqualTo(Expiration));
    }

    [Test]
    public void ConvertToProperties_CorrectMessage_CorrectResult()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration {Id = QueueId, Lifetime = TimeSpan.Zero};
      var message = new TextMessage {MessageId = MessageId, CorrelationId = CorrelationId, ReplyQueue = ReplyQueue};
      message.Properties.Add("a", 1);

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.Multiple(() =>
      {
        Assert.That(properties.MessageId, Is.EqualTo(MessageId));
        Assert.That(properties.CorrelationId, Is.EqualTo(CorrelationId));
        Assert.That(properties.Headers, Is.EqualTo(message.Properties));
        Assert.That(properties.DeliveryMode, Is.EqualTo(2));
        Assert.That(properties.ReplyTo, Is.EqualTo(ReplyQueue));
        Assert.That(properties.Expiration,
          Is.EqualTo(((int)queueConfiguration.Lifetime.Value.TotalMilliseconds).ToString()));
      });
    }

    [Test]
    public void ConvertToProperties_MessageWithLifetime_MessageLifetimeReturned()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration {Id = QueueId};
      var message = new TextMessage {LifeTime = TimeSpan.FromMilliseconds(100)};

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(properties.Expiration, Is.EqualTo("100"));
    }

    [Test]
    public void ConvertToProperties_NoMessageId_MessageIdFilledWithNewGuid()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration {Id = QueueId, Lifetime = TimeSpan.Zero};
      var message = new TextMessage();

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(Guid.TryParse(properties.MessageId, out _));
    }

    [Test]
    public void ConvertToProperties_TextMessage_TypeIsCorrect()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration {Id = QueueId, Lifetime = TimeSpan.Zero};
      const string contentType = "application/xml";
      var message = new TextMessage {ContentType = contentType};

      var properties = message.ConvertToProperties(queueConfiguration);

      Assert.That(properties.ContentType, Is.EqualTo(contentType));
    }

    [Test]
    public void ConvertToProperties_UnknownMessage_Throws()
    {
      var queueConfiguration = new RabbitMqQueueConfiguration {Id = QueueId, Lifetime = TimeSpan.Zero};
      var message = new TestMessage();

      Assert.Throws<ArgumentException>(() => message.ConvertToProperties(queueConfiguration));
    }
  }
}