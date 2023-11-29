using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Cfg;
using ViennaNET.Orm.Factories;
using Environment = NHibernate.Cfg.Environment;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ViennaNET.Orm.MSSQL
{
  internal class MsSqlSessionFactoryProvider : SessionFactoryProvider
  {
    public MsSqlSessionFactoryProvider(
      string nick, IConfiguration configuration, IInterceptor interceptor, ILoggerFactory loggerFactory,
      ILogger<MsSqlSessionFactoryProvider> logger) : base(configuration, nick, interceptor, loggerFactory, logger)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.GetConnectionString());
      nhConfig.SetProperty(Environment.Dialect, typeof(MsSqlDialect).AssemblyQualifiedName);
    }

    public override string ServerType => "MSSQL";
  }
}