namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration
{
  /// <summary>
  /// Описание секции Http-клиента в конфигурационном файле
  /// </summary>
  public static class WebapiEndpointsSection
  {
    /// <summary>
    /// Название секции в кофнигурационном файле
    /// </summary>
    public const string SectionName = "webApiEndpoints";
  }

  /// <summary>
  /// Класс для чтения конфигурационных данных из секции webApiEndpoints (список подключений к сторонним сервисам по Http-протоколу)
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
    /// Тип авторизации
    /// </summary>
    public string AuthType { get; set; }

    /// <summary>
    /// Максимальное время подключения
    /// </summary>
    public int? Timeout { get; set; }
  }
}
