using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.TestUtils.Orm
{
  /// <summary>
  ///   IEntityFactoryService stub builder.
  ///   Supposed to use like this:
  ///   var entityFactoryServiceStub = EntityFactoryServiceStubBuilder.Create().
  ///     .With(EntityRepositoryStub.Create(new [] { adminUser, guestUser }))
  ///     .With(EntityRepositoryStub.Create(new Session[] {}))
  ///     .Build();
  /// </summary>
  public class EntityFactoryServiceStubBuilder
  {
    private readonly EntityFactoryServiceStub _stub = new EntityFactoryServiceStub();

    private EntityFactoryServiceStubBuilder()
    {
    }

    /// <summary>
    ///   Factory method to create EntityFactoryServiceStubBuilder instance
    /// </summary>
    /// <returns>new EntityFactoryServiceStubBuilder instance</returns>
    public static EntityFactoryServiceStubBuilder Create()
    {
      return new EntityFactoryServiceStubBuilder();
    }

    /// <summary>
    ///   Add IEntityRepository to EntityFactoryService
    ///   You may create your own IEntityRepository mock or use our EntityRepositoryStub class
    /// </summary>
    /// <param name="repository">repository to add to EntityFactoryService</param>
    /// <typeparam name="T">Repository Key type</typeparam>
    /// <returns>Returns this builder to enable method chaining</returns>
    public EntityFactoryServiceStubBuilder WithRepository<T>(IEntityRepository<T> repository) where T : class
    {
      _stub.AddRepository(repository);

      return this;
    }

    /// <summary>
    ///   Add ICustomQueryExecutor to EntityFactoryService
    ///   You may create your own ICustomQueryExecutor mock or use our CustomQueryExecutorStub class
    /// </summary>
    /// <param name="customQueryExecutor">ICustomQueryExecutor instance</param>
    /// <typeparam name="T">Query result type</typeparam>
    /// <returns>Returns this builder to enable method chaining</returns>
    public EntityFactoryServiceStubBuilder WithCustomQueryExecutor<T>(ICustomQueryExecutor<T> customQueryExecutor) where T : class
    {
      _stub.AddCustomQueryExecutor(customQueryExecutor);

      return this;
    }

    /// <summary>
    ///   Add ICommandExecutor to EntityFactoryService
    ///   You may create your own ICommandExecutor mock or use our CommandExecutorStub class
    /// </summary>
    /// <param name="commandExecutor">ICommandExecutor instance</param>
    /// <typeparam name="T">Command type</typeparam>
    /// <returns>Returns this builder to enable method chaining</returns>
    public EntityFactoryServiceStubBuilder WithCommandExecutor<T>(ICommandExecutor<T> commandExecutor) where T : class, ICommand
    {
      _stub.AddCommandExecutor(commandExecutor);

      return this;
    }

    /// <summary>
    ///   Create IEntityFactoryService stub
    /// </summary>
    /// <returns>IEntityFactoryService stub instance</returns>
    public IEntityFactoryService Build()
    {
      return _stub;
    }

    private class EntityFactoryServiceStub : IEntityFactoryService
    {
      private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
      private readonly Dictionary<Type, object> _customQueryExecutors = new Dictionary<Type, object>();
      private readonly Dictionary<Type, object> _commandExecutors = new Dictionary<Type, object>();

      public IEntityRepository<T> Create<T>() where T : class
      {
        return (IEntityRepository<T>)_repositories[typeof(T)];
      }

      public IUnitOfWork Create(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        bool autoControl = true,
        bool closeSessions = false)
      {
        return new Uow();
      }

      public IDisposable GetScopedSession()
      {
        return new ScopedSession();
      }

      public T GetByNameSingle<T>(string namedQuery)
      {
        throw new NotImplementedException();
      }

      public object GetByNameSingle(string namedQuery)
      {
        throw new NotImplementedException();
      }

      public ICommandExecutor<T> CreateCommandExecutor<T>() where T : class, ICommand
      {
        return (ICommandExecutor<T>)_commandExecutors[typeof(T)];
      }

      public ICustomQueryExecutor<T> CreateCustomQueryExecutor<T>() where T : class
      {
        return (ICustomQueryExecutor<T>)_customQueryExecutors[typeof(T)];
      }

      public void AddRepository<T>(IEntityRepository<T> repository) where T : class
      {
        _repositories[typeof(T)] = repository;
      }

      public void AddCustomQueryExecutor<T>(ICustomQueryExecutor<T> customQueryExecutor) where T : class
      {
        _customQueryExecutors[typeof(T)] = customQueryExecutor;
      }

      public void AddCommandExecutor<T>(ICommandExecutor<T> commandExecutor) where T : class, ICommand
      {
        _commandExecutors[typeof(T)] = commandExecutor;
      }

      private class ScopedSession : IDisposable
      {
        public void Dispose()
        {
        }
      }
    }

    private class Uow : IUnitOfWork
    {
      public bool WasDisposed { get; private set; }

      public bool WasCommitted { get; private set; }

      public bool WasSaved { get; private set; }

      public bool WasRolledBack { get; private set; }

      public void Dispose()
      {
        WasDisposed = true;
      }

      public void Commit()
      {
        WasCommitted = true;
      }

      public Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
      {
        WasCommitted = true;
        return Task.CompletedTask;
      }

      public void Save()
      {
        WasSaved = true;
      }

      public Task SaveAsync(CancellationToken cancellationToken = new CancellationToken())
      {
        WasSaved = true;
        return Task.CompletedTask;
      }

      public void Rollback()
      {
        WasRolledBack = true;
      }

      public Task RollbackAsync(CancellationToken cancellationToken = new CancellationToken())
      {
        WasRolledBack = true;
        return Task.CompletedTask;
      }

      public void MarkDirty<TEntity>(TEntity entity) where TEntity : class
      {
        throw new NotImplementedException();
      }

      public void MarkDeleted<TEntity>(TEntity entity) where TEntity : class
      {
        throw new NotImplementedException();
      }

      public void MarkNew<TEntity>(TEntity entity) where TEntity : class
      {
        throw new NotImplementedException();
      }

      public Task MarkDirtyAsync<TEntity>(TEntity entity,
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
      {
        throw new NotImplementedException();
      }

      public Task MarkDeletedAsync<TEntity>(TEntity entity,
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
      {
        throw new NotImplementedException();
      }

      public Task MarkNewAsync<TEntity>(TEntity entity,
        CancellationToken cancellationToken = new CancellationToken())
        where TEntity : class
      {
        throw new NotImplementedException();
      }
    }
  }
}