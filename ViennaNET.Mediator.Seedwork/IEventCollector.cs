using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Mediator.Seedwork
{
  /// <summary>
  ///   Предоставляет функционал для отложенной отправки сообщений
  /// </summary>
  public interface IEventCollector : IDisposable
  {
    /// <summary>
    /// Возвращает коллекцию событий только для чтения, сохраненных для отправки
    /// </summary>
    IReadOnlyCollection<IEvent> Events { get; }

    /// <summary>
    /// Возвращает признак того, что очередь сообщений пуста
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    ///  Добавляет новое событие в очередь
    /// </summary>
    /// <param name="evt">Ссылка на сохраняемое событие</param>
    void Enqueue<TEvent>(TEvent evt)
      where TEvent : class, IEvent;

    /// <summary>
    /// Синхронно публикует сообщения
    /// </summary>
    void Publish();

    /// <summary>
    /// Асинхронно публикует событие
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task PublishAsync(CancellationToken cancellationToken = default);
  }
}
