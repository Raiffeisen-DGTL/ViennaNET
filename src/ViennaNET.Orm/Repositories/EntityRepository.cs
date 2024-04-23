﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Seedwork;
using ViennaNET.Utils;
using NHibernate;
using NHibernate.Engine;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Repositories
{
  internal sealed class EntityRepository<T> : IEntityRepository<T>, ICustomQueryExecutor<T>, ISessionProvider
    where T : class
  {
    private readonly ILogger _logger;
    private readonly ISession _session;

    public EntityRepository(ISession session, ILogger<EntityRepository<T>> logger)
    {
      _session = session.ThrowIfNull(nameof(session));
      _logger = logger.ThrowIfNull(nameof(logger));
    }

    public IEnumerable<T> CustomQuery(BaseQuery<T> query)
    {
      var queryInt = (ICustomQuery)query;
      var nhQuery = CreateSqlQuery(query, queryInt);
      return nhQuery.SetResultTransformer(queryInt.Transformer)
        .List<T>();
    }

    public Task<IList<T>> CustomQueryAsync(BaseQuery<T> query, CancellationToken token = default)
    {
      var queryInt = (ICustomQuery)query;
      var nhQuery = CreateSqlQuery(query, queryInt);
      return nhQuery.SetResultTransformer(queryInt.Transformer)
        .ListAsync<T>(token);
    }

    public IQueryable<T> Query()
    {
      return _session.Query<T>();
    }

    public T Get<TKey>(TKey? id) where TKey : struct
    {
      return !id.HasValue
        ? null
        : Get(id.Value);
    }

    public Task<T> GetAsync<TKey>(TKey? id, CancellationToken token = default) where TKey : struct
    {
      return !id.HasValue
        ? null
        : GetAsync(id.Value, token);
    }

    public T Get<TKey>(TKey id)
    {
      return _session.Get<T>(id);
    }

    public Task<T> GetAsync<TKey>(TKey id, CancellationToken token = default)
    {
      return _session.GetAsync<T>(id, token);
    }

    public void Add(T entity)
    {
      CheckNotReadOnly();
      _session.SaveOrUpdate(entity);
    }

    public Task AddAsync(T entity, CancellationToken token = default)
    {
      CheckNotReadOnly();
      return _session.SaveOrUpdateAsync(entity, token);
    }

    public void Insert(T entity)
    {
      CheckNotReadOnly();
      _session.Persist(entity);
    }

    public Task InsertAsync(T entity, CancellationToken token = default)
    {
      CheckNotReadOnly();
      return _session.PersistAsync(entity, token);
    }

    public void Delete(T entity)
    {
      CheckNotReadOnly();
      _session.Delete(entity);
    }

    public Task DeleteAsync(T entity, CancellationToken token = default)
    {
      CheckNotReadOnly();
      return _session.DeleteAsync(entity, token);
    }

    public ISession GetCurrentSession()
    {
      return _session;
    }

    private ISQLQuery CreateSqlQuery(BaseQuery<T> query, ICustomQuery queryInt)
    {
      var nhQuery = _session.CreateSQLQuery(queryInt.Sql);

      if (query.Parameters == null || !query.Parameters.Any())
      {
        return nhQuery;
      }

      foreach (var parameter in query.Parameters)
      {
        nhQuery.SetParameter(parameter.Key, parameter.Value);
      }

      return nhQuery;
    }

    private void CheckNotReadOnly()
    {
      if (_session.DefaultReadOnly)
      {
        throw new EntityRepositoryException("The Repository is read-only");
      }
    }
  }
}