using NHibernate.Driver;

namespace ViennaNET.Orm.DB2.NHibernateDriver
{
  /// <summary>
  /// A NHibernate Driver for using the Net5.IBM.Data.Db2 DataProvider.
  /// We have to use it because of change of root namespace in DB2 library for NET5
  /// </summary>
  public class NetDB2Driver : DB2DriverBase
  {
    /// <summary>
    /// C'tor which describe root namespace from a db2 library Net5.IBM.Data.Db2(-osx, -lnx)
    /// </summary>
    public NetDB2Driver() : base("IBM.Data.Db2")
    {
    }
  }
}