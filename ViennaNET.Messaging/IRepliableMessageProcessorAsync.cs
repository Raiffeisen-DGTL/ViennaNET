using System;
using System.Threading.Tasks;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging
{
  /// <summary>
  /// Предоставляет функционал для непосредственной обработки полученного сообшений <see cref="BaseMessage"/>
  /// </summary>
  public interface IRepliableMessageProcessorAsync : IProcessorAsync
  {
    /// <summary>
    /// Обрабатвает полученное сообщение <see cref="BaseMessage"/>
    /// </summary>
    /// <param name="message">Полученное сообщение <see cref="BaseMessage"/></param>
    /// <param name="replyAction"></param>
    /// <returns>true в случае успешной обработки сообщения <see cref="BaseMessage"/>, иначе false</returns>
    Task<bool> ProcessAndReplyAsync(BaseMessage message, Action<BaseMessage> replyAction);
  }
}
