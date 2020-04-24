namespace ViennaNET.Orm.Application
{
  /// <summary>
  /// Позволяет исполнять команды к БД
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ICommandExecutor<in T>
  {
    /// <summary>
    /// Метод для выполнения команды <see cref="ICommand"/>
    /// </summary>
    /// <param name="command">Ссылка на команду БД</param>
    void Execute(T command);
  }
}
