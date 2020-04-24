using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging
{
  /// <summary>
  /// Предоставляет функционал для непосредственной обработки, полученного сообщений <see cref="BaseMessage"/>
  /// </summary>
  public interface IMessageProcessor : IProcessor
  {
    /// <summary>
    /// Обрабатвает, полученное сообщение <see cref="BaseMessage"/>
    /// </summary>
    /// <param name="message">Полученное сообщение <see cref="BaseMessage"/></param>
    /// <returns>true в случае успешной обработки сообщения <see cref="BaseMessage"/>, иначе false</returns>
    bool Process(BaseMessage message);
  }
}
