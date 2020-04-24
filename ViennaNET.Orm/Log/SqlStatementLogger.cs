using System.Data.Common;
using ViennaNET.Logging;
using NHibernate.AdoNet.Util;
using BaseSqlStatementLogger = NHibernate.AdoNet.Util.SqlStatementLogger;

namespace ViennaNET.Orm.Log
{
  /// <inheritdoc />
  /// <summary>
  /// Логирует запрос, сформированный NHibernate, в стандартный <see cref="Logger"/>
  /// </summary>
  public class SqlStatementLogger : BaseSqlStatementLogger
  {
    public SqlStatementLogger(bool logToStdout, bool formatSql): base(logToStdout, formatSql)
    {
    }

    /// <inheritdoc />
    /// <summary> Логирует IDbCommand. </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="command">Текст SQL</param>
    /// <param name="style">Стиль форматирования</param>
    public override void LogCommand(string message, DbCommand command, FormatStyle style)
    {
      if (string.IsNullOrEmpty(command.CommandText))
      {
        return;
      }

      style = DetermineActualStyle(style);
      var statement = style.Formatter.Format(GetCommandLineWithParameters(command));
      Logger.LogDebug(string.IsNullOrEmpty(message) ? statement : message + statement);
    }
  }
}
