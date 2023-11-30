using System;
using System.Linq;
using System.Text;
using Confluent.Kafka;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.KafkaQueue
{
  internal static class KafkaMessageConverter
  {
    public const string KeyHeaderName = "KafkaKey";
    public const string CorrelationIdHeaderName = "kafka_correlationId";

    public static Message<byte[], byte[]> ConvertToKafkaMessage(this BaseMessage message)
    {
      if (string.IsNullOrWhiteSpace(message.MessageId))
      {
        message.MessageId = Guid.NewGuid()
          .ToString()
          .ToUpper();
      }

      message.SendDateTime = DateTime.Now;

      var key = Array.Empty<byte>();
      var headers = new Headers();
      foreach (var property in message.Properties)
      {
        if (property.Key == KeyHeaderName)
        {
          key = ObjectToBytes(property.Value);
        }
        else if (property.Value is string str)
        {
          headers.Add(property.Key, Encoding.UTF8.GetBytes(str));
        }
        else
        {
          throw new
            MessagingException($"The value of property {property.Key} is not a string. "
                               + "Kafka adapter support only string properties");
        }
      }

      if (headers.All(h => h.Key != CorrelationIdHeaderName))
      {
        var headerValue = string.IsNullOrEmpty(message.CorrelationId) ? message.MessageId : message.CorrelationId;
        headers.Add(CorrelationIdHeaderName, Encoding.UTF8.GetBytes(headerValue));
      }

      return new Message<byte[], byte[]>
      {
        Timestamp = new Timestamp(message.SendDateTime.GetValueOrDefault()),
        Headers = headers,
        Key = key,
        Value = message.GetMessageBodyAsBytes()
      };
    }

    public static BaseMessage ConvertToBaseMessage(this ConsumeResult<byte[], byte[]> receivedMessage)
    {
      var message = new BytesMessage
      {
        Body = receivedMessage.Message.Value,
        ReceiveDate = DateTime.Now,
        SendDateTime = receivedMessage.Message.Timestamp.UtcDateTime,
        ReplyQueue = receivedMessage.Topic
      };

      if (receivedMessage.Message.Key is { Length: > 0 })
      {
        message.Properties.Add(KeyHeaderName, receivedMessage.Message.Key);
      }

      foreach (var header in receivedMessage.Message.Headers)
      {
        if (header.Key == CorrelationIdHeaderName)
        {
          message.MessageId = message.CorrelationId = Encoding.UTF8.GetString(header.GetValueBytes());
        }
        else
        {
          message.Properties.Add(header.Key, Encoding.UTF8.GetString(header.GetValueBytes()));
        }
      }

      return message;
    }

    private static byte[]? ObjectToBytes(object? value)
    {
      return value switch
      {
        bool b => BitConverter.GetBytes(b),
        int i => BitConverter.GetBytes(i),
        long l => BitConverter.GetBytes(l),
        float f => BitConverter.GetBytes(f),
        double d => BitConverter.GetBytes(d),
        string s => Encoding.UTF8.GetBytes(s),
        byte[] bytes => bytes,
        null => null,
        _ => throw new MessagingException($"Kafka adapter cannot serialize value of type {value.GetType().Name}")
      };
    }
  }
}