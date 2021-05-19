using System;
using System.Reflection;
using NHibernate;

namespace ViennaNET.Orm
{
  /// <summary>
  /// Предназначен для создания <see cref="ISessionFactory"/>
  /// Каждый экземпляр провайдера соответствует одному именованному подключению к БД
  /// </summary>
  public interface ISessionFactoryProvider
  {
    /// <summary>
    /// Имя подключения к БД для провайдера
    /// </summary>
    string Nick { get; }

    /// <summary>
    ///  Признак диагностирования сущностей в БД
    /// </summary>
    bool IsSkipHealthCheckEntity { get; }

    /// <summary>
    /// Создает экземпляр фабрики сессий
    /// </summary>
    /// <returns></returns>
    ISessionFactory GetSessionFactory();

    /// <summary>
    /// Позволяет зарегистрировать сущность с маппингом в фабрике сессий
    /// </summary>
    /// <param name="type">Тип сущности</param>
    /// <param name="assembly">Сборка с сущностью</param>
    /// <returns>Провайдер сессий</returns>
    ISessionFactoryProvider AddClass(Type type, Assembly assembly = null);

    /// <summary>
    /// Позволяет зарегистрировать слепок события с маппингом в фабрике сессий
    /// </summary>
    /// <param name="type">Тип слепка события</param>
    /// <param name="assembly">Сборка со слепком события</param>
    /// <returns>Провайдер сессий</returns>
    ISessionFactoryProvider AddEvent(Type type, Assembly assembly = null);
  }
}
