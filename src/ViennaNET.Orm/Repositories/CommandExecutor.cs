using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Type;
using ViennaNET.Logging;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Repositories
{
  /// <inheritdoc />
  public sealed class CommandExecutor<T> : ICommandExecutor<T> where T : class, ICommand
  {
    private readonly ISession _session;

    /// <summary>
    /// Инициализирцет экземпляр ссылкой на <see cref="ISession"/>
    /// </summary>
    /// <param name="session"></param>
    public CommandExecutor(ISession session)
    {
      _session = session;
    }

    /// <inheritdoc />
    public int Execute(T command)
    {
      using (new LogAutoStopWatch($"Starting command '{command.Sql}'...", LogLevel.Debug))
      {
        var nhQuery = CreateSqlQuery(command);

        return nhQuery.ExecuteUpdate();
      }
    }

    /// <inheritdoc />
    public Task<int> ExecuteAsync(T command, CancellationToken token = default)
    {
      using (new LogAutoStopWatch($"Starting async command '{command.Sql}'...", LogLevel.Debug))
      {
        var nhQuery = CreateSqlQuery(command);

        return nhQuery.ExecuteUpdateAsync(token);
      }
    }

    private ISQLQuery CreateSqlQuery(T command)
    {
      var nhQuery = _session.CreateSQLQuery(command.Sql);

      if (command.Parameters != null && command.Parameters.Any())
      {
        foreach (var parameter in command.Parameters)
        {
          var val = parameter.Value;
          if (val != null)
          {
            nhQuery.SetParameter(parameter.Key, val.BaseValue, TypeFactory.Basic(val.Type.FullName));
          }
        }
      }

      return nhQuery;
    }
  }
}
