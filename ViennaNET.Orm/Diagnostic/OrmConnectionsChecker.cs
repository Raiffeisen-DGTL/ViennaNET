using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Logging;
using ViennaNET.Orm.Factories;
using ViennaNET.Utils;

namespace ViennaNET.Orm.Diagnostic
{
  /// <summary>
  ///   Проверяет функцию доступа к БД через ORM NHibernate.
  /// </summary>
  public class OrmConnectionsChecker : IDiagnosticImplementor
  {
    private readonly ISessionFactoryProvidersManager _sessionFactoryProvidersManager;
    private readonly ISessionFactoryManager _sessionFactoryManager;
    private const int DiagnosticTimeout = 300;

    /// <summary>
    ///   Инициализирует экземпляр ссылками на <see cref="ISessionFactoryProvidersManager" /> и
    ///   <see cref="ISessionFactoryManager" />
    /// </summary>
    /// <param name="sessionFactoryProvidersManager">Ссылка на интерфейс, представляющий менеджер провайдеров фабрик сессий</param>
    /// <param name="sessionFactoryManager">Ссылка на интерфейс, представляющий менеджер фабрик сессий</param>
    public OrmConnectionsChecker(
      ISessionFactoryProvidersManager sessionFactoryProvidersManager, ISessionFactoryManager sessionFactoryManager)
    {
      _sessionFactoryProvidersManager = sessionFactoryProvidersManager.ThrowIfNull(nameof(sessionFactoryProvidersManager));
      _sessionFactoryManager = sessionFactoryManager.ThrowIfNull(nameof(sessionFactoryManager));
    }

    /// <summary>
    ///   Проверяет подключение к БД через NHibernate и запрашивает
    ///   все сущности, зарегистрированные в ограниченном контексте приложения
    /// </summary>
    /// <returns>Результат диагностики</returns>
    public Task<IEnumerable<DiagnosticInfo>> Diagnose()
    {
      return Task.FromResult(_sessionFactoryProvidersManager.GetSessionFactoryProviders()
                                                            .Select(CheckProvider));
    }

    private DiagnosticInfo CheckProvider(ISessionFactoryProvider prov)
    {
      try
      {
        var sessionFactory = _sessionFactoryManager.GetSessionFactory(prov.Nick);
        using (var session = sessionFactory.OpenSession())
        {
          if (prov.IsSkipHealthCheckEntity)
          {
            Logger.LogDiagnostic($"Orm provider with nick {prov.Nick} has been diagnosed successfully");
            return new DiagnosticInfo($"DB: {prov.Nick}", string.Empty);
          }

          var allClassMetadata = session.SessionFactory.GetAllClassMetadata();
          foreach (var entity in allClassMetadata)
          {
            Logger.LogDiagnostic($"Diagnose the orm entity {entity.Key}");
            session.CreateCriteria(entity.Key)
                   .SetTimeout(DiagnosticTimeout)
                   .SetMaxResults(1)
                   .List();
            Logger.LogDiagnostic($"Orm entity {entity.Key} has been diagnosed successfully");
          }
        }

        Logger.LogDiagnostic($"Orm provider with nick {prov.Nick} has been diagnosed successfully");
        return new DiagnosticInfo($"DB: {prov.Nick}", string.Empty);
      }
      catch (Exception e)
      {
        Logger.LogDiagnostic($"Diagnostic of orm provider with nick {prov.Nick} has been failed with error: {e}");
        return new DiagnosticInfo($"DB: {prov.Nick}", string.Empty, DiagnosticStatus.DbConnectionError, string.Empty, e.ToString());
      }
    }

    /// <inheritdoc />
    public string Key => "ormdb";
  }
}
