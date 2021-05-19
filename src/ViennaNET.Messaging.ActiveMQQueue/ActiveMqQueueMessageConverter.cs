using System;
using Apache.NMS;
using Apache.NMS.Util;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.ActiveMQQueue
{
  internal static class ActiveMqQueueMessageConverter
  {
    public static IMessage ConvertToMqMessage(this BaseMessage message, ISession session)
    {
      var mes = ConvertToInternalMessage(message, session);
      mes.NMSMessageId = string.IsNullOrWhiteSpace(message.MessageId)
        ? Guid.NewGuid().ToString().ToUpper()
        : message.MessageId;
      mes.NMSCorrelationID = string.IsNullOrWhiteSpace(message.CorrelationId)
        ? mes.NMSMessageId
        : message.CorrelationId;
      mes.NMSTimeToLive = message.LifeTime;

      if (!string.IsNullOrWhiteSpace(message.ReplyQueue))
      {
        var replyQueue = SessionUtil.GetDestination(session, message.ReplyQueue.Trim());
        mes.NMSReplyTo = replyQueue;
      }

      return mes;
    }

    public static BaseMessage ConvertToBaseMessage(this IMessage receivedMessage)
    {
      var message = CreateBaseMessage(receivedMessage);
      message.MessageId = receivedMessage.NMSMessageId;
      message.CorrelationId = receivedMessage.NMSCorrelationID;
      message.SendDateTime = receivedMessage.NMSTimestamp;
      message.ReceiveDate = DateTime.Now;
      message.LifeTime = receivedMessage.NMSTimeToLive;

      switch (receivedMessage.NMSReplyTo)
      {
        case null:
          message.ReplyQueue = null;
          break;
        case IQueue queue:
          message.ReplyQueue = queue.QueueName;
          break;
        case ITopic topic:
          message.ReplyQueue = topic.TopicName;
          break;
      }

      if (receivedMessage.Properties != null
          && receivedMessage.Properties.Count > 0)
      {
        foreach (var property in receivedMessage.Properties.Keys)
        {
          var name = property as string;
          if (!string.IsNullOrEmpty(name))
          {
            var prop = receivedMessage.Properties[name];
            message.Properties.Add(name, prop);
          }
        }
      }

      return message;
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
          throw new ArgumentException(
            $"Unknown inherited type of BaseMessage ({message.GetType()}) while converting to IMessage");
      }
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