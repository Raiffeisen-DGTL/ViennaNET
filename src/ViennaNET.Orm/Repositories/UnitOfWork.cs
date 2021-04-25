using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Logging;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Exceptions;
using ViennaNET.Utils;

namespace ViennaNET.Orm.Repositories
{
  /// <inheritdoc cref="IUnitOfWork"/>
  public sealed class UnitOfWork : IUnitOfWork, IUoWSettings
  {
    private readonly bool _autoControl;
    private readonly bool _closeSessions;
    private readonly IEntityFactoryService _factoryService;
    private readonly IsolationLevel _isolationLevel;
    private readonly ISessionManager _sessionManager;

    private bool _commited;
    private bool _disposed;

    public UnitOfWork(
      IEntityFactoryService factoryService, ISessionManager sessionManager, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
      bool autoControl = true, bool closeSessions = false)
    {
      _factoryService = factoryService.ThrowIfNull(nameof(factoryService));
      _sessionManager = sessionManager.ThrowIfNull(nameof(sessionManager));
      _isolationLevel = isolationLevel;
      _autoControl = autoControl;
      _closeSessions = closeSessions;

      if (!sessionManager.RegisterUoW(this))
      {
        throw new UowException("Unit of Work already exists");
      }

      _sessionManager.StartTransactionAll();
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public void Commit()
    {
      try
      {
        _sessionManager.CommitAll();
        _commited = true;
      }
      catch (Exception ex)
      {
        Logger.LogErrorFormat(ex, "UoW Error begin rollback");
        _commited = false;
      }
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public void Rollback()
    {
      _sessionManager.RollbackAll(!_commited);
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public void Save()
    {
      _sessionManager.SaveAll();
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
      await Task.WhenAll(_sessionManager.CommitAllAsync(cancellationToken))
                .ConfigureAwait(false);
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
      await Task.WhenAll(_sessionManager.RollbackAllAsync(null, cancellationToken))
                .ConfigureAwait(false);
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
      await Task.WhenAll(_sessionManager.SaveAllAsync(cancellationToken))
                .ConfigureAwait(false);
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public void MarkDirty<TEntity>(TEntity entity) where TEntity : class
    {
      var repository = _factoryService.Create<TEntity>();
      ((ISessionProvider)repository).GetCurrentSession().Evict(entity);
      repository.Add(entity);
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public void MarkDeleted<TEntity>(TEntity entity) where TEntity : class
    {
      var repository = _factoryService.Create<TEntity>();
      repository.Delete(entity);
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public void MarkNew<TEntity>(TEntity entity) where TEntity : class
    {
      var repository = _factoryService.Create<TEntity>();
      repository.Add(entity);
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public async Task MarkDirtyAsync<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class
    {
      var repository = _factoryService.Create<TEntity>();
      await ((ISessionProvider)repository).GetCurrentSession().EvictAsync(entity, token);
      await repository.AddAsync(entity, token);
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public async Task MarkDeletedAsync<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class
    {
      var repository = _factoryService.Create<TEntity>();
      await repository.DeleteAsync(entity, token);
    }

    /// <inheritdoc cref="IUnitOfWork"/>
    public async Task MarkNewAsync<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class
    {
      var repository = _factoryService.Create<TEntity>();
      await repository.AddAsync(entity, token);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    IsolationLevel IUoWSettings.IsolationLevel => _isolationLevel;

    bool IUoWSettings.AutoControl => _autoControl;

    bool IUoWSettings.CloseSessions => _closeSessions;

    ~UnitOfWork()
    {
      Dispose(false);
    }

    private void Dispose(bool disposing)
    {
      if (_disposed)
      {
        return;
      }

      if (disposing)
      {
        try
        {
          _sessionManager.RollbackAll(!_commited);
        }
        catch (Exception exc)
        {
          Logger.LogErrorFormat(exc, "UoW Error rollback");
        }

        _sessionManager.UnregisterUoW();
      }

      _disposed = true;
    }
  }
}
