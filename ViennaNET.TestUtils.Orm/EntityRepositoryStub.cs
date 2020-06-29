using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Orm.Seedwork;
using ViennaNET.TestUtils.Orm.NhQueryable;

namespace ViennaNET.TestUtils.Orm
{
  /// <summary>
  ///   Статический класс-фабрика для стабов IEntityRepository
  /// </summary>
  public static class EntityRepositoryStub
  {
    /// <summary>
    ///   Функция создаёт экземпляр стаба IEntityRepository, инициализируя его коллекцией entity.
    ///   Данная перегрузка подразумевает, что каждый item коллекции реализует интерфейс IEntityKey&lt;int&gt;
    /// </summary>
    /// <param name="items">Коллекция объектов, которая будет содержаться в репозитории</param>
    /// <typeparam name="T">Тип объекта, должен реализовывать интерфейс IEntityKey&lt;int&gt;</typeparam>
    /// <returns>Экземпляр стаба IEntityRepository, содержащий переданную коллекцию объектов</returns>
    public static IEntityRepository<T> Create<T>(IEnumerable<T> items)
      where T : class, IEntityKey<int>
    {
      return new EntityRepositoryStub<T, int>(items);
    }

    /// <summary>
    ///   Функция создаёт экземпляр стаба IEntityRepository, инициализируя его коллекцией entity.
    ///   Данная перегрузка позволяет указать тип ключа. Если тип ключа int, более удобно воспользоваться перегрузкой с одним
    ///   параметром типа.
    /// </summary>
    /// <param name="items">Коллекция объектов, которая будет содержаться в репозитории</param>
    /// <typeparam name="T">Тип объекта, должен реализовывать интерфейс IEntityKey&lt;TKey&gt;</typeparam>
    /// <typeparam name="TKey">Тип ключа для entity</typeparam>
    /// <returns>Экземпляр стаба IEntityRepository, содержащий переданную коллекцию объектов</returns>
    public static IEntityRepository<T> Create<T, TKey>(IEnumerable<T> items)
      where T : class, IEntityKey<TKey>
      where TKey : IComparable, IEquatable<TKey>, IComparable<TKey>
    {
      return new EntityRepositoryStub<T, TKey>(items);
    }
  }

  internal class EntityRepositoryStub<T, TKeyType> : IEntityRepository<T>
    where T : class, IEntityKey<TKeyType>
    where TKeyType : IComparable, IEquatable<TKeyType>, IComparable<TKeyType>
  {
    private readonly ICollection<T> _items;

    public EntityRepositoryStub(IEnumerable<T> items)
    {
      _items = items.ToList();
    }

    public IQueryable<T> Query()
    {
      return new NhQueryableProxy<T>(_items);
    }

    public T Get<TKey>(TKey? id) where TKey : struct
    {
      return Get(id.Value);
    }

    public Task<T> GetAsync<TKey>(TKey? id, CancellationToken token = new CancellationToken()) where TKey : struct
    {
      return Task.FromResult(Get(id));
    }

    public T Get<TKey>(TKey id)
    {
      return _items.SingleOrDefault(i => i.Id.Equals(id));
    }

    public Task<T> GetAsync<TKey>(TKey id, CancellationToken token = new CancellationToken())
    {
      return Task.FromResult(Get(id));
    }

    public void Add(T entity)
    {
      _items.Remove(entity);
      _items.Add(entity);
    }

    public Task AddAsync(T entity, CancellationToken token = new CancellationToken())
    {
      var items = _items
        .Where(i => !i.Id.Equals(entity.Id))
        .Union(new []{ entity })
        .ToList();

      _items.Clear();

      foreach (var item in items)
      {
        _items.Add(item);
      }

      return Task.CompletedTask;
    }

    public void Insert(T entity)
    {
      _items.Add(entity);
    }

    public Task InsertAsync(T entity, CancellationToken token = new CancellationToken())
    {
      _items.Add(entity);
      return Task.CompletedTask;
    }

    public void Delete(T entity)
    {
      _items.Remove(entity);
    }

    public Task DeleteAsync(T entity, CancellationToken token = new CancellationToken())
    {
      _items.Remove(entity);
      return Task.CompletedTask;
    }
  }
}