using ViennaNET.EventSourcing.EventMappers;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace ViennaNET.EventSourcing.DefaultConfiguration
{
  /// <inheritdoc />
  /// <summary>
  /// Пакет SimpleInjector для работы с хранилищем событий <see cref="EventStore"/>
  /// </summary>
  public class EventSourcingPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      container.RegisterSingleton<IIntegrationEventMapperFactory, IntegrationEventMapperFactory>();
      container.RegisterSingleton<IEventStore, EventStore>();
    }
  }
}
