using ViennaNET.Mediator.Seedwork;
using ViennaNET.Utils;
using NHibernate;
using NHibernate.Type;

namespace ViennaNET.Orm.DI
{
  /// <inheritdoc />
  /// <summary>
  /// Позволяет при загрузке и сохранении сущности через NHibernate
  /// задать ей коллектор для временного хранения созданных доменных событий
  /// </summary>
  public class DomainEventsInterceptor : EmptyInterceptor
  {
    private readonly IEventCollectorFactory _factory;

    public DomainEventsInterceptor(IEventCollectorFactory factory)
    {
      _factory = factory.ThrowIfNull(nameof(factory));
    }

    /// <summary>
    /// Задает коллектор при загрузке сущности
    /// </summary>
    public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
    {
      if (entity is IEntityEventPublisher eventSupportable)
      {
        eventSupportable.SetCollector(_factory.Create());
      }
      return false;
    }

    /// <summary>
    /// Задает коллектор при сохранении сущности. Требуется 
    /// для вызова событий после сохранения вновь созданной сущности
    /// </summary>
    public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
    {
      if (entity is IEntityEventPublisher eventSupportable)
      {
        eventSupportable.SetCollector(_factory.Create());
      }
      return false;
    }
  }
}