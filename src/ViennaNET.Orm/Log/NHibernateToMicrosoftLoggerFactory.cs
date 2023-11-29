using System;
using NHibernate;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ViennaNET.Orm.Log
{
  internal class NHibernateToMicrosoftLoggerFactory : INHibernateLoggerFactory
  {
    private readonly ILoggerFactory _loggerFactory;

    public NHibernateToMicrosoftLoggerFactory(ILoggerFactory loggerFactory)
    {
      _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public INHibernateLogger LoggerFor(string keyName)
    {
      var msLogger = _loggerFactory.CreateLogger(keyName);
      return new NHibernateToMicrosoftLoggerAdapter(msLogger);
    }

    public INHibernateLogger LoggerFor(Type type)
    {
      return LoggerFor(type.FullName);
    }
  }
}