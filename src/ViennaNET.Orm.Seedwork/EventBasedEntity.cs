using System.Collections.Generic;

namespace ViennaNET.Orm.Seedwork
{
  /// <summary>
  /// Базовый класс для сущности, зависящей от событий.
  /// </summary>
  /// <typeparam name="T">Тип идентификатора.</typeparam>
  /// <typeparam name="TEvent">Базовый тип события.</typeparam>
  public abstract class EventBasedEntity<T, TEvent> : IEntityKey<T>
  {
    /// <summary>
    /// Конструктор, инициализирующий сущность массивом событий.
    /// </summary>
    /// <param name="events">Массив событий.</param>
    protected EventBasedEntity(IEnumerable<TEvent> events)
    {
      foreach (var e in events)
      {
        Mutate(e);
      }
    }

    /// <inheritdoc />
    public T Id { get; }

    /// <summary>
    /// Метод для применения событий. Все методы When должны быть публичными, иначе dynamic cast упадет.
    /// </summary>
    /// <param name="e">Событие для применения.</param>
    public void Mutate(TEvent e)
    {
      ((dynamic)this).When((dynamic)e);
    }
  }
}
