using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Cfg;
using ViennaNET.Orm.Factories;
using Environment = NHibernate.Cfg.Environment;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ViennaNET.Orm.DB2
{
  internal class Db2SessionFactoryProvider : SessionFactoryProvider
  {
    public Db2SessionFactoryProvider(
      string nick, IConfiguration configuration, IInterceptor interceptor, ILoggerFactory loggerFactory,
      ILogger<Db2SessionFactoryProvider> logger) : base(configuration, nick, interceptor, loggerFactory, logger)
    {
      nhConfig.SetProperty(Environment.ConnectionString, connectionInfo.GetConnectionString());
      nhConfig.SetProperty(Environment.Dialect, typeof(Db2Dialect).AssemblyQualifiedName);
    }

    public override string ServerType => "DB2";
  }
}