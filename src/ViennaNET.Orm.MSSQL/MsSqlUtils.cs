﻿using Microsoft.Data.SqlClient;
using ViennaNET.Orm.Configuration;

namespace ViennaNET.Orm.MSSQL
{
  internal static class MsSqlUtils
  {
    public static string GetConnectionString(this ConnectionInfo info)
    {
      var connectionStringBuilder = new SqlConnectionStringBuilder { ConnectionString = info.ConnectionString };
      
      return connectionStringBuilder.ConnectionString;
    }
  }
}