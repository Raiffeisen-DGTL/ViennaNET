using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ViennaNET.Extensions.Http.DependencyInjection;

namespace ViennaNET.KasperskyScanEngine.Client.DependencyInjection;

/// <summary>
///     Предоставляет методы расширения для регистрации HTTP клиента.
/// </summary>
public static class HttpClientServiceCollectionExtensions
{
    /// <summary>
    ///     Добавляет HTTP-клиент службы Kaspersky Scan Engine в колекцию служб.
    /// </summary>
    /// <param name="services">Ссылка на <see cref="IServiceCollection" />.</param>
    /// <param name="configuration">Ссылка на <see cref="IConfiguration"/>.</param>
    /// <param name="configure">
    ///     Делегат, осуществляющий настройку параметров клиента, если <see langword="null" />,
    ///     по умолчанию будет осущесвляться привязка к секции конфигурации
    ///     <see cref="KseClientOption"/>.<see cref="KseClientOption.SectionName" />.
    /// </param>
    /// <returns>Ссылка на <see cref="IHttpClientBuilder" />.</returns>
    public static IHttpClientBuilder AddKasperskyScanEngineApi(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<KseClientOption>? configure = null)
    {
        var configAction = configure ?? (option => configuration.GetSection(KseClientOption.SectionName).Bind(option));

        var builder = services
            .Configure<JsonSerializerOptions>(IKasperskyScanEngineApi.JsonSerializerOptionsName, options =>
            {
                options.Converters.Add(new JsonValueConverterBooleanString());
                options.WriteIndented = false;
                options.AllowTrailingCommas = false;
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.TypeInfoResolver = KseClientSerializerContext.Default;
            })
            .AddHttpClient<IKasperskyScanEngineApi, KseApi, KseClientOption>(configAction)
            .ConfigureHttpClient((provider, client) =>
            {
                var opt = provider.GetRequiredService<IOptionsMonitor<KseClientOption>>().CurrentValue;

                if (opt.AuthorizationToken is { } token)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            });

        return builder;
    }
}