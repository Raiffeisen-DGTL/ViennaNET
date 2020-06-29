using System;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging
{
  /// <summary>
  ///   Описывает возможности, адаптера обмена сообщениями
  /// </summary>
  public interface IMessageAdapter : IDisposable
  {
    /// <summary>
    ///   Признак что адаптер подключен к очереди
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    ///   Конфигурация адаптера
    /// </summary>
    QueueConfigurationBase Configuration { get; }

    /// <summary>
    ///   Подключение к очереди
    /// </summary>
    void Connect();

    /// <summary>
    ///   Отключиться от очереди
    /// </summary>
    void Disconnect();

    /// <summary>
    ///   Отправка сообщения
    /// </summary>
    /// <param name="message">Отправка сообщения</param>
    /// <returns>Отправить сообщение с дополнительной информацией</returns>
    BaseMessage Send(BaseMessage message);

    /// <summary>
    ///   Получить сообщение из очереди
    /// </summary>
    /// <param name="correlationId">Correlation id сообщения</param>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <returns>Полученное сообщение</returns>
    BaseMessage Receive(string correlationId = null, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters);

    /// <summary>
    ///   Получить сообщение из очереди
    /// </summary>
    /// <param name="correlationId">Correlation id сообщения</param>
    /// <param name="timeout">Время ожидания для запроса сообщения</param>
    /// <param name="additionalParameters">Дополнительные параметры для приема сообщения</param>
    /// <param name="message">Полученное сообщение</param>
    bool TryReceive(
      [CanBeNull] out BaseMessage message, [CanBeNull] string correlationId = null, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters);

    /// <summary>
    ///   Проверяет, поддерживает ли адаптер работу по переданному типу обработки
    /// </summary>
    /// <param name="processingType">Тип обработки</param>
    /// <returns>Признак поддержки типа</returns>
    bool SupportProcessingType(MessageProcessingType processingType);
  }
}