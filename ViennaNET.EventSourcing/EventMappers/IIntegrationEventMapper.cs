using System;

namespace ViennaNET.EventSourcing.EventMappers
{
  /// <summary>
  /// Базовый интерфейс преобразователя событий предметной области
  /// </summary>
  public interface IIntegrationEventMapper
  {
    /// <summary>
    /// Тип события для преобразования
    /// </summary>
    Type EventType { get; }
  }
}
