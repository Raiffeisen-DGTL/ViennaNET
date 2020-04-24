using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tools
{
  /// <summary>
  /// Сериализатор в сообщение
  /// </summary>
  public interface IMessageSerializer
  {
  }

  /// <inheritdoc />
  public interface IMessageSerializer<in TMessage> : IMessageSerializer
  {
    /// <summary>
    /// Сериализует сообщение в <see cref="BaseMessage"/>
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    BaseMessage Serialize(TMessage message);
  }
}
