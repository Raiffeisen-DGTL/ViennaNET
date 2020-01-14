using System;
using System.Net;
using System.Net.Http;
using Company.HttpClient;
using Company.Logging;
using Company.WebApi.Core.DefaultConfiguration.HttpClients.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.WebApi.Core.DefaultConfiguration.HttpClients
{
  /// <summary>
  /// Конфигуратор для регистрации Http-клиентов с JwtRequestHeadersHandler
  /// </summary>
  public static class JwtHttpClientsConfigurator
  {
    public static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
      try
      {
        var endpoints = configuration.GetSection("webApiEndpoints")
                                     .Get<WebapiEndpoint[]>();
        if (endpoints == null)
        {
          services.AddHttpClient();
          return;
        }

        foreach (var endpoint in endpoints)
        {
          ConfigureJwtClient(endpoint)
            .Register(services);
        }
      }
      catch (Exception ex)
      {
        Logger.LogErrorFormat(ex, "Error while register HttpClients");
        throw;
      }
    }

    private static HttpClientRegistrator ConfigureJwtClient(WebapiEndpoint endpoint)
    {
      return HttpClientRegistrator.Create()
                                  .WithName(endpoint.Name)
                                  .WithUrl(endpoint.Url)
                                  .WithTimeout(endpoint.Timeout)
                                  .WithHandler<BaseCompanyRequestHeadersHandler>()
                                  .WithHandler<RequestAuthorizationHeaderHandler>()
                                  .ConfigureBuilder(x => x.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                                  {
                                    AllowAutoRedirect = false,
                                    UseProxy = false
                                  }));
    }
  }
}
