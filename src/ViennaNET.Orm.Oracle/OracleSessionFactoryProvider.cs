using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Cfg;
using ViennaNET.Orm.Factories;
using Environment = NHibernate.Cfg.Environment;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ViennaNET.Orm.Oracle
{
  internal class OracleSessionFactoryProvider : SessionFactoryProvider
  {
    public OracleSessionFactoryProvider(string nick, IConfiguration configuration, IInterceptor interceptor,
      ILoggerFactory loggerFactory, ILogger<OracleSessionFactoryProvider> logger)
      : base(configuration, nick, interceptor, loggerFactory, logger)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.ConnectionString);
      nhConfig.SetProperty(Environment.Dialect, typeof(OracleDialect).AssemblyQualifiedName);
    }

    public override string ServerType => "Oracle";
  }
}