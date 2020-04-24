using System;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging
{
  /// <summary>
  /// Предоставляет функционал для непосредственной обработки полученного сообшений <see cref="BaseMessage"/>
  /// </summary>
  public interface IRepliableMessageProcessor : IProcessor
  {
    /// <summary>
    /// Обрабатывает полученное сообщение <see cref="BaseMessage"/>
    /// </summary>
    /// <param name="message">Полученное сообщение <see cref="BaseMessage"/></param>
    /// <param name="replyAction"></param>
    /// <returns>true в случае успешной обработки сообщения <see cref="BaseMessage"/>, иначе false</returns>
    bool ProcessAndReply(BaseMessage message, Action<BaseMessage> replyAction);
  }
}
