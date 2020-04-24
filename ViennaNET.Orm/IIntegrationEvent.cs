using System;

namespace ViennaNET.Orm.Application
{
  /// <summary>
  /// Интерфейс для интеграционных событий
  /// </summary>
  public interface IIntegrationEvent
  {
    /// <summary>
    /// Идентификатор события
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Идентификатор типа события
    /// </summary>
    int Type { get; }

    /// <summary>
    /// Дата стоздания
    /// </summary>
    DateTime Timestamp { get; }

    /// <summary>
    /// Инициатор события
    /// </summary>
    string Initiator { get; }

    /// <summary>
    /// Сериализованное тело события
    /// </summary>
    string Body { get; }

    /// <summary>
    /// Признак для отправки события в очередь при сохранении
    /// </summary>
    bool IsSendable { get; }
  }
}
