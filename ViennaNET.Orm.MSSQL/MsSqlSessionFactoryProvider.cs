using ViennaNET.Orm.Factories;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using ViennaNET.Orm.Configuration;

namespace ViennaNET.Orm.MSSQL
{
  internal class MsSqlSessionFactoryProvider : SessionFactoryProvider
  {
    public MsSqlSessionFactoryProvider(
      string nick, IConfiguration configuration, IInterceptor interceptor)
      : base(configuration, nick, interceptor)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.GetConnectionString());
      nhConfig.SetProperty(Environment.Dialect, typeof(MsSqlDialect).AssemblyQualifiedName);
      nhConfig.SetProperty(Environment.ConnectionProvider, typeof(DriverConnectionProvider).AssemblyQualifiedName);
    }

    public override string ServerType => "MSSQL";

    public override ISessionFactory GetSessionFactory()
    {
      var sessionFactory = base.GetSessionFactory();
      return sessionFactory;
    }
  }
}
