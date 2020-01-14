using System;
using Company.Logging.Contracts;

namespace Company.Logging.Log4NetImpl
{
  /// <summary>
  /// An implementation of category logger
  /// </summary>
  public class CategoryLogger : ICategoryLogger
  {
    private readonly string _category;
    
    internal CategoryLogger(string category)
    {
      _category = category;    
    }

    /// <summary>
    /// log message.
    /// </summary>
    /// <param name="level">logging level</param>
    /// <param name="message">message to log</param>
    public void Log(LogLevel level, string message)
    {
      Logger.Log(_category, level, message);
    }

    /// <summary>
    /// log message with debugging log level.
    /// </summary>
    /// <param name="message">message to log</param>
    public void LogDebug(string message)
    {
      Logger.Log(_category, LogLevel.Debug, message);
    }

    /// <summary>
    /// log message with information log level.
    /// </summary>
    /// <param name="message">message to log</param>
    public void LogInfo(string message)
    {
      Logger.Log(_category, LogLevel.Info, message);
    }

    /// <summary>
    /// log message with warning log level.
    /// </summary>
    /// <param name="message">message to log</param>
    public void LogWarning(string message)
    {
      Logger.Log(_category, LogLevel.Warning, message);
    }

    /// <summary>
    /// log message with error log level.
    /// </summary>
    /// <param name="message">message to log</param>
    public void LogError(string message)
    {
      Logger.Log(_category, LogLevel.Error, message);
    }

    /// <summary>
    /// log message with debugging log level and formatted string to log.
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public void LogDebugFormat(string message, params object[] formatParams)
    {
      Logger.Log(_category, LogLevel.Debug, string.Format(message, formatParams));
    }

    /// <summary>
    /// log message with information log level and formatted string to log.
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public void LogInfoFormat(string message, params object[] formatParams)
    {
      Logger.Log(_category, LogLevel.Info, string.Format(message, formatParams));
    }

    /// <summary>
    /// log message with warning log level and formatted string to log.
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public void LogWarningFormat(string message, params object[] formatParams)
    {
      Logger.Log(_category, LogLevel.Warning, string.Format(message, formatParams));
    }

    /// <summary>
    /// log message with error log level and formatted string to log
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public void LogErrorFormat(string message, params object[] formatParams)
    {
      Logger.Log(_category, LogLevel.Error, string.Format(message, formatParams));
    }

    /// <summary>
    /// log message with warning log level formatted string to log. 
    /// This function also appending exception.ToString at the end of the message
    /// </summary>
    /// <param name="exception">exception to log</param>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public void LogWarningFormat(Exception exception, string message, params object[] formatParams)
    {
      Logger.Log(
        _category, 
        LogLevel.Warning, 
        string.Concat(
          string.Format(message, formatParams),
          Environment.NewLine,
          exception.ToString()
        )
      );
    }

    /// <summary>
    /// log message with error log level formatted string to log. 
    /// This function also appending exception.ToString at the end of the message
    /// </summary>
    /// <param name="exception">exception to log</param>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    public void LogErrorFormat(Exception exception, string message, params object[] formatParams)
    {
      Log(LogLevel.Error,
        string.Concat(
          string.Format(message, formatParams),
          Environment.NewLine,
          exception.ToString()
          )
        );
    }

    /// <summary>
    /// flush information into the log
    /// </summary>
    public void Flush()
    {
      Logger.Flush();
    }

    public string Category { get { return _category; } }
  }
}
