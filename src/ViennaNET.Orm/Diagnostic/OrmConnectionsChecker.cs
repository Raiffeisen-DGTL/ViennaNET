using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Orm.Factories;
using ViennaNET.Utils;

namespace ViennaNET.Orm.Diagnostic
{
  /// <summary>
  ///   Проверяет функцию доступа к БД через ORM NHibernate.
  /// </summary>
  public class OrmConnectionsChecker : IDiagnosticImplementor
  {
    private const int DiagnosticTimeout = 300;
    private readonly ILogger _logger;
    private readonly ISessionFactoryManager _sessionFactoryManager;
    private readonly ISessionFactoryProvidersManager _sessionFactoryProvidersManager;

    /// <summary>
    ///   Инициализирует экземпляр ссылками на <see cref="ISessionFactoryProvidersManager" /> и
    ///   <see cref="ISessionFactoryManager" />
    /// </summary>
    /// <param name="sessionFactoryProvidersManager">Ссылка на интерфейс, представляющий менеджер провайдеров фабрик сессий</param>
    /// <param name="sessionFactoryManager">Ссылка на интерфейс, представляющий менеджер фабрик сессий</param>
    /// <param name="logger">Интерфейс логгирования</param>
    public OrmConnectionsChecker(
      ISessionFactoryProvidersManager sessionFactoryProvidersManager, ISessionFactoryManager sessionFactoryManager,
      ILogger<OrmConnectionsChecker> logger)
    {
      _sessionFactoryProvidersManager =
        sessionFactoryProvidersManager.ThrowIfNull(nameof(sessionFactoryProvidersManager));
      _sessionFactoryManager = sessionFactoryManager.ThrowIfNull(nameof(sessionFactoryManager));
      _logger = logger.ThrowIfNull(nameof(logger));
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

    /// <inheritdoc />
    public string Key => "ormdb";

    private DiagnosticInfo CheckProvider(ISessionFactoryProvider prov)
    {
      try
      {
        var sessionFactory = _sessionFactoryManager.GetSessionFactory(prov.Nick);
        using (var session = sessionFactory.OpenSession())
        {
          if (prov.IsSkipHealthCheckEntity)
          {
            _logger.LogTrace("Orm provider with nick {Nick} has been diagnosed successfully", prov.Nick);
            return new DiagnosticInfo($"DB: {prov.Nick}", string.Empty);
          }

          var allClassMetadata = session.SessionFactory.GetAllClassMetadata();
          foreach (var entity in allClassMetadata)
          {
            _logger.LogTrace("Diagnose the orm entity {EntityKey}", entity.Key);
            session.CreateCriteria(entity.Key)
              .SetTimeout(DiagnosticTimeout)
              .SetMaxResults(1)
              .List();
            _logger.LogTrace("Orm entity {EntityKey} has been diagnosed successfully", entity.Key);
          }
        }

        _logger.LogTrace("Orm provider with nick {Nick} has been diagnosed successfully", prov.Nick);
        return new DiagnosticInfo($"DB: {prov.Nick}", string.Empty);
      }
      catch (Exception e)
      {
        _logger.LogTrace(e, "Diagnostic of orm provider with nick {Nick} has been failed with error", prov.Nick);
        return new DiagnosticInfo($"DB: {prov.Nick}", string.Empty, DiagnosticStatus.DbConnectionError, string.Empty,
          e.ToString());
      }
    }
  }
}