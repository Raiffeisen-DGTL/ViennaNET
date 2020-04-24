using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Logging;
using ViennaNET.Orm.Factories;
using ViennaNET.Utils;
using NHibernate;
using NHibernate.Impl;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Repositories
{
  /// <inheritdoc cref="ISessionManager"/>
  /// <remarks>
  /// Сохраняет сессии к БД в потокобезопасном словаре.
  /// Экземпляр менеджера должен соответствовать логическому контексту выполнения,
  /// например, веб-вызову или отдельной задаче.
  /// </remarks>
  public sealed class ScopedSessionManager : ISessionManager, IDisposable
  {
    private readonly ConcurrentDictionary<ISessionFactory, ISession> _context = new ConcurrentDictionary<ISessionFactory, ISession>();
    private readonly ISessionFactoryManager _sessionFactoryManager;
    private bool _disposed;

    private IUoWSettings _settings;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="ISessionFactoryManager"/>
    /// </summary>
    /// <param name="sessionFactoryManager">Ссылка на интерфейс менеджера фабрик сессий</param>
    public ScopedSessionManager(ISessionFactoryManager sessionFactoryManager)
    {
      _sessionFactoryManager = sessionFactoryManager.ThrowIfNull(nameof(sessionFactoryManager));
    }

    /// <inheritdoc cref="ISessionManager"/>
    public void CloseAll()
    {
      UnregisterUoW();

      if (!_context.Any())
      {
        Logger.LogDebugFormat("The are no bounded sessions. Nothing to close");
        return;
      }

      foreach (var session in _context.Values)
      {
        CloseSession(session);
      }
    }

    private static void CloseSession(ISession session)
    {
      Logger.LogDebugFormat("The session factory will be unbound. Current session id: {0}", session.GetHashCode());
      session.Dispose();
      Logger.LogDebugFormat("The session has been disposed.", session.GetHashCode());
    }

    /// <inheritdoc cref="ISessionManager"/>
    public ISession GetSession(string nick)
    {
      var sessionFactory = _sessionFactoryManager.GetSessionFactory(nick);
      var session = GetCurrentSession(sessionFactory);

      session.DefaultReadOnly = _settings == null;
      Logger.LogDebugFormat("The nhibernate session has been got. SessionId: {0}, ReadOnly: {1}, Nick: {2}", session.GetHashCode(),
                            session.DefaultReadOnly, nick);

      if (_settings == null)
      {
        return session;
      }

      Logger.LogDebugFormat("A unit of work has been registered, starting the transaction with a level: {0}", _settings.IsolationLevel);
      session.BeginTransaction(_settings.IsolationLevel);

      return session;
    }

    /// <inheritdoc cref="ISessionManager"/>
    public IStatelessSession GetStatelessSession(string nick)
    {
      var sessionFactory = _sessionFactoryManager.GetSessionFactory(nick);

      return sessionFactory.OpenStatelessSession();
    }

    /// <inheritdoc cref="ISessionManager"/>
    public void StartTransactionAll()
    {
      foreach (var session in _context.Values)
      {
        session.BeginTransaction(_settings.IsolationLevel);
        SetReadOnly(session, false, _settings.AutoControl);
      }
    }

    /// <inheritdoc cref="ISessionManager"/>
    public void CommitAll()
    {
      foreach (var session in _context.Values)
      {
        if (CheckCommitTransactionIsActive(session))
        {
          return;
        }

        session.Transaction.Commit();
      }
    }

    /// <inheritdoc cref="ISessionManager"/>
    public void RollbackAll(bool? existException)
    {
      foreach (var session in _context.Values)
      {
        HandleExceptionIfNeeded(existException, session);
        if (session.Transaction.IsActive)
        {
          Logger.LogDebugFormat("The transaction is active, rollback them.");
          session.Transaction.Rollback();
        }

        SetReadonlyIfNeeded(existException, session);
      }

      if (_settings.CloseSessions)
      {
        CloseAll();
      }
    }

    /// <inheritdoc cref="ISessionManager"/>
    public void SaveAll()
    {
      foreach (var session in _context.Values)
      {
        session.Flush();
      }
    }

    /// <inheritdoc cref="ISessionManager"/>
    public bool RegisterUoW(IUnitOfWork uow)
    {
      if (_settings != null)
      {
        return false;
      }

      _settings = (IUoWSettings)uow;
      return true;
    }

    /// <inheritdoc cref="ISessionManager"/>
    public void UnregisterUoW()
    {
      _settings = null;
    }

    /// <summary>
    /// </summary>
    public IEnumerable<Task> CommitAllAsync(CancellationToken cancellationToken)
    {
      return from session in _context.Values
              where !CheckCommitTransactionIsActive(session)
              select session.Transaction.CommitAsync(cancellationToken);
    }

    private static bool CheckCommitTransactionIsActive(ISession session)
    {
      Logger.LogDebugFormat("Commit the transaction. Current session id: {0}", session.GetHashCode());
      if (!session.Transaction.IsActive)
      {
        return true;
      }

      Logger.LogDebugFormat("The transaction is active, commit them.");
      return false;
    }

    /// <inheritdoc cref="ISessionManager"/>
    public IEnumerable<Task> RollbackAllAsync(bool? existException, CancellationToken cancellationToken)
    {
      var result = new List<Task>();
      foreach (var session in _context.Values)
      {
        HandleExceptionIfNeeded(existException, session);
        Logger.LogDebugFormat("The transaction is active, rollback them.");
        var rollbackTask = session.Transaction.IsActive
          ? session.Transaction.RollbackAsync(cancellationToken)
          : Task.CompletedTask;
        result.Add(rollbackTask.ContinueWith(task => SetReadonlyIfNeeded(existException, session), cancellationToken)
                          .ContinueWith(task =>
                          {
                            if (_settings.CloseSessions)
                            {
                              CloseSession(session);
                            }
                          }, cancellationToken));
      }

      return result;
    }

    private void SetReadonlyIfNeeded(bool? existException, ISession session)
    {
      if (existException.HasValue && !_settings.CloseSessions)
      {
        SetReadOnly(session, true, true);
      }
    }

    private static void HandleExceptionIfNeeded(bool? existException, ISession session)
    {
      if (existException != true)
      {
        return;
      }

      Logger.LogDebugFormat("Prepare to rollback a transaction after an exception. Current session id: {0}", session.GetHashCode());
      session.DefaultReadOnly = true;
      session.Clear();
    }

    /// <inheritdoc cref="ISessionManager"/>
    public IEnumerable<Task> SaveAllAsync(CancellationToken cancellationToken)
    {
      return from session in _context.Values
             select session.FlushAsync(cancellationToken);
    }

    private ISession GetCurrentSession(ISessionFactory sessionFactory)
    {
      return _context.GetOrAdd(sessionFactory, sf => sf.OpenSession());
    }

    private static void SetReadOnly(ISession session, bool readOnly, bool changeEntities)
    {
      if (session.DefaultReadOnly == readOnly)
      {
        return;
      }

      session.DefaultReadOnly = readOnly;
      if (!changeEntities)
      {
        return;
      }

      using (new LogAutoStopWatch($"SetReadOnly({readOnly})", LogLevel.Debug))
      {
        var context = ((SessionImpl)session).PersistenceContext;
        foreach (var entity in context.EntitiesByKey.Values)
        {
          if (!context.EntityEntries.Contains(entity))
          {
            continue;
          }

          var entityEntry = context.GetEntry(entity);
          if (entityEntry.Persister.IsMutable)
          {
            context.SetReadOnly(entity, readOnly);
          }
        }
      }
    }

    public void Dispose()
    {
      DisposeInternal();
      GC.SuppressFinalize(this);
    }

    private void DisposeInternal()
    {
      if (_disposed)
      {
        return;
      }

      try
      {
        CloseAll();
      }
      catch (Exception ex)
      {
        Logger.LogErrorFormat(ex, "Error to close NH Session");
      }

      _disposed = true;
    }

    ~ScopedSessionManager()
    {
      DisposeInternal();
    }
  }
}
