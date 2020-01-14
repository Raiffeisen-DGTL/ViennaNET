namespace Company.WebApi.Core.DefaultConfiguration.Swagger.Configuration
{
  /// <summary>
  /// Конфигурационна секция swagger
  /// </summary>
  public class SwaggerConfiguration
  {
    /// <summary>
    /// Адрес сервиса авторизации для автоматической авторизации в интерфейсе Swagger'а
    /// </summary>
    public string SecurityServiceUrl { get; set; }

    /// <summary>
    /// Http-глагол запроса токена в сервисе авторизации
    /// </summary>
    public string RequestMethod { get; set; }
  }
}
