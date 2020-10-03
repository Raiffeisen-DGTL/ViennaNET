using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using ViennaNET.Orm.Factories;

namespace ViennaNET.Orm.Oracle
{
  internal class OracleSessionFactoryProvider : SessionFactoryProvider
  {
    public OracleSessionFactoryProvider(string nick, IConfiguration configuration, IInterceptor interceptor)
      : base(configuration, nick, interceptor)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.ConnectionString);
      nhConfig.SetProperty(Environment.Dialect, typeof(OracleDialect).AssemblyQualifiedName);
    }

    public override string ServerType => "Oracle";
  }
}