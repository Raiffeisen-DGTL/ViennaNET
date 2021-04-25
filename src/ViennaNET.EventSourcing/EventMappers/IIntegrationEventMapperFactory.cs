using ViennaNET.Mediator.Seedwork;
using ViennaNET.Orm.Application;

namespace ViennaNET.EventSourcing.EventMappers
{
  /// <summary>
  /// Интерфейс фабрики подбора преобразователей событий. Используется в <see cref="EventStore"/>
  /// </summary>
  public interface IIntegrationEventMapperFactory
  {
    /// <summary>
    /// Возвращает маппер события предметной области в интеграционное событие.
    /// </summary>
    /// <typeparam name="T">Тип интеграционного события.</typeparam>
    /// <param name="event">Событие предметной области.</param>
    /// <returns>Интеграционное событие.</returns>
    IConcreteIntegrationEventMapper<T> GetMapper<T>(IEvent @event) where T : IIntegrationEvent;
  }
}
