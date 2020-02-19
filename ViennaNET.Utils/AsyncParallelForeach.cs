using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.Utils
{
  /// <summary>
  /// Асинхронный параллельный ForEach
  /// </summary>
  public static class AsyncParallelForeach
  {
    /// <summary>
    /// Метод, выполняющий асинхронный параллельный Select
    /// </summary>
    /// <param name="source">Массив исходных данных</param>
    /// <param name="action">Метод для выполнения по параметрам <typeparamref name="V"/>, возвращающий Task <typeparamref name="T"/></param>
    /// <param name="threadsCount">Количество потоков</param>
    /// <returns>List объектов типа <typeparamref name="T"/></returns>
    public static async Task<List<T>> SelectAsync<T, V>(IEnumerable<V> source, Func<V, Task<T>> action, int threadsCount)
    {
      var result = new ConcurrentBag<T>();
      var tasks = new ConcurrentQueue<Task<T>>(source.Select(d => action(d)));
      var threads = Enumerable.Range(1, threadsCount)
                             .Select(async p =>
                             {
                               while (tasks.TryDequeue(out var item))
                               {
                                 if (item.IsCompleted)
                                 {
                                   result.Add(item.Result);
                                 }
                                 else
                                 {
                                   result.Add(await item);
                                 }
                               }
                             });
      await Task.WhenAll(threads);
      return result.ToList();
    }
    /// <summary>
    /// Метод, выполняющий асинхронный параллельный Foreach
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Массив исходных данных для запроса</param>
    /// <param name="action">Метод для выполнения запроса по параметрам <typeparamref name="T"/>, возвращающий Task</param>
    /// <param name="threadsCount">Количество потоков</param>
    /// <returns></returns>
    public static async Task ForEachAsync<T>(IEnumerable<T> source, Func<T, Task> action, int threadsCount)
    {
      var tasks = new ConcurrentQueue<Task>(source.Select(d => action(d)));
      var threads = Enumerable.Range(1, threadsCount)
                             .Select(async p =>
                             {
                               while (tasks.TryDequeue(out var item))
                               {
                                 await item;
                               }
                             });
      await Task.WhenAll(threads);
    }
  }
}
