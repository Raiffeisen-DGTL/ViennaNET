using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using ViennaNET.Orm.Factories;

namespace ViennaNET.Orm.SQLite
{
  internal class SQLiteSessionFactoryProvider : SessionFactoryProvider
  {
    public SQLiteSessionFactoryProvider(
      string nick, IConfiguration configuration, IInterceptor interceptor)
      : base(configuration, nick, interceptor)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.ConnectionString);
      nhConfig.SetProperty(Environment.Dialect, typeof(SQLiteDialect).AssemblyQualifiedName);
      nhConfig.SetProperty(Environment.ConnectionProvider, typeof(DriverConnectionProvider).AssemblyQualifiedName);
    }

    public override string ServerType => "SQLite";

    public override ISessionFactory GetSessionFactory()
    {
      var sessionFactory = base.GetSessionFactory();
      return sessionFactory;
    }
  }
}