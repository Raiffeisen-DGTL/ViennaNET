using System;
using System.Collections.Generic;

namespace ViennaNET.Messaging.Configuration
{
  /// <summary>
  ///   Базовые настройки очереди
  /// </summary>
  public abstract class QueueConfigurationBase
  {
    /// <summary>
    ///   Идентификатор
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///   Сервер
    /// </summary>
    public string Server { get; set; }

    /// <summary>
    ///   Имя очереди
    /// </summary>
    public string QueueName { get; set; }

    /// <summary>
    ///   Тип взаимодействия
    /// </summary>
    public MessageProcessingType ProcessingType { get; set; }

    /// <summary>
    ///   Пользователь (пусто - без авторизации)
    /// </summary>
    public string User { get; set; }

    /// <summary>
    ///   Пароль
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Очередь для ответа
    /// </summary>
    public string ReplyQueue { get; set; }

    /// <summary>
    ///   Время жизни сообщения
    /// </summary>
    public TimeSpan? Lifetime { get; set; }

    /// <summary>
    ///   Пользовательские заголовки
    /// </summary>
    public List<CustomHeader> CustomHeaders { get; set; }

    /// <summary>
    ///   Интервал для опроса очереди (в режиме ThreadStrategy - интервал опроса, в остальных режимах - интервал
    ///   переподключения)
    /// </summary>
    public int IntervalPollingQueue { get; set; }

    /// <summary>
    ///   Признак зависимости от 'Health Check'
    /// </summary>
    public bool? ServiceHealthDependent { get; set; }

    /// <summary>
    ///   Признак использования для 'Health Check'
    /// </summary>
    public bool IsHealthCheck { get; set; }

    /// <summary>
    ///   Идентификатор потока логирования для опроса очереди
    /// </summary>
    public string PollingId { get; set; }
  }
}