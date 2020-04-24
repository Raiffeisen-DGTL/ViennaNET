using System;
using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Orm.Application
{
  /// <summary>
  /// Абстрактная единица работы
  /// </summary>
  public interface IUnitOfWork : IDisposable
  {
    /// <summary>
    /// Синхронно сохраняет измененные данные и закрывает текущую транзакцию
    /// </summary>
    void Commit();

    /// <summary>
    /// Асинхронно сохраняет измененные данные и закрывает текущую транзакцию
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Синхронно сохраняет измененные данные
    /// </summary>
    void Save();

    /// <summary>
    /// Асинхронно сохраняет измененные данные
    /// </summary>
    Task SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Синхронно отменяет изменения данных и откатывает транзакцию
    /// </summary>
    void Rollback();

    /// <summary>
    /// Асинхронно отменяет изменения данных и откатывает транзакцию
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Синхронно помечает сущность как содержащую изменения. При сохранении
    /// состояние сущности будет сохранено в БД
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    /// <param name="entity">Ссылка на сущность</param>
    void MarkDirty<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Синхронно помечает сущность как удаленную.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    /// <param name="entity">Ссылка на сущность</param>
    void MarkDeleted<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Синхронно помечает сущность как новую.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    /// <param name="entity">Ссылка на сущность</param>
    void MarkNew<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Асинхронно помечает сущность как содержащую изменения. При сохранении
    /// состояние сущности будет сохранено в БД
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    /// <param name="entity">Ссылка на сущность</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task MarkDirtyAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;

    /// <summary>
    /// Асинхронно помечает сущность как удаленную.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    /// <param name="entity">Ссылка на сущность</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task MarkDeletedAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;

    /// <summary>
    /// Асинхронно помечает сущность как новую.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    /// <param name="entity">Ссылка на сущность</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task MarkNewAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
  }
}
