namespace ViennaNET.WebApi.Configurators.SimpleInjector.Configuration
{
  /// <summary>
  /// Объект для чтения конфигурационных данных из секции webApiConfiguration (общие настройки сервиса)
  /// </summary>
  public class SimpleInjectorConfiguration
  {
    /// <summary>
    /// Название секции в конфигурационном файле
    /// </summary>
    public const string SectionName = "simpleInjector";

    /// <summary>
    /// Флаг включающий динамическое подключение модулей с поиском внутри папки приложения
    /// Если не установлено, то все модули подключаются вручную
    /// </summary>
    public bool LoadPackagesDynamically { get; set; }
  }
}
