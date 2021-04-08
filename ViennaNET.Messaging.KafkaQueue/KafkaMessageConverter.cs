using System;
using System.Text;
using Confluent.Kafka;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.KafkaQueue
{
  internal static class KafkaMessageConverter
  {
    public static Message<Null, byte[]> ConvertToKafkaMessage(this BaseMessage message)
    {
      if (string.IsNullOrWhiteSpace(message.MessageId))
      {
        message.MessageId = Guid.NewGuid()
          .ToString()
          .ToUpper();
      }

      message.SendDateTime = DateTime.Now;

      var headers = new Headers();
      foreach (var property in message.Properties)
      {
        if (!(property.Value is string str))
        {
          throw new
            MessagingException($"The value of property {property.Key} is not a string. Kafka adapter support only string properties");
        }

        headers.Add(property.Key, Encoding.UTF8.GetBytes(str));
      }

      return new Message<Null, byte[]>
      {
        Timestamp = new Timestamp(message.SendDateTime.GetValueOrDefault()),
        Headers = headers,
        Value = message.GetMessageBodyAsBytes()
      };
    }

    public static BaseMessage ConvertToBaseMessage(this ConsumeResult<Ignore, byte[]> receivedMessage)
    {
      var message = new BytesMessage
      {
        Body = receivedMessage.Message.Value,
        ReceiveDate = DateTime.Now,
        SendDateTime = receivedMessage.Message.Timestamp.UtcDateTime,
        ReplyQueue = receivedMessage.Topic
      };

      foreach (var header in receivedMessage.Headers)
      {
        message.Properties.Add(header.Key, Encoding.UTF8.GetString(header.GetValueBytes()));
      }

      return message;
    }
  }
}
