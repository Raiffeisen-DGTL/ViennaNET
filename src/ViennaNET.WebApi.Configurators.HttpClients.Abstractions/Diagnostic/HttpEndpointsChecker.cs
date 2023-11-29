using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Utils;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration;

namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Diagnostic
{
  /// <summary>
  ///   Проводит диагностику всех Http-подключений к другим сервисам
  /// </summary>
  public class HttpEndpointsChecker : IDiagnosticImplementor
  {
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger _logger;
    private readonly WebapiEndpoint[] _restEndPoints;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="clientFactory"></param>
    /// <param name="logger"></param>
    public HttpEndpointsChecker(IConfiguration configuration, IHttpClientFactory clientFactory,
      ILogger<HttpEndpointsChecker> logger)
    {
      _restEndPoints = configuration.GetSection("webApiEndpoints")
        .Get<WebapiEndpoint[]>() ?? new WebapiEndpoint[0];
      _clientFactory = clientFactory.ThrowIfNull(nameof(clientFactory));
      _logger = logger.ThrowIfNull(nameof(logger));
    }

    public async Task<IEnumerable<DiagnosticInfo>> Diagnose()
    {
      var tasks = _restEndPoints.Where(e => e.IsHealthCheck != false)
        .Select(e =>
        {
          var client = _clientFactory.CreateClient(e.Name);
          return PingEndpointAsync(e, client);
        });
      return await Task.WhenAll(tasks)
        .ConfigureAwait(false);
    }

    public string Key => "http-client";

    private async Task<DiagnosticInfo> PingEndpointAsync(WebapiEndpoint endpoint, System.Net.Http.HttpClient client)
    {
      try
      {
        _logger.LogTrace("Diagnosting endpoint {Endpoint}", endpoint.Name);

        using var response = await client.GetAsync("/diagnostic/ping").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        _logger.LogTrace("Endpoint {Endpoint} has been diagnosed successfully", endpoint.Name);

        return new DiagnosticInfo(endpoint.Name, endpoint.Url);
      }
      catch (Exception e)
      {
        _logger.LogTrace(e, "Diagnostic of endpoint {Endpoint} has been failed with error", endpoint.Name);

        return new DiagnosticInfo(endpoint.Name, endpoint.Url, DiagnosticStatus.PingError, string.Empty, e.ToString());
      }
    }
  }
}