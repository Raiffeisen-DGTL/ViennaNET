namespace Company.WebApi.Core.DefaultConfiguration.HttpClients
{
  /// <summary>
  /// Объект для чтения конфигурационных данных из секции webApiEndpoints (список подключений к сторонним сервисам по Http-протоколу)
  /// </summary>
  public class WebapiEndpoint
  {
    /// <summary>
    /// Название подключения, используется для получения экземпляра HttpClient
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Базовый URL стороннего сервиса
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Максимальное время подключения
    /// </summary>
    public int? Timeout { get; set; }
  }
}
