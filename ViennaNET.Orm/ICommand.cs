namespace ViennaNET.Orm.Application
{
  /// <summary>
  /// Интерфейс-маркер команды БД. Команда должна содержать
  /// SQL-скрипт для выполнения
  /// </summary>
  public interface ICommand
  {
    /// <summary>
    /// SQL-скрипт для выполнения
    /// </summary>
    string Sql { get; }
  }
}
