using ViennaNET.Orm.Configuration;
using IBM.Data.DB2.Core;

namespace ViennaNET.Orm.DB2
{
  internal static class Db2Utils
  {
    public static string GetConnectionString(this ConnectionInfo info)
    {
      var connectionStringBuilder = new DB2ConnectionStringBuilder { ConnectionString = info.ConnectionString };

      if (!string.IsNullOrWhiteSpace(info.EncPassword))
      {
        connectionStringBuilder.Password = info.EncPassword;
      }
      return connectionStringBuilder.ConnectionString;
    }
  }
}
