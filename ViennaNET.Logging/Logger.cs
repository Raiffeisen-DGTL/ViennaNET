using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ViennaNET.Logging.Configuration;
using ViennaNET.Logging.Contracts;
using ViennaNET.Logging.Log4NetImpl;
using log4net;
using ILog = ViennaNET.Logging.Contracts.ILog;

namespace ViennaNET.Logging
{
  /// <summary>
  ///   main wrapper class for working with the logger
  /// </summary>
  public static class Logger
  {
    private static string Category
    {
      get
      {
        try
        {
          if (categoryLoggers.IsValueCreated && categoryLoggers.Value.Count > 0)
          {
            return categoryLoggers.Value.Peek();
          }
        }
        catch
        {
        }
        return string.Empty;
      }
    }

    public static string User
    {
      set { LogicalThreadContext.Properties[userParameter] = value; }
    }

    public static string RequestId
    {
      set { LogicalThreadContext.Properties[requestIdParameter] = value; }
    }

    public static string Service
    {
      set { LogicalThreadContext.Properties[serviceParameter] = value; }
    }

    public static string DefaultService
    {
      set { GlobalContext.Properties[serviceParameter] = value; }
    }

    /// <summary>
    ///   log message to all categories
    /// </summary>
    /// <param name="level">logging level</param>
    /// <param name="message">message to log</param>
    public static void Log(LogLevel level, string message)
    {
      loggerImpl.Log(Category, level, message);
    }

    /// <summary>
    ///   log message to the specific category
    /// </summary>
    /// <param name="category">log category</param>
    /// <param name="level">logging level</param>
    /// <param name="message">message to log</param>
    public static void Log(string category, LogLevel level, string message)
    {
      loggerImpl.Log(category, level, message);
    }

    /// <summary>
    ///   log message to the specific category with debugging log level
    /// </summary>
    /// <param name="category">log category</param>
    /// <param name="message">message to log</param>
    public static void LogDebug(string category, string message)
    {
      loggerImpl.Log(category, LogLevel.Debug, message);
    }

    /// <summary>
    ///   log message to the specific category with information log level
    /// </summary>
    /// <param name="category">log category</param>
    /// <param name="message">message to log</param>
    public static void LogInfo(string category, string message)
    {
      loggerImpl.Log(category, LogLevel.Info, message);
    }

    /// <summary>
    ///   log message to the specific category with warning log level
    /// </summary>
    /// <param name="category">log category</param>
    /// <param name="message">message to log</param>
    public static void LogWarning(string category, string message)
    {
      loggerImpl.Log(category, LogLevel.Warning, message);
    }

    /// <summary>
    ///   log message to the specific category with error log level
    /// </summary>
    /// <param name="category">log category</param>
    /// <param name="message">message to log</param>
    public static void LogError(string category, string message)
    {
      loggerImpl.Log(category, LogLevel.Error, message);
    }

    /// <summary>
    ///   log message to all categories with debugging log level
    /// </summary>
    /// <param name="message">message to log</param>
    public static void LogDebug(string message)
    {
      loggerImpl.Log(Category, LogLevel.Debug, message);
    }

    /// <summary>
    ///   log message to all categories with information log level
    /// </summary>
    /// <param name="message">message to log</param>
    public static void LogInfo(string message)
    {
      loggerImpl.Log(Category, LogLevel.Info, message);
    }

    /// <summary>
    ///   log message to all categories with warning log level
    /// </summary>
    /// <param name="message">message to log</param>
    public static void LogWarning(string message)
    {
      loggerImpl.Log(Category, LogLevel.Warning, message);
    }

    /// <summary>
    ///   log message to all categories with error log level
    /// </summary>
    /// <param name="message">message to log</param>
    public static void LogError(string message)
    {
      loggerImpl.Log(Category, LogLevel.Error, message);
    }

    /// <summary>
    ///   log message to all categories with debugging log level and formatted string to log
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public static void LogDebugFormat(string message, params object[] formatParams)
    {
      loggerImpl.Log(Category, LogLevel.Debug, string.Format(message, formatParams));
    }

    /// <summary>
    ///   log message to all categories with information log level and formatted string to log
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public static void LogInfoFormat(string message, params object[] formatParams)
    {
      loggerImpl.Log(Category, LogLevel.Info, string.Format(message, formatParams));
    }

    /// <summary>
    ///   log message to all categories with warning log level and formatted string to log
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public static void LogWarningFormat(string message, params object[] formatParams)
    {
      loggerImpl.Log(Category, LogLevel.Warning, string.Format(message, formatParams));
    }

    /// <summary>
    ///   log message to all categories with error log level and formatted string to log
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public static void LogErrorFormat(string message, params object[] formatParams)
    {
      loggerImpl.Log(Category, LogLevel.Error, string.Format(message, formatParams));
    }

    /// <summary>
    ///   log message to all categories with warning log level formatted string to log.
    ///   This function also appending exception.ToString at the end of the message
    /// </summary>
    /// <param name="exception">exception to log</param>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public static void LogWarningFormat(Exception exception, string message, params object[] formatParams)
    {
      loggerImpl.Log(Category, LogLevel.Warning,
                     string.Concat(string.Format(message, formatParams), Environment.NewLine, exception.ToString()));
    }

    /// <summary>
    ///   log message to all categories with error log level formatted string to log.
    ///   This function also appending exception.ToString at the end of the message
    /// </summary>
    /// <param name="exception">exception to log</param>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public static void LogErrorFormat(Exception exception, string message, params object[] formatParams)
    {
      loggerImpl.Log(Category, LogLevel.Error,
                     string.Concat(string.Format(message, formatParams), Environment.NewLine, exception.ToString()));
    }

    /// <summary>
    ///   log message to all categories with error log level formatted string to log.
    ///   This function also appending exception.ToString at the end of the message
    /// </summary>
    /// <param name="exception">exception to log</param>
    /// <param name="message">formatted message to log</param>
    public static void LogError(Exception exception, string message)
    {
      loggerImpl.Log(Category, LogLevel.Error, string.Concat(message, Environment.NewLine, exception.ToString()));
    }

    public static void LogDiagnostic(string message)
    {
      loggerImpl.Log(Category, LogLevel.Diagnostic, message);
    }

    /// <summary>
    ///   flush information into the log
    /// </summary>
    public static void Flush()
    {
      loggerImpl.Flush();
    }

    /// <summary>
    ///   Gets an instanse of category logger.
    /// </summary>
    /// <param name="category">Log category.</param>
    public static ICategoryLogger GetCategoryLogger(string category)
    {
      return new CategoryLogger(category);
    }

    internal static void RedirectOn(string category)
    {
      if (category == null)
      {
        return;
      }
      categoryLoggers.Value.Push(category);
    }

    internal static void RedirectOff()
    {
      if (!categoryLoggers.IsValueCreated || categoryLoggers.Value.Count == 0)
      {
        return;
      }
      categoryLoggers.Value.Pop();
      Flush();
    }

    public static void ClearUser()
    {
      if (LogicalThreadContext.Properties[userParameter] != null)
      {
        LogicalThreadContext.Properties.Remove(userParameter);
      }
    }

    public static void ClearRequestId()
    {
      if (LogicalThreadContext.Properties[requestIdParameter] != null)
      {
        LogicalThreadContext.Properties.Remove(requestIdParameter);
      }
    }

    public static void ClearService()
    {
      if (LogicalThreadContext.Properties[serviceParameter] != null)
      {
        LogicalThreadContext.Properties.Remove(serviceParameter);
      }
    }

    public static string NewRequestId()
    {
      var reqId = new StringBuilder();
      for (var i = 0; i < 7; i++)
      {
        var c = requestDic[rnd.Next(requestDic.Length)];
        reqId.Append(c);
      }

      return reqId.ToString();
    }

    #region private variables

    /// <summary>
    ///   log implementation for logging - by default empty implementation to avoid NullReferenceException
    /// </summary>
    private static ILog loggerImpl = new EmptyLoggerImpl();

    /// <summary>
    /// </summary>
    private static readonly ThreadLocal<Stack<string>> categoryLoggers = new ThreadLocal<Stack<string>>(() => new Stack<string>());

    private const string userParameter = "user";
    private const string requestIdParameter = "request_id";
    private const string serviceParameter = "service";

    private static readonly Random rnd = new Random();
    private static readonly string requestDic = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    #endregion

    #region Initialization

    /// <summary>
    ///   static C'tor for default implemenation
    /// </summary>
    static Logger()
    {
      ConfigureDefaultLogger();

      GlobalContext.Properties[userParameter] = Environment.UserName;
      GlobalContext.Properties[requestIdParameter] = "no_id";
      GlobalContext.Properties[serviceParameter] = "no_data";
    }

    /// <summary>
    ///   configure default logger and log exception
    /// </summary>
    /// <param name="ex"></param>
    private static void ConfigureDefaultLogger()
    {
      var configuration = new FluentConfiguration().AddTextListener()
                                                   .SetTextFileAppendMode(0, true)
                                                   .SetMinLevel(0, LogLevel.Error)
                                                   .SetTextFileName(0, "application.log");
      Configure(configuration);
    }

    /// <summary>
    ///   re-configure logger by specifing logger factory configurarion
    /// </summary>
    /// <param name="configuration">log configuration factory object</param>
    public static void Configure(ILogConfiguration configuration)
    {
      var newLoggerImpl = configuration.BuildLogger();
      if (newLoggerImpl != null)
      {
        Interlocked.Exchange(ref loggerImpl, newLoggerImpl);
      }
    }

    /// <summary>
    ///   merge configuration with the current
    /// </summary>
    /// <param name="configuration">configuration to merge</param>
    public static void Merge(ILogConfiguration configuration)
    {
      var newLoggerImpl = configuration.BuildLogger();
      if (newLoggerImpl != null)
      {
        newLoggerImpl.Merge(loggerImpl);
        Interlocked.Exchange(ref loggerImpl, newLoggerImpl);
      }
    }

    /// <summary>
    ///   init logger facade by specific logger implementation (only for unit-testing)
    /// </summary>
    /// <param name="logger">logger instance</param>
    static public void Configure(ILog logger)
    {
      Interlocked.Exchange(ref loggerImpl, logger);
    }

    #endregion
  }
}
