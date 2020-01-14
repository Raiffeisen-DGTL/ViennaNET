namespace Company.WebApi.Core.JsonConfiguration
{
  /// <summary>
  /// Объект для чтения конфигурационных данных из секции webApiConfiguration (общие настройки сервиса)
  /// </summary>
  public class BaseWebApiConfiguration
  {
    /// <summary>
    /// Признак включения Swagger'а
    /// </summary>
    public bool? SwaggerSubmit { get; set; }

    /// <summary>
    /// Название секции в кофнигурационном файле
    /// </summary>
    public const string SectionName = "webApiConfiguration";
  }
}
