using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Logging;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator.Collectors
{
  /// <inheritdoc />
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  internal sealed class EventCollector : IEventCollector
  {
    private readonly ConcurrentQueue<IEvent> _events;
    private readonly IMediator _mediator;
    private bool _disposed;

    /// <summary>
    ///   Инициализирует экземпляр <see cref="EventCollector" /> ссылкой на экземпляр <see cref="IMediator" />.
    /// </summary>
    /// <param name="mediator">Ссылка на объект <see cref="IMediator" />.</param>
    internal EventCollector(IMediator mediator)
    {
      _mediator = mediator;
      _events = new ConcurrentQueue<IEvent>();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IEvent> Events =>
      _events.ToList()
        .AsReadOnly();

    /// <inheritdoc />
    public bool IsEmpty => _events.IsEmpty;

    /// <inheritdoc />
    public void Enqueue<TEvent>(TEvent evt) where TEvent : class, IEvent
    {
      _events.Enqueue(evt);

      if (_disposed)
      {
        _disposed = !_disposed;
      }
    }

    /// <inheritdoc />
    /// <remarks>
    ///   Преобразование к dynamic необходимо для того, чтобы
    ///   передавался оригинальный тип события, а не <see cref="T:ViennaNET.Mediator.Seedwork.IEvent" />
    /// </remarks>
    public void Publish()
    {
      while (!_events.IsEmpty)
      {
        if (_events.TryDequeue(out var evt))
        {
          _mediator.SendMessage(evt as dynamic);
        }
      }
    }

    /// <inheritdoc />
    /// <remarks>
    ///   Преобразование к dynamic необходимо для того, чтобы
    ///   передавался оригинальный тип события, а не <see cref="T:ViennaNET.Mediator.Seedwork.IEvent" />
    /// </remarks>
    public async Task PublishAsync(CancellationToken cancellationToken = default)
    {
      var publishEventTasks = new List<Task>();

      while (!_events.IsEmpty)
      {
        if (_events.TryDequeue(out var evt))
        {
          publishEventTasks.Add(_mediator.SendMessageAsync(evt as dynamic, cancellationToken));
        }
      }

      await Task.WhenAll(publishEventTasks);
    }

    public void Dispose()
    {
      Dispose(true);
    }

    private void Dispose(bool disposing)
    {
      if (_disposed)
      {
        return;
      }

      if (disposing)
      {
        Clear();
      }

      _disposed = true;
    }

    private void Clear()
    {
      Logger.LogInfo(
        $"{nameof(Events)} collection from {nameof(EventCollector)} will be cleared. {_events.Count} events were not published.");

      if (_events.IsEmpty)
      {
        return;
      }

      while (!_events.IsEmpty)
      {
        if (_events.TryDequeue(out var evt))
        {
          Logger.LogInfo($"Event {evt.GetType().Name} were not published.");
        }
      }

      Logger.LogInfo($"{nameof(Events)} collection from {nameof(EventCollector)} has been cleared.");
    }
  }
}