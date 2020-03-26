using SyslogNet.Client;

namespace ViennaNET.ArcSight
{
  internal static class CefSeverityExtensions
  {
    public static Severity ToSyslogSeverity(this CefSeverity severity)
    {
      return (Severity)(10 - (int)severity);
    }
  }
}