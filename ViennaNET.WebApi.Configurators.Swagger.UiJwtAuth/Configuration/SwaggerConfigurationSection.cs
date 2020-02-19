namespace ViennaNET.WebApi.Configurators.Swagger.UiJwtAuth.Configuration
{
  /// <summary>
  /// Секция конфигурации Swagger
  /// </summary>
  public class SwaggerConfigurationSection : Swagger.Configuration.SwaggerConfigurationSection
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
