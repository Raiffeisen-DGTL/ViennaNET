using System.Threading.Tasks;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging
{
  /// <summary>
  /// Предоставляет функционал для непосредственной асинхронной обработки, полученного сообщений <see cref="BaseMessage"/>
  /// </summary>
  public interface IMessageProcessorAsync : IProcessorAsync
  {
    /// <summary>
    /// Асинхронно обрабатвает, полученное сообщение <see cref="BaseMessage"/>
    /// </summary>
    /// <param name="message">Полученное сообщение <see cref="BaseMessage"/></param>
    /// <returns>true в случае успешной обработки сообщения <see cref="BaseMessage"/>, иначе false</returns>
    Task<bool> ProcessAsync(BaseMessage message);
  }
}
