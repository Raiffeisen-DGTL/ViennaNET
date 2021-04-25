using NHibernate;

namespace ViennaNET.Orm
{
  /// <summary>
  /// Интерфейс-маркер, позволяющий получить текущую сессию контекста
  /// </summary>
  public interface ISessionProvider
  {
    /// <summary>
    /// Возвращает текущую сессию контекста
    /// </summary>
    /// <returns>Сессия БД</returns>
    ISession GetCurrentSession();
  }
}