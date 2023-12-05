namespace ViennaNET.ArcSight
{
  /// <summary>
  ///   Уровень критичности сообщения в ArcSight
  /// </summary>
  public enum CefSeverity
  {
    /// <summary>
    ///   System is unusable
    /// </summary>
    Emergency = 10,

    /// <summary>
    ///   Action must be taken immediately
    /// </summary>
    Alert = 9,

    /// <summary>
    ///   Critical conditions
    /// </summary>
    Critical = 8,

    /// <summary>
    ///   Error conditions
    /// </summary>
    Error = 7,

    /// <summary>
    ///   Warning conditions
    /// </summary>
    Warning = 6,

    /// <summary>
    ///   Normal but significant condition
    /// </summary>
    Notice = 5,

    /// <summary>
    ///   Informational messages
    /// </summary>
    Informational = 4,

    /// <summary>
    ///   Debug-level messages
    /// </summary>
    Debug = 3
  }
}