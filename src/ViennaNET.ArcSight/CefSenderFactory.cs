using Microsoft.Extensions.Logging;
using SyslogNet.Client.Transport;
using ViennaNET.ArcSight.Configuration;
using ViennaNET.ArcSight.Exceptions;

namespace ViennaNET.ArcSight
{
  /// <inheritdoc />
  public class CefSenderFactory : ICefSenderFactory
  {
    private readonly ILogger _logger;

    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="logger">A logger interface</param>
    public CefSenderFactory(ILogger<CefSenderFactory> logger)
    {
      _logger = logger;
    }

    /// <inheritdoc />
    public ICefSender CreateSender(ArcSightSection cefConfig)
    {
      _logger.LogDebug("Call CreateSender...");
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

      _logger.LogDebug("Call CreateSender done.");
      return new CefSender(syslogSender);
    }
  }
}
