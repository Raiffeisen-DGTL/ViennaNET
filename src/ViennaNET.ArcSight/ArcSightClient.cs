using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using SyslogNet.Client.Serialization;
using ViennaNET.ArcSight.Configuration;
using ViennaNET.ArcSight.Exceptions;
using ViennaNET.Utils;

namespace ViennaNET.ArcSight
{
  /// <inheritdoc />
  public class ArcSightClient : IArcSightClient
  {
    private readonly ArcSightSection _cefConfig;
    private readonly ICefSenderFactory _cefSenderFactory;
    private readonly ILogger _logger;
    private readonly ISyncPolicy _policy;
    private readonly CefMessageSerializer _serializer;

    /// <summary>
    ///   Инициализирует экземпляр <see cref="ArcSightClient" /> ссылками на экземпляры <see cref="IConfiguration" />,
    ///   <see cref="IErrorHandlingPoliciesFactory" />, <see cref="ICefSenderFactory" />.
    /// </summary>
    /// <param name="configuration">Ссылка на интерфейс, предоставляющий доступ к конфигурации</param>
    /// <param name="policiesFactory">Ссылка на интерфейс фабрики по созданию политик вызовов</param>
    /// <param name="cefSenderFactory">Ссылка на интерфейс фабрики по созданию классов для доступа к каналу отправки</param>
    /// <param name="logger">Logger interface</param>
    public ArcSightClient(IConfiguration configuration, IErrorHandlingPoliciesFactory policiesFactory,
      ICefSenderFactory cefSenderFactory, ILogger<ArcSightClient> logger)
    {
      _logger = logger;
      _cefConfig = configuration.ThrowIfNull(nameof(configuration))
        .GetSection("arcSight")
        .Get<ArcSightSection>()
        .ThrowIfNull(new InvalidOperationException("Can not find CEF configuration"));

      _policy = policiesFactory.ThrowIfNull(nameof(policiesFactory))
        .CreateStdCommunicationPolicy();

      _cefSenderFactory = cefSenderFactory.ThrowIfNull(nameof(cefSenderFactory));

      _serializer = CreateSerializer();
    }

    /// <inheritdoc />
    public void Send(CefMessage message)
    {
      var syslogMessage = _serializer.Serialize(message);
      var stream = new MemoryStream(_serializer.Serialize(syslogMessage));
      var syslogMessageStr = new StreamReader(stream).ReadToEnd();
      _logger.LogInformation("Send syslog message: {SyslogMessage}", syslogMessageStr);
      _policy.Execute(() => SendInternal(message));
    }

    private CefMessageSerializer CreateSerializer()
    {
      switch (_cefConfig.SyslogVersion)
      {
        case "rfc3164":
          return new CefMessageSerializer(new SyslogRfc3164MessageSerializer());
        case "rfc5424":
          return new CefMessageSerializer(new SyslogRfc5424MessageSerializer());
        default:
          throw new ArcSightConfigurationException($"The syslog version {_cefConfig.SyslogVersion} has not supported");
      }
    }

    private void SendInternal(CefMessage message)
    {
      _logger.LogDebug("Call SendInternal...");
      using (var sender = _cefSenderFactory.CreateSender(_cefConfig))
      {
        sender.Send(message, _serializer);
      }

      _logger.LogDebug("Call SendInternal done");
    }
  }
}