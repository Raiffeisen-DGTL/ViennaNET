using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Company.Diagnostic.Core;
using Company.Diagnostic.Core.Data;
using Company.Logging;
using Company.Utils;
using Company.WebApi.Core.DefaultConfiguration.HttpClients;
using Microsoft.Extensions.Configuration;

namespace Company.WebApi.Core.DefaultConfiguration.Diagnostic.Checkers
{
  /// <summary>
  ///   Проводит диагностику всех Http-подключений к другим сервисам
  /// </summary>
  internal class RestEndpointsChecker : IDiagnosticImplementor
  {
    private readonly IHttpClientFactory _clientFactory;
    private readonly WebapiEndpoint[] _restEndPoints;

    public RestEndpointsChecker(IConfiguration configuration, IHttpClientFactory clientFactory)
    {
      _restEndPoints = configuration.GetSection("webApiEndpoints")
                                    .Get<WebapiEndpoint[]>() ?? new WebapiEndpoint[0];
      _clientFactory = clientFactory.ThrowIfNull(nameof(clientFactory));
    }

    public async Task<IEnumerable<DiagnosticInfo>> Diagnose()
    {
      var tasks = _restEndPoints.Select(e =>
                                {
                                  var client = _clientFactory.CreateClient(e.Name);
                                  return PingEndpoint(e, client);
                                })
                                .ToList();
      return await Task.WhenAll(tasks)
                       .ConfigureAwait(false);
    }

    public string Key => "http-client";

    private static async Task<DiagnosticInfo> PingEndpoint(WebapiEndpoint endpoint, System.Net.Http.HttpClient client)
    {
      try
      {
        Logger.LogDiagnostic($"Diagnosting the REST endpoint {endpoint.Name}");
        var pinged = DiagnosticStatus.PingError;

        using (var response = await client.GetAsync("/diagnostic/ping"))
        {
          if (response.IsSuccessStatusCode)
          {
            pinged = DiagnosticStatus.Ok;
            Logger.LogDiagnostic($"REST endpoint {endpoint.Name} has been diagnosed successfully");
          }
        }

        return new DiagnosticInfo(endpoint.Name, endpoint.Url, pinged);
      }
      catch (Exception e)
      {
        Logger.LogDiagnostic($"Diagnostic of REST endpoint {endpoint.Name} has been failed with error: {e}");
        return new DiagnosticInfo(endpoint.Name, endpoint.Url, DiagnosticStatus.PingError, string.Empty, e.ToString());
      }
    }
  }
}

