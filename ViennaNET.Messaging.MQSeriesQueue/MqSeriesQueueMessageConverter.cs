using System;
using System.Text;
using IBM.XMS;
using ViennaNET.Logging;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  internal static class MqSeriesQueueMessageConverter
  {
    public static IMessage ConvertToMqMessage(this BaseMessage message, ISession session)
    {
      var mes = ConvertToInternalMessage(message, session);
      mes.JMSMessageID = string.IsNullOrWhiteSpace(message.MessageId)
        ? Guid.NewGuid()
              .ToString()
              .ToUpper()
        : message.MessageId;
      mes.JMSCorrelationID = string.IsNullOrWhiteSpace(message.CorrelationId)
        ? mes.JMSMessageID
        : message.CorrelationId;
      mes.JMSExpiration = (int)message.LifeTime.TotalSeconds * 10;

      if (!string.IsNullOrWhiteSpace(message.ReplyQueue))
      {
        var replyQueue = session.CreateQueue(message.ReplyQueue.Trim());
        mes.JMSReplyTo = replyQueue;
      }

      foreach (var kv in message.Properties)
      {
        mes.SetObjectProperty(kv.Key, kv.Value);
      }

      return mes;
    }

    private static IMessage ConvertToInternalMessage(BaseMessage message, ISession session)
    {
      switch (message)
      {
        case TextMessage textMessage:
          return session.CreateTextMessage(textMessage.Body);
        case BytesMessage bytesMessage:
          var result = session.CreateBytesMessage();
          result.WriteBytes(bytesMessage.Body);
          return result;
        default:
          throw new ArgumentException($"Unknown inherited type of BaseMessage ({message.GetType()}) while converting to IMessage");
      }
    }

    public static BaseMessage ConvertToBaseMessage(this IMessage receivedMessage)
    {
      var sendDate = ConvertJavaTimestampToDateTime(receivedMessage.JMSTimestamp);
      var expirationDate = ConvertJavaTimestampToDateTime(receivedMessage.JMSExpiration);

      var message = CreateBaseMessage(receivedMessage);
      message.MessageId = receivedMessage.JMSMessageID;
      message.CorrelationId = receivedMessage.JMSCorrelationID;
      message.SendDateTime = sendDate;
      message.ReceiveDate = DateTime.Now;
      message.ReplyQueue = receivedMessage.JMSReplyTo?.Name;
      message.LifeTime = expirationDate > sendDate
        ? expirationDate - sendDate
        : new TimeSpan();

      // чтение properties сообщения web sphere.
      var propertyNames = receivedMessage.PropertyNames;
      var messageProps = new StringBuilder();
      while (propertyNames.MoveNext())
      {
        var name = (string)propertyNames.Current ?? string.Empty;
        var prop = receivedMessage.GetObjectProperty(name);
        message.Properties.Add(name, prop);
        messageProps = messageProps.Append(name)
                                   .Append(" = ")
                                   .Append(prop)
                                   .Append(";\n");
      }

      Logger.LogDebugFormat("Message Properties: \n" + messageProps);

      return message;
    }

    private static DateTime ConvertJavaTimestampToDateTime(long jmsTimestamp)
    {
      // Example:  Converting Java millis time to .NET time
      var baseTime = new DateTime(1970, 1, 1, 0, 0, 0);
      var utcTimeTicks = jmsTimestamp * 10000 + baseTime.Ticks;
      return new DateTime(utcTimeTicks, DateTimeKind.Utc);
    }

    private static BaseMessage CreateBaseMessage(IMessage message)
    {
      switch (message)
      {
        case ITextMessage textMessage:
          return new TextMessage { Body = textMessage.Text };
        case IBytesMessage bytesMessage:
          var buffer = new byte[bytesMessage.BodyLength];
          bytesMessage.ReadBytes(buffer);
          return new BytesMessage { Body = buffer };
        default:
          return new TextMessage { Body = string.Empty };
      }
    }
  }
}