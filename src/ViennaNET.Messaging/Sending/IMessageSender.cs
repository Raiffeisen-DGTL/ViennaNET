using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Sending
{
  /// <inheritdoc cref="IDisposable"/>
  /// <summary>
  ///   Описывает возможности, которые предоставлены для отправки сообщения
  /// </summary>
  public interface IMessageSender : IDisposable
  {
    /// <summary>
    /// Отправляет сообщение в очередь
    /// </summary>
    /// <param name="message">Сообщение <see cref="BaseMessage"/></param>
    /// <param name="additionalProperties">Дополнительные параметры для перечачи в заголовках сообщения</param>
    /// <returns>Идентификатор сообщения</returns>
    string SendMessage(BaseMessage message, IReadOnlyDictionary<string, object> additionalProperties = null);

    /// <summary>
    /// Отправляет сообщение в очередь
    /// </summary>
    /// <param name="message">Сообщение <see cref="BaseMessage"/></param>
    /// <param name="additionalProperties">Дополнительные параметры для перечачи в заголовках сообщения</param>
    /// <param name="cancellationToken">Токен отмены <see cref="CancellationToken"/></param>
    /// <returns>Идентификатор сообщения</returns>
    Task<string> SendAsync(BaseMessage message, IReadOnlyDictionary<string, object> additionalProperties = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет сообщение в очередь и ждет ответа через другую очередь
    /// </summary>
    /// <param name="message">Сообщение <see cref="BaseMessage"/></param>
    /// <param name="additionalProperties">Дополнительные параметры для перечачи в заголовках сообщения</param>
    /// <returns>Ответное сообщение</returns>
    BaseMessage SendAndWaitReplyMessage(BaseMessage message, IReadOnlyDictionary<string, object> additionalProperties = null);

    /// <summary>
    /// Отправляет сообщение в очередь и ждет ответа через другую очередь
    /// </summary>
    /// <param name="message">Сообщение <see cref="BaseMessage"/></param>
    /// <param name="additionalProperties">Дополнительные параметры для перечачи в заголовках сообщения</param>
    /// <param name="cancellationToken">Токен отмены <see cref="CancellationToken"/></param>
    /// <returns>Ответное сообщение</returns>
    Task<BaseMessage> SendAndWaitReplyMessageAsync(BaseMessage message, IReadOnlyDictionary<string, object> additionalProperties = null, CancellationToken cancellationToken = default);
  }
}
