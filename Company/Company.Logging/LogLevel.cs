namespace Company.Logging
{
  /// <summary>
  /// loging levels enum
  /// </summary>
  public enum LogLevel : byte
  {
    None = 0,
    Debug = 1,
    Info = 2,
    Warning = 4,
    Error = 8,
    Diagnostic = 16
  }
}