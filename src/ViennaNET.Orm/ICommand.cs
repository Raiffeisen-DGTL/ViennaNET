using System.Collections.Generic;
using ViennaNET.Orm.Repositories;

namespace ViennaNET.Orm.Application
{
  /// <summary>
  ///   Интерфейс-маркер команды БД. Команда должна содержать
  ///   SQL-скрипт для выполнения
  /// </summary>
  public interface ICommand
  {
    /// <summary>
    ///   SQL-скрипт для выполнения
    /// </summary>
    string Sql { get; }

    /// <summary>
    ///   Параметры для записи в формируемую SQL-команду
    /// </summary>
    IDictionary<string, TypeWrapper> Parameters { get; }
  }
}