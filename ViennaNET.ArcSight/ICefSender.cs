using System;
using System.Collections.Generic;

namespace ViennaNET.ArcSight
{
  /// <summary>
  /// Интерфейс для работы с каналом отправки сообщений в ArcSight
  /// </summary>
  public interface ICefSender : IDisposable
  {
    /// <summary>
    /// Метод переподключения к каналу отправки.
    /// </summary>
    void Reconnect();

    /// <summary>
    /// Метод для отправки сообщения в канал.
    /// </summary>
    /// <param name="message">Сообщение в стандартном формате</param>
    /// <param name="serializer">Сериализатор для перевода сообщения в формат передачи</param>
    void Send(CefMessage message, CefMessageSerializer serializer);

    /// <summary>
    /// Метод для отправки сообщений в канал.
    /// </summary>
    /// <param name="messages">Колллекция сообщений в стандартном формате</param>
    /// <param name="serializer">Сериализатор для перевода сообщения в формат передачи</param>
    void Send(IEnumerable<CefMessage> messages, CefMessageSerializer serializer);
  }
}
