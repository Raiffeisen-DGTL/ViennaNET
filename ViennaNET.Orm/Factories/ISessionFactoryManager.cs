using System.Collections.Generic;
using NHibernate;

namespace ViennaNET.Orm.Factories
{
  /// <summary>
  /// Менеджер фабрик сессий. Обеспечивает инциализации и хранение фабрик
  /// </summary>
  public interface ISessionFactoryManager
  {
    /// <summary>
    /// Получает фабрику сессий
    /// </summary>
    /// <param name="nick">Имя подключения</param>
    /// <returns>Фабрика сессий</returns>
    ISessionFactory GetSessionFactory(string nick);

    /// <summary>
    /// Получает все сохраненные фабрики сессий
    /// </summary>
    /// <returns>Фабрики сессий</returns>
    IEnumerable<ISessionFactory> GetSessionFactories();
  }
}
