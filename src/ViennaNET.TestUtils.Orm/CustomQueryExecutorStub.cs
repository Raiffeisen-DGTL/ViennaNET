using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Repositories;

namespace ViennaNET.TestUtils.Orm
{
  /// <summary>
  ///   CustomQueryExecutorStub class factory method
  ///   Usage: var cqs = CustomQueryExecutorStub.Create(items);
  ///   where items - collection of items to return
  ///   after call you may examine cqs.QueriesCalled collection to assert executed command count and contents
  /// </summary>
  public static class CustomQueryExecutorStub
  {
    /// <summary>
    ///   Create ICustomQueryExecutorStub with specified collection of items
    /// </summary>
    /// <param name="items">collection of items to return</param>
    /// <typeparam name="T">collection item type</typeparam>
    /// <returns></returns>
    public static CustomQueryExecutorStub<T> Create<T>(IEnumerable<T> items) where T : class
    {
      return new CustomQueryExecutorStub<T>(items);
    }
  }

  /// <summary>
  ///   Custom query executor stub class
  ///   Returns the same collection on each call
  ///   Saves queries passed on each call (so they can be asserted after call)
  ///   Usage: var cqs = CustomQueryExecutorStub.Create(items);
  ///   where items - collection of items to return
  /// </summary>
  /// <typeparam name="T">Result type</typeparam>
  public class CustomQueryExecutorStub<T> : ICustomQueryExecutor<T> where T : class
  {
    private readonly IList<T> _items;

    internal CustomQueryExecutorStub(IEnumerable<T> items)
    {
      _items = items.ToList();
    }

    /// <summary>
    ///   Collection of queries called (stored on each call)
    /// </summary>
    public ICollection<BaseQuery<T>> QueriesCalled { get; } = new List<BaseQuery<T>>();

    /// <inheritdoc />
    public IEnumerable<T> CustomQuery(BaseQuery<T> query)
    {
      QueriesCalled.Add(query);
      return _items;
    }

    /// <inheritdoc />
    public Task<IList<T>> CustomQueryAsync(BaseQuery<T> query, CancellationToken token = default)
    {
      QueriesCalled.Add(query);
      return Task.FromResult(_items);
    }
  }
}