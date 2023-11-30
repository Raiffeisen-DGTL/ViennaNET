using NHibernate.Cfg;
using NHibernate.Driver;
using BaseDialect = NHibernate.Dialect.PostgreSQL83Dialect;
using Environment = NHibernate.Cfg.Environment;

namespace ViennaNET.Orm.PostgreSql
{
  public class PostgreSqlDialect : BaseDialect
  {
    /// <summary>
    ///   Инициализирует переменную драйвера подключения
    ///   выбранным драйвером
    /// </summary>
    public PostgreSqlDialect()
    {
      DefaultProperties[Environment.ConnectionDriver] = typeof(NpgsqlDriver).AssemblyQualifiedName;
    }
  }
}