﻿using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tools
{
  /// <summary>
  ///   Текстовый сериализатор
  /// </summary>
  public class PlainTextSerializer : IMessageSerializer<string>, IMessageDeserializer<string>
  {
    private const string _contentType = "Text";

    /// <inheritdoc />
    public string Deserialize(BaseMessage message)
    {
      return ((TextMessage)message).Body;
    }

    /// <inheritdoc />
    public BaseMessage Serialize(string message)
    {
      return new TextMessage { Body = message, ContentType = _contentType };
    }
  }
}