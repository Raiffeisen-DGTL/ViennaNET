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
    /// Признак что адаптер подключен к очереди
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Конфигурация адаптера
    /// </summary>
    QueueConfigurationBase Configuration { get; }

    /// <summary>
    ///   Подключение к очереди
    /// </summary>
    void Connect();

    /// <summary>
    /// Отключиться от очереди
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <param name="message">Отправка сообщения</param>
    /// <returns>Отправить сообщение с дополнительной информацией</returns>
    BaseMessage Send(BaseMessage message);

    /// <summary>
    /// Получить сообщение из очереди
    /// </summary>
    /// <param name="correlationId">Correlation id of message</param>
    /// <returns>Полученное сообщение</returns>
    BaseMessage Receive(string correlationId = null);

    /// <summary>
    /// Получить сообщение из очереди
    /// </summary>
    /// <param name="correlationId">Correlation id of message</param>
    /// <param name="message">Полученное сообщение</param>
    bool TryReceive([CanBeNull] out BaseMessage message, [CanBeNull] string correlationId = null);

    /// <summary>
    /// Проверяет, поддерживает ли адаптер работу по переданному типу обработки
    /// </summary>
    /// <param name="processingType">Тип обработки</param>
    /// <returns>Признак поддержки типа</returns>
    bool SupportProcessingType(MessageProcessingType processingType);
  }
}
