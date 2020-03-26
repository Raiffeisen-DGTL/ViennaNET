using ViennaNET.ArcSight.Configuration;
using ViennaNET.ArcSight.Exceptions;
using ViennaNET.Logging;
using SyslogNet.Client.Transport;

namespace ViennaNET.ArcSight
{
  /// <inheritdoc />
  public class CefSenderFactory : ICefSenderFactory
  {
    /// <inheritdoc />
    public ICefSender CreateSender(ArcSightSection cefConfig)
    {
      Logger.LogDebug("Call CreateSender...");
      ISyslogMessageSender syslogSender;
      switch (cefConfig.Protocol)
      {
        case "tcp":
          syslogSender = new SyslogTcpSender(cefConfig.ServerHost, cefConfig.ServerPort);
          break;
        case "udp":
          syslogSender = new SyslogUdpSender(cefConfig.ServerHost, cefConfig.ServerPort);
          break;
        case "local":
          syslogSender = new SyslogLocalSender();
          break;
        default:
          throw new ArcSightConfigurationException($"The syslog protocol {cefConfig.Protocol} has not supported");
      }

      Logger.LogDebug("Call CreateSender done.");
      return new CefSender(syslogSender);
    }
  }
}
