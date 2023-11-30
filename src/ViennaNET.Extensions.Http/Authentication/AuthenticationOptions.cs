namespace ViennaNET.Extensions.Http.Authentication;

/// <summary>
///     Представляет параметры аутентификации HTTP клиента.
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    ///     Параметры конфигурации для Negotiate схемы аутентификации. (WWW-Authenticate: Negotiate)
    /// </summary>
    public NegotiateOptions? Negotiate { get; set; }
}