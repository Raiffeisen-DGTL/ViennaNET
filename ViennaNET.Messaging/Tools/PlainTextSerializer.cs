using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tools
{
  /// <summary>
  /// Текстовый сериализатор
  /// </summary>
  public class PlainTextSerializer : IMessageSerializer<string>, IMessageDeserializer<string>
  {
    /// <inheritdoc />
    public BaseMessage Serialize(string message) => new TextMessage { Body = message };

    /// <inheritdoc />
    public string Deserialize(BaseMessage message) => ((TextMessage)message).Body;
  }
}
