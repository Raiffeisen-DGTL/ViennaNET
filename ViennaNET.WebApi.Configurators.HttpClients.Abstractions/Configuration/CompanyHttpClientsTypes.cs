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
    public const string Jwt = "jwt";

    /// <summary>
    /// Тип Http-клиента с NTLM-авторизацией
    /// </summary>
    public const string Ntlm = "ntlm";
  }
}
