using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Messaging.Sending
{
  /// <summary>
  ///  Сериализация сообщения в Message переде отправкой
  /// </summary>
  /// <typeparam name="TMessage">Тип отправляемого сообщения</typeparam>
  public interface ISerializedMessageSender<in TMessage> : IDisposable
  {
    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="correlationId"></param>
    /// <returns>Идентификатор отправленного сообщения</returns>
    string SendMessage(TMessage message, string correlationId = null);

    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="additionalProperties">Набор дополнительных параметров для добавления в сообщение</param>
    /// <param name="correlationId"></param>
    /// <returns>Идентификатор отправленного сообщения</returns>
    string SendMessage(TMessage message, IReadOnlyDictionary<string, object> additionalProperties, string correlationId = null);

    /// <summary>
    /// Асинхронная отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="correlationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Идентификатор отправленного сообщения</returns>
    Task<string> SendMessageAsync(TMessage message, string correlationId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронная отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="additionalProperties">Набор дополнительных параметров для добавления в сообщение</param>
    /// <param name="correlationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Идентификатор отправленного сообщения</returns>
    Task<string> SendMessageAsync(TMessage message, IReadOnlyDictionary<string, object> additionalProperties, string correlationId = null, CancellationToken cancellationToken = default);
  }
}
