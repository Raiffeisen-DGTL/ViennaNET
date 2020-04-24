using NHibernate.Cfg;
using BaseDialect = NHibernate.Dialect.PostgreSQLDialect;

namespace ViennaNET.Orm.PostgreSql
{
  public class PostgreSqlDialect : BaseDialect
  {
    /// <summary>
    /// Инициализирует переменную драйвера подключения
    /// выбранным драйвером
    /// </summary>
    public PostgreSqlDialect()
    {
      DefaultProperties[Environment.ConnectionDriver] = typeof(PostgreSqlDriver).AssemblyQualifiedName;
    }
  }
}