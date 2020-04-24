using NHibernate.Cfg;
using BaseDialect = NHibernate.Dialect.Oracle10gDialect;

namespace ViennaNET.Orm.Oracle
{
  public class OracleDialect : BaseDialect
  {
    /// <summary>
    /// Инициализирует переменную драйвера подключения
    /// выбранным драйвером
    /// </summary>
    public OracleDialect()
    {
      DefaultProperties[Environment.ConnectionDriver] = typeof(OracleDriver).AssemblyQualifiedName;
    }
  }
}