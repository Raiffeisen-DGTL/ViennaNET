using NHibernate.Cfg;
using BaseDialect = NHibernate.Dialect.MsSql2012Dialect;

namespace ViennaNET.Orm.MSSQL
{
  /// <inheritdoc />
  public class MsSqlDialect : BaseDialect
  {
    /// <summary>
    /// Инициализирует переменную драйвера подключения
    /// выбранным драйвером
    /// </summary>
    public MsSqlDialect()
    {
      DefaultProperties[Environment.ConnectionDriver] = typeof (MsSqlDriver).AssemblyQualifiedName;
    }
  }
}
