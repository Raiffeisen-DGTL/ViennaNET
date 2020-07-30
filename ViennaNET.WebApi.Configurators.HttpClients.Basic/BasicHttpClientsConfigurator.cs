using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ViennaNET.Diagnostic;
using ViennaNET.HttpClient;
using ViennaNET.Logging;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Diagnostic;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Handlers;

namespace ViennaNET.WebApi.Configurators.HttpClients.Basic
{
  /// <summary>
  /// Конфигуратор для регистрации Http-клиентов c basic авторизацией
  /// </summary>
  public static class BasicHttpClientsConfigurator
  {
    /// <summary>
    /// Регистрирация Http-клиентов с basic авторизацией
    /// </summary>
    /// <param name="companyHostBuilder"></param>
    /// <returns></returns>
    public static ICompanyHostBuilder UseBasicHttpClients(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.RegisterServices(RegisterHttpClients);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    internal static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
      services.TryAddSingleton<IDiagnosticImplementor, RestEndpointsChecker>();

      try
      {
        var endpoints = configuration.GetSection(WebapiEndpointsSection.SectionName)
          .Get<BasicWebApiEndPoint[]>();

        if (endpoints == null)
        {
          services.AddHttpClient();
          return;
        }

        foreach (var endpoint in endpoints.Where(e => e.AuthType == CompanyHttpClientsTypes.Basic))
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

    /// <summary>
    /// Создаёт и конфигурирует Http-клиенты
    /// </summary>
    private static HttpClientRegistrator ConfigureClient(BasicWebApiEndPoint endpoint)
    {
      return HttpClientRegistrator.Create()
        .WithName(endpoint.Name)
        .WithUrl(endpoint.Url)
        .WithTimeout(endpoint.Timeout)
        .WithHandler(() => new BasicHttpClientAuthorizationRequestHandler(endpoint.UserName, endpoint.Password))
        .WithHandler<BaseCompanyRequestHeadersHandler>()
        .ConfigureBuilder(x => x.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
          AllowAutoRedirect = false,
          UseProxy = false
        }));
    }
  }
}