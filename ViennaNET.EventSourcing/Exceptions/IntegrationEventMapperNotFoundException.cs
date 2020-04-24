using System;

namespace ViennaNET.EventSourcing.Exceptions
{
  /// <summary>
  /// Исключение, возникающее в случае если для события предметной области не определен преобразователь на интеграционное событие.
  /// </summary>
  public class IntegrationEventMapperNotFoundException : Exception
  {
    /// <summary>
    /// Инициализирует экземпляр исключения сообщением, содержащим типы событий.
    /// </summary>
    /// <param name="integrationEventType">Тип интеграционного события.</param>
    /// <param name="eventType">Тип доменного события.</param>
    public IntegrationEventMapperNotFoundException(Type integrationEventType, Type eventType) :
      base($"Concrete integration event mapper from event {eventType.FullName} to integration event {integrationEventType.FullName} not found")
    {
    }
  }
}
