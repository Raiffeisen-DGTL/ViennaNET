using System;

namespace Company.Logging.Contracts
{
  /// <summary>
  /// Category logger instanse.<br/>
  /// All methods writes logs with specific category, defined in constructor.
  /// </summary>
  public interface ICategoryLogger
  {
    /// <summary>
    /// log message.
    /// </summary>
    /// <param name="level">logging level</param>
    /// <param name="message">message to log</param>
    void Log(LogLevel level, string message);

    /// <summary>
    /// log message with debugging log level.
    /// </summary>
    /// <param name="message">message to log</param>
    void LogDebug(string message);

    /// <summary>
    /// log message with information log level.
    /// </summary>
    /// <param name="message">message to log</param>
    void LogInfo(string message);

    /// <summary>
    /// log message with warning log level.
    /// </summary>
    /// <param name="message">message to log</param>
    void LogWarning(string message);

    /// <summary>
    /// log message with error log level.
    /// </summary>
    /// <param name="message">message to log</param>
    void LogError(string message);

    /// <summary>
    /// log message with debugging log level and formatted string to log.
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    void LogDebugFormat(string message, params object[] formatParams);

    /// <summary>
    /// log message with information log level and formatted string to log.
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    void LogInfoFormat(string message, params object[] formatParams);

    /// <summary>
    /// log message with warning log level and formatted string to log.
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    void LogWarningFormat(string message, params object[] formatParams);

    /// <summary>
    /// log message with error log level and formatted string to log
    /// </summary>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    void LogErrorFormat(string message, params object[] formatParams);

    /// <summary>
    /// log message with warning log level formatted string to log. 
    /// This function also appending exception.ToString at the end of the message
    /// </summary>
    /// <param name="exception">exception to log</param>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    void LogWarningFormat(Exception exception, string message, params object[] formatParams);

    /// <summary>
    /// log message with error log level formatted string to log. 
    /// This function also appending exception.ToString at the end of the message
    /// </summary>
    /// <param name="exception">exception to log</param>
    /// <param name="message">formatted message to log</param>
    /// <param name="formatParams">parameters to format the message</param>
    void LogErrorFormat(Exception exception, string message, params object[] formatParams);

    /// <summary>
    /// flush information into the log
    /// </summary>
    void Flush();

    /// <summary>
    /// category name
    /// </summary>
    string Category { get; }
  }
}
