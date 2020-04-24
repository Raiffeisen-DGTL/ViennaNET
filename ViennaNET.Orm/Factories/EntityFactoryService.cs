using System;
using System.Data;
using System.Threading;
using ViennaNET.Orm.DI;
using ViennaNET.Orm.Repositories;
using ViennaNET.Orm.Seedwork;
using ViennaNET.Utils;
using NHibernate;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Factories
{
  /// <inheritdoc />
  public class EntityFactoryService : IEntityFactoryService
  {
    private readonly IApplicationContextProvider _applicationContextProvider;
    private readonly ISessionFactoryManager _factoryManager;

    private readonly AsyncLocal<ExplicitNhSessionScope> _scopes = new AsyncLocal<ExplicitNhSessionScope>();
    private readonly ISessionManagerProvider _sessionManagerProvider;

    public EntityFactoryService(
      ISessionManagerProvider sessionManagerProvider, ISessionFactoryManager factoryManager,
      IApplicationContextProvider applicationContextProvider)
    {
      _sessionManagerProvider = sessionManagerProvider.ThrowIfNull(nameof(sessionManagerProvider));
      _factoryManager = factoryManager.ThrowIfNull(nameof(factoryManager));
      _applicationContextProvider = applicationContextProvider.ThrowIfNull(nameof(applicationContextProvider));
    }

    /// <inheritdoc />
    public IEntityRepository<T> Create<T>() where T : class
    {
      var type = typeof(T);
      var nick = _applicationContextProvider.GetNick(type);
      return new EntityRepository<T>(GetSession(nick));
    }

    /// <inheritdoc />
    public IUnitOfWork Create(
      IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool autoControl = true, bool closeSessions = false)
    {
      return new UnitOfWork(this, GetSessionManager(), isolationLevel, autoControl, closeSessions);
    }

    /// <inheritdoc />
    public IDisposable GetScopedSession()
    {
      var scope = new ExplicitNhSessionScope(new ScopedSessionManager(_factoryManager));
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
    public ICommandExecutor<T> CreateCommandExecutor<T>() where T : BaseCommand
    {
      var type = typeof(T);
      var nick = _applicationContextProvider.GetNickForCommand(type);
      return new CommandExecutor<T>(GetSession(nick));
    }

    /// <inheritdoc />
    public ICustomQueryExecutor<T> CreateCustomQueryExecutor<T>() where T : class
    {
      return (ICustomQueryExecutor<T>)Create<T>();
    }

    private void ScopeOnDisposed(object sender, EventArgs eventArgs)
    {
      ((ExplicitNhSessionScope)sender).Disposed -= ScopeOnDisposed;
      _scopes.Value = null;
    }

    /// <inheritdoc />
    public object GetByNameSingle(string namedQuery)
    {
      var nick = _applicationContextProvider.GetNickForNamedQuery(namedQuery.ThrowIfNull(nameof(namedQuery)));
      var session = GetSession(nick);
      return session.GetNamedQuery(namedQuery)
                    .UniqueResult();
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
