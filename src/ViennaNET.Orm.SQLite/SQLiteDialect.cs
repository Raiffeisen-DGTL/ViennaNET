using NHibernate.Cfg;
using BaseDialect = NHibernate.Dialect.SQLiteDialect;

namespace ViennaNET.Orm.SQLite
{
  public class SQLiteDialect : BaseDialect
  {
    /// <summary>
    /// Инициализирует переменную драйвера подключения
    /// выбранным драйвером
    /// </summary>
    public SQLiteDialect()
    {
      DefaultProperties[Environment.ConnectionDriver] = typeof(SQLiteDriver).AssemblyQualifiedName;
    }
  }
}