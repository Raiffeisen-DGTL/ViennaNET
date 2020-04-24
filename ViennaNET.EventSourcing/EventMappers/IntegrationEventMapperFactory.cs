using System.Collections.Generic;
using System.Linq;
using ViennaNET.EventSourcing.Exceptions;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Orm.Application;
using ViennaNET.Utils;

namespace ViennaNET.EventSourcing.EventMappers
{
  /// <summary>
  /// Реализация фабрики для преобразователей событий.
  /// </summary>
  public class IntegrationEventMapperFactory : IIntegrationEventMapperFactory
  {
    private readonly IEnumerable<IIntegrationEventMapper> _mappers;

    /// <summary>
    /// Инициализирует экземпляр фабрики коллекцией преобразователей событий.
    /// </summary>
    /// <param name="mappers">Коллекция преобразователей событий.</param>
    public IntegrationEventMapperFactory(IEnumerable<IIntegrationEventMapper> mappers)
    {
      _mappers = mappers.ThrowIfNull(nameof(mappers));
    }

    /// <inheritdoc />
    public IConcreteIntegrationEventMapper<T> GetMapper<T>(IEvent @event) where T : IIntegrationEvent
    {
      var mapper = _mappers.OfType<IConcreteIntegrationEventMapper<T>>()
                           .SingleOrDefault(x => x.EventType == @event.GetType());

      if (mapper is null)
      {
        throw new IntegrationEventMapperNotFoundException(typeof(T), @event.GetType());
      }

      return mapper;
    }
  }
}
