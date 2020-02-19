using System;
using System.Collections.Generic;
using log4net;
using log4net.Appender;
using log4net;
using log4net.Repository;
using ILog = ViennaNET.Logging.Contracts.ILog;
using log4net.Core;

namespace ViennaNET.Logging.Log4NetImpl
{
  internal class Log4NetWrapper : ILog
  {
    private readonly Dictionary<string, log4net.ILog> _logs;

    public Log4NetWrapper(Dictionary<string, log4net.ILog> logs)
    {
      _logs = logs;
    }

    public void Log(string category, LogLevel level, string message)
    {
      LogInternal(category, level, message);
      if(!string.IsNullOrEmpty(category)) // if category != ALL
      {
        LogInternal(string.Empty, level, message); // log with category ALL
      }
    }

    /// <summary>
    /// merge logger internal states
    /// </summary>
    /// <param name="logger">logger to merge</param>
    public void Merge(ILog logger)
    {
      if (logger == null)
      {
        return;
      }
      var log4netWrapper = logger as Log4NetWrapper;
      if (log4netWrapper == null)
      {
        return;
      }
      foreach (var logEntry in log4netWrapper._logs)
      {
        if (!_logs.ContainsKey(logEntry.Key))
        {
          _logs.Add(logEntry.Key, logEntry.Value);
        }
      }
    }

    public void Flush()
    {
      ILoggerRepository rep = LogManager.GetRepository("Repository");
      foreach (IAppender appender in rep.GetAppenders())
      {
        var buffered = appender as BufferingAppenderSkeleton;
        if (buffered != null)
        {
          buffered.Flush();
        }
      }
    }

    private void LogInternal(string category, LogLevel level, string message)
    {
      log4net.ILog log;
      if (_logs.ContainsKey(category))
      {
        log = _logs[category];
      } else
      {
        return;
      }      
      switch (level)
      {
        case LogLevel.Debug:
          log.Debug(message);
          break;
        case LogLevel.Info:
          log.Info(message);
          break;
        case LogLevel.Error:
          log.Error(message);
          break;
        case LogLevel.Warning:
          log.Warn(message);
          break;
        case LogLevel.Diagnostic:
          log.Logger.Log(null, new Level(30000, "DIAGNOSTIC"), message, null);
          break;
      }
    }
  }
}