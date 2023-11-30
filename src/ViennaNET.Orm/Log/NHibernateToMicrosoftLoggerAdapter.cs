using System;
using Microsoft.Extensions.Logging;
using NHibernate;
using ViennaNET.Utils;

namespace ViennaNET.Orm.Log
{
  internal class NHibernateToMicrosoftLoggerAdapter : INHibernateLogger
  {
    private readonly ILogger _msLogger;

    public NHibernateToMicrosoftLoggerAdapter(ILogger msLogger)
    {
      _msLogger = msLogger.ThrowIfNull(nameof(msLogger));
    }

    public void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception? exception)
    {
      _msLogger.Log(GetLogLevel(logLevel), 0, state, exception, MessageFormatter);
    }

    public bool IsEnabled(NHibernateLogLevel logLevel)
    {
      return _msLogger.IsEnabled(GetLogLevel(logLevel));
    }

    private static LogLevel GetLogLevel(NHibernateLogLevel logLevel) => logLevel switch
    {
      NHibernateLogLevel.Trace => LogLevel.Trace,
      NHibernateLogLevel.Debug => LogLevel.Debug,
      NHibernateLogLevel.Info => LogLevel.Information,
      NHibernateLogLevel.Warn => LogLevel.Warning,
      NHibernateLogLevel.Error => LogLevel.Error,
      NHibernateLogLevel.Fatal => LogLevel.Critical,
      NHibernateLogLevel.None => LogLevel.None,
      _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
    };

    private static string MessageFormatter(NHibernateLogValues state, Exception? exception) => exception is null
      ? state.ToString()
      : $"{state.ToString()}.{exception}";
  }
}