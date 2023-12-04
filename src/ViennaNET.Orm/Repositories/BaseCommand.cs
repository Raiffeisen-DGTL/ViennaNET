using System.Collections.Generic;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Repositories
{
  /// <inheritdoc />
  public abstract class BaseCommand : ICommand
  {
    protected Dictionary<string, TypeWrapper> Parameters = new();
    protected string Sql;

    /// <inheritdoc />
    IDictionary<string, TypeWrapper> ICommand.Parameters => Parameters;

    /// <inheritdoc />
    string ICommand.Sql => Sql;
  }
}