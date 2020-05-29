using System;
using System.Text;
using EasyNetQ;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  internal static class RabbitMqQueueMessageConverter
  {
    private const string DefaultLifeTime = "3600000";

    public static MessageProperties ConvertToProperties(this BaseMessage message, RabbitMqQueueConfiguration configuration)
    {
      var properties = new MessageProperties
      {
        MessageId = string.IsNullOrEmpty(message.MessageId)
          ? Guid.NewGuid()
                .ToString()
                .ToUpper()
          : message.MessageId,
        Headers = message.Properties,
        DeliveryMode = 2,
        Expiration = configuration.Lifetime.HasValue
          ? ((int)configuration.Lifetime.Value.TotalMilliseconds).ToString()
          : DefaultLifeTime,
        Timestamp = DateTime.Now.ToFileTimeUtc(),
        ContentType = GetContentType(message)
      };

      if (!string.IsNullOrWhiteSpace(message.ReplyQueue))
      {
        properties.ReplyTo = message.ReplyQueue;
      }

      if (!string.IsNullOrEmpty(message.CorrelationId))
      {
        properties.CorrelationId = message.CorrelationId;
      }

      return properties;
    }

    private static string GetContentType(BaseMessage message)
    {
      switch (message)
      {
        case TextMessage _:
          return ContentType.Text.ToString("G");
        case BytesMessage _:
          return ContentType.Bytes.ToString("G");
        default:
          throw new
            ArgumentException($"Unknown inherited type of BaseMessage ({message.GetType()}) while get content type of Rabbit message");
      }
    }

    public static BaseMessage ConvertToBaseMessage(this byte[] body, MessageProperties messageProperties)
    {
      BaseMessage message;
      if (messageProperties.ContentType == ContentType.Bytes.ToString("G"))
      {
        message = new BytesMessage { Body = body };
      }
      else
      {
        message = new TextMessage { Body = Encoding.UTF8.GetString(body) };
      }

      message.MessageId = messageProperties.MessageId;
      message.CorrelationId = messageProperties.CorrelationId;
      message.ReplyQueue = messageProperties.ReplyTo;
      message.LifeTime = TimeSpan.Parse(messageProperties.Expiration);
      message.SendDateTime = DateTime.FromFileTimeUtc(messageProperties.Timestamp);
      message.ReceiveDate = DateTime.Now;

      foreach (var header in messageProperties.Headers)
      {
        var value = string.Empty;
        if (header.Value is byte[] bytes)
        {
          value = Encoding.UTF8.GetString(bytes);
        }

        if (header.Value is string stringHeader)
        {
          value = stringHeader;
        }

        message.Properties.Add(header.Key, value);
      }

      return message;
    }

    private enum ContentType
    {
      Bytes,
      Text
    }
  }
}