using System.Threading;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator.Collectors
{
  /// <inheritdoc />
  public sealed class EventCollectorFactory : IEventCollectorFactory
  {
    private readonly IMediator _mediator;
    private readonly AsyncLocal<IEventCollector> _collector = new AsyncLocal<IEventCollector>();

    /// <summary>
    ///  Инициализирует экземпляр <see cref = "EventCollectorFactory" /> ссылкой на <see cref = "IMediator" />.
    /// </summary>
    /// <param name="mediator">Ссылка на объект <see cref="IMediator"/>.</param>
    public EventCollectorFactory(IMediator mediator)
    {
      _mediator = mediator;
    }

    /// <inheritdoc />
    public IEventCollector Create()
    {
      if (_collector.Value is null)
      {
        _collector.Value = new EventCollector(_mediator);
      }

      return _collector.Value;
    }
  }
}
