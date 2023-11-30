using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Cfg;
using ViennaNET.Orm.Factories;
using Environment = NHibernate.Cfg.Environment;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ViennaNET.Orm.PostgreSql
{
  internal class PostgreSqlSessionFactoryProvider : SessionFactoryProvider
  {
    public PostgreSqlSessionFactoryProvider(string nick, IConfiguration configuration, IInterceptor interceptor,
      ILoggerFactory loggerFactory, ILogger<PostgreSqlSessionFactoryProvider> logger)
      : base(configuration, nick, interceptor, loggerFactory, logger)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.ConnectionString);
      nhConfig.SetProperty(Environment.Dialect, typeof(PostgreSqlDialect).AssemblyQualifiedName);
    }

    public override string ServerType => "PostgreSql";
  }
}