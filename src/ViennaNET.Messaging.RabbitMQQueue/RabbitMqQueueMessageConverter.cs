using System;
using System.Globalization;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  internal static class RabbitMqQueueMessageConverter
  {
    private const string DefaultLifeTime = "3600000";

    public static IBasicProperties ConvertToProperties(
      this BaseMessage message,
      IBasicProperties properties,
      TimeSpan? configLifeTime = null)
    {
      properties.MessageId = string.IsNullOrEmpty(message.MessageId)
        ? Guid.NewGuid().ToString().ToUpper()
        : message.MessageId;
      properties.Headers = message.Properties;
      properties.DeliveryMode = 2;
      properties.Expiration = GetExpiration(message, configLifeTime);
      properties.Timestamp = new AmqpTimestamp(DateTime.Now.ToFileTimeUtc());
      properties.ContentType = GetContentType(message);

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

    private static string GetExpiration(BaseMessage message, TimeSpan? configLifeTime = null)
    {
      if (message.LifeTime.TotalMilliseconds > 0)
      {
        return message.LifeTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
      }

      if (configLifeTime.HasValue)
      {
        return ((int)configLifeTime.Value.TotalMilliseconds).ToString(CultureInfo.InvariantCulture);
      }

      return DefaultLifeTime;
    }

    private static string GetContentType(BaseMessage message)
    {
      switch (message)
      {
        case TextMessage mes:
          return mes.ContentType ?? ContentType.Text.ToString("G");
        case BytesMessage _:
          return ContentType.Bytes.ToString("G");
        default:
          throw new
            ArgumentException(
              $"Unknown inherited type of BaseMessage ({message.GetType()}) while get content type of Rabbit message");
      }
    }

    public static BaseMessage ConvertToBaseMessage(this BasicDeliverEventArgs args)
    {
      return ConvertToBaseMessage(args.BasicProperties, args.Body);
    }

    public static BaseMessage ConvertToBaseMessage(this BasicGetResult getResult)
    {
      return ConvertToBaseMessage(getResult.BasicProperties, getResult.Body);
    }

    private static BaseMessage ConvertToBaseMessage(IBasicProperties properties, ReadOnlyMemory<byte> body)
    {
      BaseMessage message;
      if (properties.ContentType == ContentType.Bytes.ToString("G"))
      {
        message = new BytesMessage { Body = body.ToArray() };
      }
      else
      {
        message = new TextMessage { Body = Encoding.UTF8.GetString(body.ToArray()) };
      }

      message.MessageId = properties.MessageId;
      message.CorrelationId = properties.CorrelationId;
      message.ReplyQueue = properties.ReplyTo;
      message.SendDateTime = DateTime.FromFileTimeUtc(properties.Timestamp.UnixTime);
      message.ReceiveDate = DateTime.Now;

      if (TimeSpan.TryParse(properties.Expiration, out var lifetime))
      {
        message.LifeTime = lifetime;
      }

      if (properties.Headers != null)
      {
        foreach (var header in properties.Headers)
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
      }

      return message;
    }

    internal enum ContentType
    {
      Bytes,
      Text
    }
  }
}