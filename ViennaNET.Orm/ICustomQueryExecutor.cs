using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Orm.Repositories;

namespace ViennaNET.Orm.Application
{
  /// <summary>
  /// Позволяет выполнять запросы к БД
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ICustomQueryExecutor<T> where T : class
  {
    /// <summary>
    /// Синхронно выполняет запрос на основе SQL к БД
    /// </summary>
    /// <param name="query">Ссылка на запрос</param>
    /// <returns>Коллекция результатов запроса</returns>
    IEnumerable<T> CustomQuery(BaseQuery<T> query);

    /// <summary>
    /// Асинхронно выполняет запрос на основе SQL к БД
    /// </summary>
    /// <param name="query">Ссылка на запрос</param>
    /// <param name="token">Токен отмены операции</param>
    /// <returns>Коллекция результатов запроса</returns>
    Task<IList<T>> CustomQueryAsync(BaseQuery<T> query, CancellationToken token = default);
  }
}
