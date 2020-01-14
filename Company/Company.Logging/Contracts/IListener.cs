namespace Company.Logging.Contracts
{
  /// <summary>
  /// custom listener interface
  /// </summary>
  public interface IListener
  {
    /// <summary>
    /// returns validator
    /// </summary>
    /// <returns></returns>
    IListenerValidator GetValidator();

    /// <summary>
    /// log event message
    /// </summary>
    /// <param name="message">message to log</param>
    /// <param name="level">loging level</param>
    void LogMessage(string message, LogLevel level);

    /// <summary>
    /// flush data into the log
    /// </summary>
    void Flush();
  }
}