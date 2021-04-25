using System;
using NHibernate;

namespace ViennaNET.Orm.Repositories
{
  /// <summary>
  /// Методы расширения для <see cref="ISession"/>.
  /// </summary>
  public static class SessionExtensions
  {
    /// <summary>
    /// Позволяет получить изначальное значение поля сущности до его изменения
    /// </summary>
    /// <param name="session">Ссылка на сессию БД</param>
    /// <param name="entity">Ссылка на сущность</param>
    /// <param name="propertyName">Имя поля</param>
    /// <returns>Изначальное значения поля по имени</returns>
    public static object GetOriginalEntityProperty(this ISession session, object entity, string propertyName)
    {
      var sessionImpl = session.GetSessionImplementation();

      var persistenceContext = sessionImpl.PersistenceContext;

      var oldEntry = persistenceContext.GetEntry(entity);
      var className = oldEntry.EntityName;

      var persister = sessionImpl.Factory.GetEntityPersister(className);

      var oldState = oldEntry.LoadedState;
      var currentState = persister.GetPropertyValues(entity);
      var dirtyProps = persister.FindDirty(currentState, oldState, entity, sessionImpl);
      var index = Array.IndexOf(persister.PropertyNames, propertyName);

      var isDirty = dirtyProps != null && Array.IndexOf(dirtyProps, index) != -1;

      return isDirty
        ? oldState[index]
        : currentState[index];
    }
  }
}
