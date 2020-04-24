using ViennaNET.Orm.Factories;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using ViennaNET.Orm.Configuration;

namespace ViennaNET.Orm.PostgreSql
{
  internal class PostgreSqlSessionFactoryProvider : SessionFactoryProvider
  {
    public PostgreSqlSessionFactoryProvider(
      string nick, IConfiguration configuration, IInterceptor interceptor)
      : base(configuration, nick, interceptor)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.ConnectionString);
      nhConfig.SetProperty(Environment.Dialect, typeof(PostgreSqlDialect).AssemblyQualifiedName);
      nhConfig.SetProperty(Environment.ConnectionProvider, typeof(DriverConnectionProvider).AssemblyQualifiedName);
    }

    public override string ServerType => "PostgreSql";

    public override ISessionFactory GetSessionFactory()
    {
      var sessionFactory = base.GetSessionFactory();
      return sessionFactory;
    }
  }
}