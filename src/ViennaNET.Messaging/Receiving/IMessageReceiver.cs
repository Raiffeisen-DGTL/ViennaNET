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
    ///   Получает сообщение
    /// </summary>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <returns>Сообщение <see cref="TMessage" /></returns>
    TMessage Receive(TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters);

    /// <summary>
    ///   Получает сообщение
    /// </summary>
    /// <param name="receivedMessage">Полученное сообщение</param>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <returns>Сообщение <see cref="TMessage" /></returns>
    TMessage Receive(
      out BaseMessage receivedMessage, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters);

    /// <summary>
    ///   Получает сообщение
    /// </summary>
    /// <param name="correlationId">Идентификатор корреляции </param>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <returns>Сообщение <see cref="TMessage" /></returns>
    TMessage Receive(string correlationId, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters);

    /// <summary>
    ///   Получает сообщение
    /// </summary>
    /// <param name="correlationId">Идентификатор корреляции </param>
    /// <param name="receivedMessage">Полученное сообщение</param>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <returns>Сообщение <see cref="TMessage" /></returns>
    TMessage Receive(
      string correlationId, out BaseMessage receivedMessage, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters);

    /// <summary>
    ///   Пробует получить сообщение
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <returns>true, если сообщение получено, иначе false</returns>
    bool TryReceive(out TMessage message, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters);

    /// <summary>
    ///   Пробует получить сообщение
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="receivedMessage">Полученное сообщение</param>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <returns>true, если сообщение получено, иначе false</returns>
    bool TryReceive(
      out TMessage message, out BaseMessage receivedMessage, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters);

    /// <summary>
    ///   Пробует получить сообщение
    /// </summary>
    /// <param name="correlationId">Идентификатор корреляции</param>
    /// <param name="message">Сообщение</param>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <returns>true, если сообщение получено, иначе false</returns>
    bool TryReceive(
      string correlationId, out TMessage message, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters);

    /// <summary>
    ///   Пробует получить сообщение
    /// </summary>
    /// <param name="correlationId">Идентификатор корреляции</param>
    /// <param name="message">Сообщение</param>
    /// <param name="receivedMessage">Полученное сообщение</param>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <returns>true, если сообщение получено, иначе false</returns>
    bool TryReceive(
      string correlationId, out TMessage message, out BaseMessage receivedMessage, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters);
  }
}