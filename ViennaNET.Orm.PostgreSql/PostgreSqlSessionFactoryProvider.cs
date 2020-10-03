using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using ViennaNET.Orm.Factories;

namespace ViennaNET.Orm.PostgreSql
{
  internal class PostgreSqlSessionFactoryProvider : SessionFactoryProvider
  {
    public PostgreSqlSessionFactoryProvider(string nick, IConfiguration configuration, IInterceptor interceptor)
      : base(configuration, nick, interceptor)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.ConnectionString);
      nhConfig.SetProperty(Environment.Dialect, typeof(PostgreSqlDialect).AssemblyQualifiedName);
    }

    public override string ServerType => "PostgreSql";
  }
}