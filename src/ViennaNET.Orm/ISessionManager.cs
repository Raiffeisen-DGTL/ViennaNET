using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm
{
  /// <summary>
  /// Предназначен для управления сессиями NHibernate. Абстрагирует
  /// доступ к сессиям от деталей реализации их хранения
  /// </summary>
  public interface ISessionManager
  {
    /// <summary>
    /// Получение сессии по имени подключения к БД
    /// </summary>
    /// <param name="nick">Имя подключения</param>
    /// <returns>Сессия БД</returns>
    ISession GetSession(string nick);

    /// <summary>
    /// Получение сессии без состояния. Подходит для массовых операций
    /// </summary>
    /// <param name="nick">Имя подключения</param>
    /// <returns>Сессия БД</returns>
    IStatelessSession GetStatelessSession(string nick);


    /// <summary>
    /// Запускает транзакции во всех сохраненных сессиях БД
    /// </summary>
    void StartTransactionAll();

    /// <summary>
    /// Синхронно подтверждает транзакции во всех сохраненных сессиях БД
    /// </summary>
    void CommitAll();

    /// <summary>
    /// Асинхронно подтверждает транзакции во всех сохраненных сессиях БД
    /// </summary>
    IEnumerable<Task> CommitAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Синхронно откатывает транзакции во всех сохраненных сессиях БД
    /// </summary>
    void RollbackAll(bool? existException);

    /// <summary>
    /// Асинхронно откатывает транзакции во всех сохраненных сессиях БД
    /// </summary>
    IEnumerable<Task> RollbackAllAsync(bool? existException, CancellationToken cancellationToken);

    /// <summary>
    /// Закрывает все сохраненные сессии БД
    /// </summary>
    void CloseAll();

    /// <summary>
    /// Синхронно сохраняет изменения во всех сохраненных сессиях БД
    /// </summary>
    void SaveAll();

    /// <summary>
    /// Асинхронно сохраняет изменения во всех сохраненных сессиях БД
    /// </summary>
    IEnumerable<Task> SaveAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Регистрирует единицу работы
    /// </summary>
    bool RegisterUoW(IUnitOfWork uow);

    /// <summary>
    /// Отменяет регистрацию единицы работы
    /// </summary>
    void UnregisterUoW();
  }
}
