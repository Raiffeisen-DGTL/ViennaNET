using System;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Receiving
{
  /// <summary>
  ///   Описывает возможности, которые предоставлены для получения сообщений
  /// </summary>
  /// <typeparam name="TMessage">Тип сообщения</typeparam>
  public interface IMessageReceiver<TMessage> : IDisposable
  {
    /// <summary>
    /// Получает сообщение
    /// </summary>
    /// <returns>Сообщение <see cref="TMessage"/></returns>
    TMessage Receive();

    /// <summary>
    /// Получает сообщение
    /// </summary>
    /// <param name="receivedMessage">Полученное сообщение</param>
    /// <returns>Сообщение <see cref="TMessage"/></returns>
    TMessage Receive(out BaseMessage receivedMessage);

    /// <summary>
    /// Получает сообщение
    /// </summary>
    /// <param name="correlationId">Идентификатор корреляции </param>
    /// <returns>Сообщение <see cref="TMessage"/></returns>
    TMessage Receive(string correlationId);

    /// <summary>
    /// Получает сообщение
    /// </summary>
    /// <param name="correlationId">Идентификатор корреляции </param>
    /// <param name="receivedMessage">Полученное сообщение</param>
    /// <returns>Сообщение <see cref="TMessage"/></returns>
    TMessage Receive(string correlationId, out BaseMessage receivedMessage);

    /// <summary>
    /// Пробует получить сообщение
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>true, если сообщение получено, иначе false</returns>
    bool TryReceive(out TMessage message);

    /// <summary>
    /// Пробует получить сообщение
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="receivedMessage">Полученное сообщение</param>
    /// <returns>true, если сообщение получено, иначе false</returns>
    bool TryReceive(out TMessage message, out BaseMessage receivedMessage);

    /// <summary>
    /// Пробует получить сообщение
    /// </summary>
    /// <param name="correlationId">Идентификатор корреляции</param>
    /// <param name="message">Сообщение</param>
    /// <returns>true, если сообщение получено, иначе false</returns>
    bool TryReceive(string correlationId, out TMessage message);

    /// <summary>
    /// Пробует получить сообщение
    /// </summary>
    /// <param name="correlationId">Идентификатор корреляции</param>
    /// <param name="message">Сообщение</param>
    /// <param name="receivedMessage">Полученное сообщение</param>
    /// <returns>true, если сообщение получено, иначе false</returns>
    bool TryReceive(string correlationId, out TMessage message, out BaseMessage receivedMessage);
  }
}
