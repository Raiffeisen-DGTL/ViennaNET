using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using ViennaNET.Orm.Factories;

namespace ViennaNET.Orm.MSSQL
{
  internal class MsSqlSessionFactoryProvider : SessionFactoryProvider
  {
    public MsSqlSessionFactoryProvider(string nick, IConfiguration configuration, IInterceptor interceptor)
      : base(configuration, nick, interceptor)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.GetConnectionString());
      nhConfig.SetProperty(Environment.Dialect, typeof(MsSqlDialect).AssemblyQualifiedName);
    }

    public override string ServerType => "MSSQL";
  }
}
