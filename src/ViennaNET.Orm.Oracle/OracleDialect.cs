using NHibernate.Cfg;
using NHibernate.Driver;
using BaseDialect = NHibernate.Dialect.Oracle10gDialect;
using Environment = NHibernate.Cfg.Environment;

namespace ViennaNET.Orm.Oracle
{
  public class OracleDialect : BaseDialect
  {
    /// <summary>
    ///   Инициализирует переменную драйвера подключения
    ///   выбранным драйвером
    /// </summary>
    public OracleDialect()
    {
      DefaultProperties[Environment.ConnectionDriver] = typeof(OracleManagedDataClientDriver).AssemblyQualifiedName;
    }
  }
}