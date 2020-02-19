#region usings

using System;
using System.IO;
using log4net.Appender;
using log4net.Core;
using ViennaNET.Logging.Contracts;

#endregion

namespace ViennaNET.Logging.Log4NetImpl
{
  /// <summary>
  /// custom appender for using with the log4net (with buffer posibility)
  /// </summary>
  public class CustomAppender : BufferingAppenderSkeleton
  {
    /// <summary>
    /// custom listener
    /// </summary>
    private readonly IListener _listener;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="listener">custom listener </param>
    public CustomAppender(IListener listener)
    {
      if (listener == null)
      {
        throw new ArgumentNullException("listener");
      }
      _listener = listener;
    }

    /// <summary>
    /// append message to the listener
    /// </summary>
    /// <param name="loggingEvent">logging event</param>
    protected override void Append(LoggingEvent loggingEvent)
    {
      var logLevel = LogLevel.Debug;
      switch (loggingEvent.Level.Name)
      {
        case "DEBUG":
          logLevel = LogLevel.Debug;
          break;
        case "INFO":
          logLevel = LogLevel.Info;
          break;
        case "ERROR":
          logLevel = LogLevel.Error;
          break;
        case "WARN":
          logLevel = LogLevel.Warning;
          break;
      }
      var textWriter = new StringWriter();
      RenderLoggingEvent(textWriter, loggingEvent);
      _listener.LogMessage(textWriter.ToString(), logLevel);
    }

    /// <summary>
    /// send data into the log
    /// </summary>
    /// <param name="events">events to process</param>
    protected override void SendBuffer(LoggingEvent[] events)
    {
      foreach (var loggingEvent in events)
      {
        Append(loggingEvent);
      }
      _listener.Flush();
    }

    /// <summary>
    /// override flush function
    /// </summary>
    public override void Flush()
    {
      base.Flush();
      _listener.Flush();
    }

    /// <summary>
    /// override flush function
    /// </summary>
    /// <param name="flushLossyBuffer"></param>
    public override void Flush(bool flushLossyBuffer)
    {
      base.Flush(flushLossyBuffer);
      _listener.Flush();
    }
  }
}