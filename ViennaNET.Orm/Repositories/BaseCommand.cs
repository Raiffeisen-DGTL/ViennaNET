using System.Collections.Generic;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Repositories
{
  /// <inheritdoc />
  public abstract class BaseCommand : ICommand
  {
    protected string Sql;

    /// <summary>
    /// Параметры для записи в формируемую SQL-команду
    /// </summary>
    public IDictionary<string, TypeWrapper> Parameters { get; set; }

    /// <inheritdoc />
    string ICommand.Sql => Sql;
  }
}
