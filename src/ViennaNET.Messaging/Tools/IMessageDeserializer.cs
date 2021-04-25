using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tools
{
  /// <summary>
  /// Десериализатор сообщения
  /// </summary>
  public interface IMessageDeserializer
  {
  }

  /// <inheritdoc />
  public interface IMessageDeserializer<out TMessage> : IMessageDeserializer
  {
    /// <summary>
    /// Десереализует сообщение <see cref="BaseMessage"/>
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    TMessage Deserialize(BaseMessage message);
  }
}
