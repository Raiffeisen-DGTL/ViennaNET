using Environment = NHibernate.Cfg.Environment;
using BaseDialect = NHibernate.Dialect.DB2Dialect;

namespace ViennaNET.Orm.DB2
{
  /// <inheritdoc />
  public class Db2Dialect : BaseDialect
  {
    /// <summary>
    /// Инициализирует переменную драйвера подключения
    /// выбранным драйвером
    /// </summary>
    public Db2Dialect()
    {
      DefaultProperties[Environment.ConnectionDriver] = typeof (Db2Driver).AssemblyQualifiedName;
    }
  }
}
