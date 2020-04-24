using System;
using System.Data;
using ViennaNET.Orm.Repositories;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm.Application
{
  /// <inheritdoc />
  /// <summary>
  /// Интерфейс фабричного сервиса для инфраструктурных операций с БД,
  /// недоступных в доменной модели
  /// </summary>
  public interface IEntityFactoryService : IEntityRepositoryFactory
  {
    /// <summary>
    /// Создает единицу работы
    /// </summary>
    /// <param name="isolationLevel">Уровень изоляции транзакции</param>
    /// <param name="autoControl">Признак изменения состояния уже загруженных сущностей</param>
    /// <param name="closeSessions">Признак закрытия сессий при откате транзакции</param>
    /// <returns>Единица работы</returns>
    IUnitOfWork Create(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool autoControl = true, bool closeSessions = false);

    /// <summary>
    /// Создает пользовательский контекст сессии. Необходимо использовать
    /// при работе с БД в потоках и задачах, порожденных пользователем
    /// и не относящихся к веб-вызовам
    /// </summary>
    /// <returns>Контекст сессии</returns>
    IDisposable GetScopedSession();

    /// <summary>
    /// Получает результат выполнения именованного запроса
    /// </summary>
    /// <typeparam name="T">Тип данных запроса</typeparam>
    /// <param name="namedQuery">Имя запроса</param>
    /// <returns>Результат запроса в соответствии с типом</returns>
    T GetByNameSingle<T>(string namedQuery);

    /// <summary>
    /// Получает результат выполнения именованного запроса
    /// </summary>
    /// <param name="namedQuery">Имя запроса</param>
    /// <returns>Результат запроса</returns>
    object GetByNameSingle(string namedQuery);

    /// <summary>
    /// Создает исполнитель команд 
    /// </summary>
    /// <typeparam name="T">Тип команды<see cref="BaseCommand"/>></typeparam>
    /// <returns>Экземпляр исполнителя команд</returns>
    ICommandExecutor<T> CreateCommandExecutor<T>() where T : BaseCommand;

    /// <summary>
    /// Создает исполнитель запросов
    /// </summary>
    /// <typeparam name="T">Тип запроса</typeparam>
    /// <returns>Экземпляр исполнителя запросов</returns>
    ICustomQueryExecutor<T> CreateCustomQueryExecutor<T>() where T : class;
  }
}
