using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Type;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Repositories
{
  /// <inheritdoc />
  public sealed class CommandExecutor<T> : ICommandExecutor<T> where T : class, ICommand
  {
    private readonly ILogger _logger;
    private readonly ISession _session;

    /// <summary>
    ///   Инициализирцет экземпляр ссылкой на <see cref="ISession" />
    /// </summary>
    /// <param name="session">Сессия подключения к БД</param>
    /// <param name="logger">Интерфейс логгирования</param>
    public CommandExecutor(ISession session, ILogger<CommandExecutor<T>> logger)
    {
      _session = session;
      _logger = logger;
    }

    /// <inheritdoc />
    public int Execute(T command)
    {
      var nhQuery = CreateSqlQuery(command);
      return nhQuery.ExecuteUpdate();
    }

    /// <inheritdoc />
    public Task<int> ExecuteAsync(T command, CancellationToken token = default)
    {
      var nhQuery = CreateSqlQuery(command);
      return nhQuery.ExecuteUpdateAsync(token);
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