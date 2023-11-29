using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ViennaNET.Extensions.Http.Authentication;

/// <summary>
///     Предоставляет методы расширения для настройки аутентификации используемой HTTP клиентом.
/// </summary>
public static class AuthenticationHttpClientBuilderExtensions
{
    /// <summary>
    ///     Настраивает основной <see cref="HttpMessageHandler " /> для использования Negotiate аутентификаци.
    /// </summary>
    /// <param name="builder">Ссылка на <see cref="IHttpClientBuilder"/>.</param>
    /// <typeparam name="TOption">Ссылка на <typeparamref name="TOption"/>, параметры клиента.</typeparam>
    /// <returns>Ссылка на <see cref="IHttpClientBuilder"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Возникает, если не установлены параметры
    ///     <typeparamref name="TOption"/>.
    ///     <see cref="ClientOptionsBase.Authentication"/>.<see cref="AuthenticationOptions.Negotiate"/>.
    /// </exception>
    public static IHttpClientBuilder UseNegotiateAuthentication<TOption>(this IHttpClientBuilder builder)
        where TOption : ClientOptionsBase
    {
        return builder.ConfigurePrimaryHttpMessageHandler(provider =>
        {
            var options = provider.GetRequiredService<IOptions<TOption>>().Value;

            if (options.Authentication?.Negotiate == null)
            {
                throw new InvalidOperationException($"Требуются параметры конфигурации: {nameof(NegotiateOptions)}.");
            }

            return options.Authentication.Negotiate.UseDefaultCredentials
                ? new HttpClientHandler { UseDefaultCredentials = true }
                : new HttpClientHandler
                {
                    Credentials = new NetworkCredential(options.Authentication.Negotiate.UserName,
                        options.Authentication.Negotiate.Password)
                };
        });
    }
}