using System.Linq;
using ViennaNET.Logging;
using NHibernate;
using NHibernate.Type;
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
    public void Execute(T command)
    {
      using (new LogAutoStopWatch($"Starting command '{command.Sql}'...", LogLevel.Debug))
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

        nhQuery.ExecuteUpdate();
      }
    }
  }
}
