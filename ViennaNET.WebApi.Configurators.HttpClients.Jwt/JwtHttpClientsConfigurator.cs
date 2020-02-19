using System;
using System.Linq;
using System.Net.Http;
using ViennaNET.Diagnostic;
using ViennaNET.HttpClient;
using ViennaNET.Logging;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Diagnostic;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Handlers;
using ViennaNET.WebApi.Configurators.HttpClients.Jwt.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ViennaNET.WebApi.Configurators.HttpClients.Jwt
{
  /// <summary>
  /// Конфигуратор для регистрации Http-клиентов с JwtRequestHeadersHandler
  /// </summary>
  public static class JwtHttpClientsConfigurator
  {
    /// <summary>
    /// Регистрирует Http-клиентов, реализующих проброску авторизации в заголовках
    /// </summary>
    public static ICompanyHostBuilder UseJwtHttpClients(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.RegisterServices(RegisterHttpClients);
    }

    internal static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
      services.TryAddSingleton<IDiagnosticImplementor, RestEndpointsChecker>();

      try
      {
        var endpoints = configuration.GetSection(WebapiEndpointsSection.SectionName)
                                     .Get<WebapiEndpoint[]>();
        if (endpoints == null)
        {
          services.AddHttpClient();
          return;
        }

        foreach (var endpoint in endpoints.Where(e => e.AuthType == CompanyHttpClientsTypes.Jwt))
        {
          ConfigureClient(endpoint)
            .Register(services);
        }
      }
      catch (Exception ex)
      {
        Logger.LogErrorFormat(ex, "Error while register HttpClients");
        throw;
      }
    }

    private static HttpClientRegistrator ConfigureClient(WebapiEndpoint endpoint)
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
