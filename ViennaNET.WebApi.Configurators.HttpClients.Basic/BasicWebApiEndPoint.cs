using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration;

namespace ViennaNET.WebApi.Configurators.HttpClients.Basic
{
  /// <summary>
  /// Класс для чтения конфигурационных данных для basic httpclient
  /// </summary>
  internal sealed class BasicWebApiEndPoint : WebapiEndpoint
  {
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string Password { get; set; }
  }
}