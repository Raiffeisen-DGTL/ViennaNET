using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Orm.Application
{
  /// <summary>
  ///   Позволяет исполнять команды к БД
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ICommandExecutor<in T>
  {
    /// <summary>
    ///   Метод для синхронного выполнения команды <see cref="ICommand" />
    /// </summary>
    /// <param name="command">Ссылка на команду БД</param>
    /// <returns>Количество строк, обновленных или удаленных</returns>
    int Execute(T command);

    /// <summary>
    ///   Метод для асинхронного выполнения команды <see cref="ICommand" />
    /// </summary>
    /// <param name="command">Ссылка на команду БД</param>
    /// <param name="token">Токен отмены операции</param>
    /// <returns>Количество строк, обновленных или удаленных</returns>
    Task<int> ExecuteAsync(T command, CancellationToken token = default);
  }
}