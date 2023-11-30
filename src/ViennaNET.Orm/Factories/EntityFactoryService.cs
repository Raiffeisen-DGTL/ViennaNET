using System;
using System.Data;
using System.Threading;
using NHibernate;
using Microsoft.Extensions.Logging;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.DI;
using ViennaNET.Orm.Repositories;
using ViennaNET.Orm.Seedwork;
using ViennaNET.Utils;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ViennaNET.Orm.Factories
{
  /// <inheritdoc />
  public class EntityFactoryService : IEntityFactoryService
  {
    private readonly IApplicationContextProvider _applicationContextProvider;
    private readonly ISessionFactoryManager _factoryManager;
    private readonly ILoggerFactory _loggerFactory;

    private readonly AsyncLocal<ExplicitNhSessionScope> _scopes = new();
    private readonly ISessionManagerProvider _sessionManagerProvider;

    public EntityFactoryService(
      ISessionManagerProvider sessionManagerProvider, ISessionFactoryManager factoryManager,
      IApplicationContextProvider applicationContextProvider, ILoggerFactory loggerFactory)
    {
      _sessionManagerProvider = sessionManagerProvider.ThrowIfNull(nameof(sessionManagerProvider));
      _factoryManager = factoryManager.ThrowIfNull(nameof(factoryManager));
      _applicationContextProvider = applicationContextProvider.ThrowIfNull(nameof(applicationContextProvider));
      _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
    }

    /// <inheritdoc />
    public IEntityRepository<T> Create<T>() where T : class
    {
      var type = typeof(T);
      var nick = _applicationContextProvider.GetNick(type);
      var repositoryLogger = _loggerFactory.CreateLogger<EntityRepository<T>>();

      return new EntityRepository<T>(GetSession(nick), repositoryLogger);
    }

    /// <inheritdoc />
    public IUnitOfWork Create(
      IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool autoControl = true, bool closeSessions = false)
    {
      var uowLogger = _loggerFactory.CreateLogger<UnitOfWork>();
      return new UnitOfWork(this, GetSessionManager(), uowLogger, isolationLevel, autoControl, closeSessions);
    }

    /// <inheritdoc />
    public IDisposable GetScopedSession()
    {
      var managerLogger = _loggerFactory.CreateLogger<ScopedSessionManager>();
      var sessionScopeLogger = _loggerFactory.CreateLogger<ExplicitNhSessionScope>();
      var scope = new ExplicitNhSessionScope(new ScopedSessionManager(_factoryManager, managerLogger),
        sessionScopeLogger);
      scope.Disposed += ScopeOnDisposed;
      _scopes.Value = scope;
      return scope;
    }

    /// <inheritdoc />
    public T GetByNameSingle<T>(string namedQuery)
    {
      var nick = _applicationContextProvider.GetNickForNamedQuery(namedQuery.ThrowIfNull(nameof(namedQuery)));
      var session = GetSession(nick);
      return session.GetNamedQuery(namedQuery)
        .UniqueResult<T>();
    }

    /// <inheritdoc />
    public ICommandExecutor<T> CreateCommandExecutor<T>() where T : class, ICommand
    {
      var type = typeof(T);
      var nick = _applicationContextProvider.GetNickForCommand(type);
      var executorLogger = _loggerFactory.CreateLogger<CommandExecutor<T>>();

      return new CommandExecutor<T>(GetSession(nick), executorLogger);
    }

    /// <inheritdoc />
    public ICustomQueryExecutor<T> CreateCustomQueryExecutor<T>() where T : class
    {
      return (ICustomQueryExecutor<T>)Create<T>();
    }

    /// <inheritdoc />
    public object GetByNameSingle(string namedQuery)
    {
      var nick = _applicationContextProvider.GetNickForNamedQuery(namedQuery.ThrowIfNull(nameof(namedQuery)));
      var session = GetSession(nick);
      return session.GetNamedQuery(namedQuery)
        .UniqueResult();
    }

    private void ScopeOnDisposed(object sender, EventArgs eventArgs)
    {
      ((ExplicitNhSessionScope)sender).Disposed -= ScopeOnDisposed;
      _scopes.Value = null;
    }

    private ISession GetSession(string nick)
    {
      return GetSessionManager()
        .GetSession(nick);
    }

    private ISessionManager GetSessionManager()
    {
      return _scopes.Value == null
        ? _sessionManagerProvider.GetSessionManager()
        : _scopes.Value.SessionManager;
    }
  }
}