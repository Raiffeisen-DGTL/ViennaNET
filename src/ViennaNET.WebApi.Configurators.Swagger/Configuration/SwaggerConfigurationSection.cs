namespace ViennaNET.WebApi.Configurators.Swagger.Configuration
{
  /// <summary>
  ///   Секция конфигурации Swagger
  /// </summary>
  public class SwaggerConfigurationSection
  {
    /// <summary>
    ///   Название секции в конфигурационном файле
    /// </summary>
    public const string SectionName = "swagger";

    /// <summary>
    ///   Признак включения Swagger'а
    /// </summary>
    public bool? UseSwagger { get; set; }

    /// <summary>
    ///   Имя родительского сервиса. Используется для IIS когда сервис устанавливается как вложенный сайт.
    /// </summary>
    public string ParentSiteName { get; set; }
  }
}