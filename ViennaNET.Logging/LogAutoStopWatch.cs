using System;
using System.Diagnostics;

namespace ViennaNET.Logging
{
  /// <summary>
  ///   auto logging performance of the block bounded by use statement
  /// </summary>
  public class LogAutoStopWatch : IDisposable
  {
    /// <summary>
    ///   description to add to the log
    /// </summary>
    private readonly string _description;

    /// <summary>
    ///   level of logging
    /// </summary>
    private readonly LogLevel _level;

    /// <summary>
    ///   timer
    /// </summary>
    private readonly Stopwatch _stopWatch;

    /// <summary>
    ///   C'tor
    /// </summary>
    /// <param name="descr">description to add to the output</param>
    /// <param name="level">level of logging</param>
    public LogAutoStopWatch(string descr, LogLevel level)
    {
      _stopWatch = new Stopwatch();
      _stopWatch.Start();
      _description = descr;
      _level = level;
      Logger.Log(_level, $"Starting {_description}.");
    }

    /// <summary>
    ///   stop and log information about the timer
    /// </summary>
    public void Dispose()
    {
      _stopWatch.Stop();
      var ts = _stopWatch.Elapsed;
      Logger.Log(_level,
                 string.Format("{4}. Time elapsed: {0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10,
                               _description));
    }

    public static LogAutoStopWatch CreateDebugLog(string className, string methodName, string description = "")
    {
      return new LogAutoStopWatch($"{className}.{methodName} {description}", LogLevel.Debug);
    }

    public static void WriteDebugLog(string className, string methodName, string description, TimeSpan ts)
    {
      var message = $"{className}.{methodName} {description}";
      Logger.Log(LogLevel.Debug,
                 string.Format("{4}. Time elapsed: {0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10,
                               message));
    }
  }
}
