using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Orm.Seedwork
{
  /// <summary>
  /// Интерфейс для репозитория, работающего с абстрактной коллекцией объектов
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public interface IEntityRepository<TEntity> where TEntity : class
  {
    /// <summary>
    /// Возвращает реализацию интерфейса IQueryable 
    /// для создания пользовательских LINQ-запросов 
    /// </summary>
    IQueryable<TEntity> Query();

    /// <summary>
    /// Синхронно получает сущность по идентификатору nullable-типа
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    TEntity Get<TKey>(TKey? id) where TKey : struct;

    /// <summary>
    /// Асинхронно получает сущность по идентификатору nullable-типа
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="token">Токен отмены операции</param>
    Task<TEntity> GetAsync<TKey>(TKey? id, CancellationToken token = default) where TKey : struct;

    /// <summary>
    /// Синхронно получает сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    TEntity Get<TKey>(TKey id);

    /// <summary>
    /// Асинхронно получает сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="token">Токен отмены операции</param>
    Task<TEntity> GetAsync<TKey>(TKey id, CancellationToken token = default);

    /// <summary>
    /// Синхронно добавляет сущность в абстрактную коллекцию. Если сущность
    /// уже есть в коллеции, обновляет ее данные на основе новой сущности
    /// </summary>
    /// <param name="entity">Ссылка на сущность</param>
    void Add(TEntity entity);

    /// <summary>
    /// Асинхронно добавляет сущность в абстрактную коллекцию. Если сущность
    /// уже есть в коллеции, обновляет ее данные на основе новой сущности
    /// </summary>
    /// <param name="entity">Ссылка на сущность</param>
    /// <param name="token">Токен отмены операции</param>
    Task AddAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    /// Синхронно добавляет сущность в абстрактную коллекцию. Если сущность
    /// уже есть в коллеции, вызывается исключение
    /// </summary>
    /// <param name="entity">Ссылка на сущность</param>
    void Insert(TEntity entity);

    /// <summary>
    /// Синхронно добавляет сущность в абстрактную коллекцию. Если сущность
    /// уже есть в коллеции, вызывается исключение
    /// </summary>
    /// <param name="entity">Ссылка на сущность</param>
    /// <param name="token">Токен отмены операции</param>
    Task InsertAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    /// Синхронно удаляет сущность из абстрактной коллекции
    /// </summary>
    /// <param name="entity">Ссылка на сущность</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Асинхронно удаляет сущность из абстрактной коллекции
    /// </summary>
    /// <param name="entity">Ссылка на сущность</param>
    /// <param name="token">Токен отмены операции</param>
    Task DeleteAsync(TEntity entity, CancellationToken token = default);
  }
}
