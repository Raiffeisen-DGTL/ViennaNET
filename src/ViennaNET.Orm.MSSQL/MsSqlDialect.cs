using NHibernate.Cfg;
using NHibernate.Driver;
using BaseDialect = NHibernate.Dialect.MsSql2012Dialect;
using Environment = NHibernate.Cfg.Environment;

namespace ViennaNET.Orm.MSSQL
{
  /// <inheritdoc />
  public class MsSqlDialect : BaseDialect
  {
    /// <summary>
    ///   Инициализирует переменную драйвера подключения
    ///   выбранным драйвером
    /// </summary>
    public MsSqlDialect()
    {
      DefaultProperties[Environment.ConnectionDriver] = typeof(MicrosoftDataSqlClientDriver).AssemblyQualifiedName;
    }
  }
}