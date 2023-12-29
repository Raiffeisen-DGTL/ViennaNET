using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ViennaNET.Diagnostic;
using ViennaNET.HttpClient;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Diagnostic;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Handlers;

namespace ViennaNET.WebApi.Configurators.HttpClients.NoAuthentication
{
    /// <summary>
    ///   Конфигуратор для регистрации Http-клиентов без предустановленных заголовков авторизации
    /// </summary>
    public static class NoAuthenticationHttpClientsConfigurator
    {
        /// <summary>
        ///   Регистрирует Http-клиентов, не реализующих проброску авторизации в заголовках
        /// </summary>
        public static ICompanyHostBuilder UseNoAuthHttpClients(this ICompanyHostBuilder companyHostBuilder)
        {
            return companyHostBuilder.RegisterServices(RegisterHttpClients);
        }

        /// <summary>
        ///   Регистрирует Http-клиенты
        /// </summary>
        internal static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IDiagnosticImplementor, HttpEndpointsChecker>();

            var endpoints = configuration.GetSection(WebapiEndpointsSection.SectionName)
                .Get<WebapiEndpoint[]>();
            if (endpoints == null)
            {
                services.AddHttpClient();
                return;
            }

            foreach (var endpoint in endpoints.Where(e => e.AuthType == CompanyHttpClientsTypes.NoAuth))
            {
                ConfigureClient(endpoint)
                    .Register(services);
            }
        }

        /// <summary>
        ///   Создаёт и конфигурирует Http-клиенты
        /// </summary>
        private static HttpClientRegistrator ConfigureClient(WebapiEndpoint endpoint)
        {
            return HttpClientRegistrator.Create()
                .WithName(endpoint.Name)
                .WithUrl(endpoint.Url)
                .WithTimeout(endpoint.Timeout)
                .WithHandler<BaseCompanyRequestHeadersHandler>()
                .ConfigureBuilder(x => x.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    UseProxy = false,
                    MaxConnectionsPerServer = endpoint.MaxConnections ?? int.MaxValue
                }));
        }
    }
}