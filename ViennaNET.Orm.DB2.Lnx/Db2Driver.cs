using System.Data.Common;
using NHibernate.AdoNet.Util;
using NHibernate.Driver;
using SqlStatementLogger = ViennaNET.Orm.Log.SqlStatementLogger;

namespace ViennaNET.Orm.DB2
{
  /// <summary>
  /// Позволяет логировать SQL-запросы перед запуском команды в БД
  /// </summary>
  public class Db2Driver : DB2CoreDriver
  {
    private readonly SqlStatementLogger _statementLogger = new SqlStatementLogger(false, true);

    protected override void OnBeforePrepare(DbCommand command)
    {
      _statementLogger.LogCommand(command, FormatStyle.Basic);
      base.OnBeforePrepare(command);
    }
  }
}
