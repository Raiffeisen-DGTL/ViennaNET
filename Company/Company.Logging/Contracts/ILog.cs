namespace Company.Logging.Contracts
{
  /// <summary>
  /// logger interface
  /// </summary>
  public interface ILog
  {
    /// <summary>
    /// log message with specific category, logging level and message
    /// </summary>
    /// <param name="category">log category</param>
    /// <param name="level">logging level</param>
    /// <param name="message">message</param>
    void Log(string category, LogLevel level, string message);

    /// <summary>
    /// merge internal state 
    /// </summary>
    /// <param name="logger">logger internal state to add</param>
    void Merge(ILog logger);

    /// <summary>
    /// flush cached data into the log
    /// </summary>
    void Flush();
  }
}