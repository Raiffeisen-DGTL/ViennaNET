using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using ViennaNET.Orm.Factories;

namespace ViennaNET.Orm.DB2
{
  internal class Db2SessionFactoryProvider : SessionFactoryProvider
  {
    public Db2SessionFactoryProvider(string nick, IConfiguration configuration, IInterceptor interceptor)
      : base(configuration, nick, interceptor)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.GetConnectionString());
      nhConfig.SetProperty(Environment.Dialect, typeof(Db2Dialect).AssemblyQualifiedName);
    }

    public override string ServerType => "DB2";
  }
}