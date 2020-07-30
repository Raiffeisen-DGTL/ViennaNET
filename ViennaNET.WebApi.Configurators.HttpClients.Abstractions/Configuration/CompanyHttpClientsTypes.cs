namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration
{
  /// <summary>
  /// Словарь с типами авторизации
  /// </summary>
  public static class CompanyHttpClientsTypes
  {
    /// <summary>
    /// Тип Http-клиента с JWT-авторизацией
    /// </summary>
    public static readonly string Jwt = "jwt";

    /// <summary>
    /// Тип Http-клиента с NTLM-авторизацией
    /// </summary>
    public static readonly string Ntlm = "ntlm";

    /// <summary>
    /// Тип Http-клиента без авторизации
    /// </summary>
    public static readonly string NoAuth = "noauth";

    /// <summary>
    /// Тип Http-клиента с basic-авторизацией 
    /// </summary>
    public static readonly string Basic = "basic";
  }
}
