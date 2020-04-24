using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Repositories;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.TestUtils.Orm
{
  /// <summary>
  ///   Билдер для стаба IEntityFactoryService.
  ///   Предлагается использовать таким образом:
  ///   var entityFactoryServiceStub = EntityFactoryServiceStubBuilder.Create().
  ///   .With(EntityRepositoryStub.Create(new [] { adminUser, ruapus9User }))
  ///   .With(EntityRepositoryStub.Create(new Session[] {}))
  ///   .Build();
  /// </summary>
  public class EntityFactoryServiceStubBuilder
  {
    private readonly EntityFactoryServiceStub _stub = new EntityFactoryServiceStub();

    private EntityFactoryServiceStubBuilder()
    {
    }

    /// <summary>
    ///   Статический метод для создания билдера
    /// </summary>
    /// <returns>Экземпляр EntityFactoryServiceStubBuilder</returns>
    public static EntityFactoryServiceStubBuilder Create()
    {
      return new EntityFactoryServiceStubBuilder();
    }

    /// <summary>
    ///   Добавить репозиторий
    /// </summary>
    /// <param name="repository">Репозиторий</param>
    /// <typeparam name="T">Тип ключа в репозитории</typeparam>
    /// <returns>Возвращает this для chaining</returns>
    public EntityFactoryServiceStubBuilder WithRepository<T>(IEntityRepository<T> repository) where T : class
    {
      _stub.AddRepository(repository);

      return this;
    }

    /// <summary>
    ///   Создать IEntityFactoryService
    /// </summary>
    /// <returns>Экземпляр IEntityFactoryService</returns>
    public IEntityFactoryService Build()
    {
      return _stub;
    }

    private class EntityFactoryServiceStub : IEntityFactoryService
    {
      private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

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

      public ICommandExecutor<T> CreateCommandExecutor<T>() where T : BaseCommand
      {
        throw new NotImplementedException();
      }

      public ICustomQueryExecutor<T> CreateCustomQueryExecutor<T>() where T : class
      {
        throw new NotImplementedException();
      }

      public void AddRepository<T>(IEntityRepository<T> repository) where T : class
      {
        _repositories[typeof(T)] = repository;
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