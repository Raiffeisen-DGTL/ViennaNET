using NHibernate.Cfg;
using NHibernate.Driver;
using BaseDialect = NHibernate.Dialect.SQLiteDialect;
using Environment = NHibernate.Cfg.Environment;

namespace ViennaNET.Orm.SQLite
{
  public class SQLiteDialect : BaseDialect
  {
    /// <summary>
    ///   Инициализирует переменную драйвера подключения
    ///   выбранным драйвером
    /// </summary>
    public SQLiteDialect()
    {
      DefaultProperties[Environment.ConnectionDriver] = typeof(SQLite20Driver).AssemblyQualifiedName;
    }
  }
}