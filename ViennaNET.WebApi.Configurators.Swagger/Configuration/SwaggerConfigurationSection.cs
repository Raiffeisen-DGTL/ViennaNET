namespace ViennaNET.WebApi.Configurators.Swagger.Configuration
{
  /// <summary>
  /// Секция конфигурации Swagger
  /// </summary>
  public class SwaggerConfigurationSection
  {
    /// <summary>
    /// Признак включения Swagger'а
    /// </summary>
    public bool? UseSwagger { get; set; }

    /// <summary>
    /// Название секции в конфигурационном файле
    /// </summary>
    public const string SectionName = "swagger";
  }
}
