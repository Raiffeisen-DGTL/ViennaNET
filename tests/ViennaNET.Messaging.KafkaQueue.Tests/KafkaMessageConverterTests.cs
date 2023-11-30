using System;
using System.Linq;
using System.Text;
using Confluent.Kafka;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.KafkaQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(KafkaMessageConverter))]
  internal class KafkaMessageConverterTests
  {
    [Test]
    public void ConvertToKafkaMessage_TextMessageNoKey()
    {
      var srcMessage = new TextMessage { Body = "Body text" };
      srcMessage.Properties.Add("Prop1", "Val1");
      var property = srcMessage.Properties.First();

      var dstMessage = srcMessage.ConvertToKafkaMessage();

      var keyHeader = dstMessage.Headers.FirstOrDefault(h => h.Key == property.Key);
      Assert.Multiple(() =>
      {
        Assert.AreEqual(srcMessage.Body, Encoding.UTF8.GetString(dstMessage.Value));
        Assert.IsNotNull(keyHeader);
        Assert.AreEqual(property.Value, Encoding.UTF8.GetString(keyHeader.GetValueBytes()));
        Assert.AreEqual(property.Value, Encoding.UTF8.GetString(keyHeader.GetValueBytes()));
      });
    }

    [Test]
    public void ConvertToKafkaMessage_TextMessageIntProperty()
    {
      var srcMessage = new TextMessage { Body = "Body text" };
      srcMessage.Properties.Add("Prop1", 1);
      var property = srcMessage.Properties.First();

      Assert.Throws<MessagingException>(() => srcMessage.ConvertToKafkaMessage());
    }

    [Test]
    public void ConvertToKafkaMessage_BytesMessageNoKey()
    {
      var srcMessage = new BytesMessage { Body = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF } };

      var dstMessage = srcMessage.ConvertToKafkaMessage();

      var idHeader = dstMessage.Headers.FirstOrDefault(h => h.Key == KafkaMessageConverter.CorrelationIdHeaderName);
      Assert.Multiple(() =>
      {
        CollectionAssert.AreEqual(srcMessage.Body, dstMessage.Value);
        Assert.IsNotNull(idHeader);
        Assert.AreEqual(srcMessage.MessageId, Encoding.UTF8.GetString(idHeader.GetValueBytes()));
      });
    }

    public static object[] ConvertToKafkaMessage_TextMessageHasKey_Source =
    {
      new object[] { true, BitConverter.GetBytes(true) }, new object[] { 7, BitConverter.GetBytes(7) },
      new object[] { 7.7, BitConverter.GetBytes(7.7) }, new object[] { "qwe", Encoding.UTF8.GetBytes("qwe") },
      new object[] { new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, new byte[] { 0xDE, 0xAD, 0xBE, 0xEF } },
      new object[] { null, null }
    };

    [TestCaseSource(nameof(ConvertToKafkaMessage_TextMessageHasKey_Source))]
    public void ConvertToKafkaMessage_TextMessageHasKey(object key, byte[] keyBytes)
    {
      var srcMessage = new TextMessage { Body = "Body text" };
      srcMessage.Properties.Add("Prop1", "Val1");
      var property = srcMessage.Properties.First();
      srcMessage.Properties.Add("KafkaKey", key);

      var dstMessage = srcMessage.ConvertToKafkaMessage();

      var propHeader = dstMessage.Headers.FirstOrDefault(h => h.Key == property.Key);
      var idHeader = dstMessage.Headers.FirstOrDefault(h => h.Key == KafkaMessageConverter.CorrelationIdHeaderName);
      Assert.Multiple(() =>
      {
        CollectionAssert.AreEqual(keyBytes, dstMessage.Key);
        Assert.AreEqual(srcMessage.Body, Encoding.UTF8.GetString(dstMessage.Value));
        Assert.IsNotNull(propHeader);
        Assert.AreEqual(property.Value, Encoding.UTF8.GetString(propHeader.GetValueBytes()));
        Assert.IsNotNull(idHeader);
        Assert.AreEqual(srcMessage.MessageId, Encoding.UTF8.GetString(idHeader.GetValueBytes()));
      });
    }

    [Test]
    public void ConvertToKafkaMessage_TextMessageInvalidKey()
    {
      var srcMessage = new TextMessage { Body = "Body text" };
      srcMessage.Properties.Add("KafkaKey", new object());

      Assert.Throws<MessagingException>(() => srcMessage.ConvertToKafkaMessage());
    }

    [Test]
    public void ConvertToKafkaMessage_TestMessageCorrelId()
    {
      var srcMessage = new TextMessage { Body = "Body text", CorrelationId = "qwe123" };

      var dstMessage = srcMessage.ConvertToKafkaMessage();

      var idHeader = dstMessage.Headers.FirstOrDefault(h => h.Key == KafkaMessageConverter.CorrelationIdHeaderName);
      Assert.Multiple(() =>
      {
        Assert.AreEqual(srcMessage.Body, Encoding.UTF8.GetString(dstMessage.Value));
        Assert.IsNotNull(idHeader);
        Assert.AreEqual(srcMessage.CorrelationId, Encoding.UTF8.GetString(idHeader.GetValueBytes()));
      });
    }

    [Test]
    public void ConvertToBaseMessage_HasKey()
    {
      var result = new ConsumeResult<byte[], byte[]>
      {
        Topic = "kafkaTopic",
        Message = new Message<byte[], byte[]>
        {
          Key = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF },
          Value = new byte[] { 0xBE, 0xEF, 0xDE, 0xAD },
          Timestamp = new Timestamp(DateTime.Now),
          Headers = new Headers()
        }
      };
      var headerValue = "header value";
      result.Message.Headers.Add("Key1", Encoding.UTF8.GetBytes(headerValue));

      var message = result.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<BytesMessage>(message);
        Assert.AreEqual(result.Topic, message.ReplyQueue);
        CollectionAssert.AreEqual(result.Message.Value, message.GetMessageBodyAsBytes());
        Assert.AreEqual(result.Message.Timestamp.UtcDateTime, message.SendDateTime.Value);
        Assert.AreEqual(2, message.Properties.Count);
        CollectionAssert.Contains(message.Properties.Keys, result.Message.Headers[0].Key);
        Assert.AreEqual(headerValue, message.Properties["Key1"]);
        CollectionAssert.Contains(message.Properties.Keys, "KafkaKey");
        CollectionAssert.AreEqual(result.Message.Key, (byte[])message.Properties["KafkaKey"]);
      });
    }

    [Test]
    public void ConvertToBaseMessage_HasCorrelId()
    {
      var correlId = "qwe123";
      var result = new ConsumeResult<byte[], byte[]>
      {
        Topic = "kafkaTopic",
        Message = new Message<byte[], byte[]>
        {
          Value = new byte[] { 0xBE, 0xEF, 0xDE, 0xAD },
          Headers = new Headers
          {
            { KafkaMessageConverter.CorrelationIdHeaderName, Encoding.UTF8.GetBytes(correlId) }
          }
        }
      };

      var message = result.ConvertToBaseMessage();

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<BytesMessage>(message);
        Assert.AreEqual(result.Topic, message.ReplyQueue);
        CollectionAssert.AreEqual(result.Message.Value, message.GetMessageBodyAsBytes());
        Assert.AreEqual(correlId, message.CorrelationId);
        Assert.AreEqual(correlId, message.MessageId);
      });
    }
  }
}