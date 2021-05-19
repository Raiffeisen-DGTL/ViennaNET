using System.Collections.Generic;

namespace ViennaNET.Mediator.Seedwork
{
  /// <summary>
  /// Интерфейс, определяющий, может ли сущность порождать события
  /// </summary>
  public interface IEntityEventPublisher
  {
    /// <summary>
    /// Метод, устанавливающий сущности экземпляр
    /// <see cref="IEventCollector" /> для порождения событий
    /// </summary>
    /// <param name="eventCollector"></param>
    void SetCollector(IEventCollector eventCollector);

    /// <summary>
    /// Возвращает список изменений
    /// </summary>
    IReadOnlyCollection<IEvent> GetChanges();
  }
}
