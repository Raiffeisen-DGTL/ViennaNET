using ViennaNET.Mediator.Seedwork;
using ViennaNET.Orm.Application;

namespace ViennaNET.EventSourcing.EventMappers
{
  /// <summary>
  ///   Интерфейс маппера, позволяющего преобразовать событие
  ///   предметной области в интеграционное событие.
  /// </summary>
  /// <typeparam name="T">Тип интеграционного события</typeparam>
  public interface IConcreteIntegrationEventMapper<out T> : IIntegrationEventMapper
    where T : IIntegrationEvent
  {
    /// <summary>
    ///   Преобразует события предметной области в интеграционное событие
    /// </summary>
    /// <param name="event">Событие предметной области</param>
    /// <param name="body">Тело интеграционного события, сериализованное в строку</param>
    /// <returns>Интеграционное событие</returns>
    T Map(IEvent @event, string body);
  }
}
