﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ViennaNET.HttpClient;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Handlers;

namespace ViennaNET.WebApi.Configurators.HttpClients.Ntlm
{
    /// <summary>
    ///   Конфигуратор для регистрации Http-клиентов со специальной настройкой для поддержки NTLM
    /// </summary>
    public static class NtlmHttpClientsConfigurator
    {
        /// <summary>
        ///   Регистрирует Http-клиентов, реализующих NTLM-авторизацию
        /// </summary>
        public static ICompanyHostBuilder UseNtlmHttpClients(this ICompanyHostBuilder companyHostBuilder)
        {
            return companyHostBuilder.RegisterServices(RegisterHttpClients);
        }

        internal static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var endpoints = configuration.GetSection("webApiEndpoints")
                .Get<WebapiEndpoint[]>();
            if (endpoints == null)
            {
                services.AddHttpClient();
                return;
            }

            foreach (var endpoint in endpoints.Where(e => e.AuthType == CompanyHttpClientsTypes.Ntlm))
            {
                ConfigureNtlmClient(endpoint)
                    .Register(services);
            }
        }

        private static HttpClientRegistrator ConfigureBaseClient(WebapiEndpoint endpoint)
        {
            return HttpClientRegistrator.Create()
                .WithName(endpoint.Name)
                .WithUrl(endpoint.Url)
                .WithTimeout(endpoint.Timeout)
                .WithHandler<BaseCompanyRequestHeadersHandler>();
        }

        private static HttpClientRegistrator ConfigureNtlmClient(WebapiEndpoint endpoint)
        {
            return ConfigureBaseClient(endpoint)
                .ConfigureBuilder(x =>
                {
                    x.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        AllowAutoRedirect = false,
                        UseDefaultCredentials = true,
                        PreAuthenticate = true,
                        MaxConnectionsPerServer = endpoint.MaxConnections ?? int.MaxValue
                    });
                });
        }
    }
}