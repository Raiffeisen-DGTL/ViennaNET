using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Orm.Application;

namespace ViennaNET.EventSourcing
{
  /// <summary>
  /// Интерфейс хранилища событий.
  /// </summary>
  public interface IEventStore
  {
    /// <summary>
    /// Запрашивает событие из хранилища.
    /// </summary>
    /// <param name="predicate">Фильтр, позволяющий получить конкретный набор событий.</param>
    /// <typeparam name="T">Тип события.</typeparam>
    /// <returns>Коллекция экземпляров событий из хранилища.</returns>
    IEnumerable<T> LoadFromStream<T>(Expression<Func<T, bool>> predicate) where T : class, IIntegrationEvent;

    /// <summary>
    /// Помещает событие в хранилище. 
    /// </summary>
    /// <param name="withCommit">Фиксирует событие в БД через unit of work</param>
    /// <param name="events">События для сохранения</param>
    /// <typeparam name="T">Тип события для сохранения.</typeparam>
    void AppendToStream<T>(bool withCommit, params IEvent[] events) where T : class, IIntegrationEvent;

    /// <summary>
    /// Помещает событие в хранилище и отправляет публикуемые события во внешнюю очередь. 
    /// </summary>
    /// <param name="withCommit">Фиксирует событие в БД через unit of work</param>
    /// <param name="queueId">Идентификатор внешней очереди.</param>
    /// <param name="events">События для сохранения.</param>
    /// <typeparam name="T">Тип события для сохранения.</typeparam>
    void AppendToStream<T>(bool withCommit, string queueId, params IEvent[] events) where T : class, IIntegrationEvent;

    /// <summary>
    /// Помещает событие в хранилище и возвращает ответ от внешней очереди.
    /// </summary>
    /// <param name="withCommit">Фиксирует событие в БД через unit of work</param>
    /// <param name="queueId">Идентификатор внешней очереди.</param>
    /// <param name="event">Событие для сохранения.</param>
    /// <typeparam name="T">Тип события для сохранения.</typeparam>
    /// <typeparam name="TReply">Тип ответа.</typeparam>
    /// <returns></returns>
    Task<TReply> AppendToStreamWithReply<T, TReply>(bool withCommit, string queueId, IEvent @event) where T : class, IIntegrationEvent where TReply : class;
  }
}
