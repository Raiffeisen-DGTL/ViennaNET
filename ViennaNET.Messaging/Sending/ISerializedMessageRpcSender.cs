using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Messaging.Sending
{
  /// <summary>
  ///  Сериализация сообщения в Message перед отправкой с десериализацией ответа
  /// </summary>
  /// <typeparam name="TMessage">Тип отправляемого сообщения</typeparam>
  /// <typeparam name="TResponse">Тип ответа</typeparam>
  public interface ISerializedMessageRpcSender<in TMessage, TResponse> : IDisposable
  {
    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>Идентификатор отправленного сообщения</returns>
    TResponse SendMessageAndWaitResponse(TMessage message);

    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="additionalProperties">Набор дополнительных параметров для добавления в сообщение</param>
    /// <returns>Идентификатор отправленного сообщения</returns>
    TResponse SendMessageAndWaitResponse(TMessage message, IReadOnlyDictionary<string, object> additionalProperties);

    /// <summary>
    /// Асинхронная отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Идентификатор отправленного сообщения</returns>
    Task<TResponse> SendMessageAndWaitResponseAsync(TMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронная отправка сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="additionalProperties">Набор дополнительных параметров для добавления в сообщение</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Идентификатор отправленного сообщения</returns>
    Task<TResponse> SendMessageAndWaitResponseAsync(TMessage message, IReadOnlyDictionary<string, object> additionalProperties, CancellationToken cancellationToken = default);
  }
}
