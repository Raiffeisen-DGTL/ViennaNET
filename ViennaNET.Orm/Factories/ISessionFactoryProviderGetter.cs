using ViennaNET.Orm.Configuration;
using NHibernate;

namespace ViennaNET.Orm.Factories
{
  /// <summary>
  /// Создает провайдер для создания <see cref="ISessionFactory"/>
  /// </summary>
  public interface ISessionFactoryProviderGetter
  {
    /// <summary>
    /// Тип БД провайдера
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Создает экземпляр провайдера для БД, соответствующей типу
    /// </summary>
    /// <param name="info">Параметры подключения к БД</param>
    /// <returns>Экземпляр провайдера для <see cref="ISessionFactory"/></returns>
    ISessionFactoryProvider GetSessionFactoryProvider(ConnectionInfo info);
  }
}
