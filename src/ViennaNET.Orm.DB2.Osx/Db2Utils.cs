using IBM.Data.Db2;
using ViennaNET.Orm.Configuration;

namespace ViennaNET.Orm.DB2
{
  internal static class Db2Utils
  {
    public static string GetConnectionString(this ConnectionInfo info)
    {
      var connectionStringBuilder = new DB2ConnectionStringBuilder { ConnectionString = info.ConnectionString };
      return connectionStringBuilder.ConnectionString;
    }
  }
}