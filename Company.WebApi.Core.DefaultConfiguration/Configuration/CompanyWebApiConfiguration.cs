using Company.WebApi.Core.JsonConfiguration;

namespace Company.WebApi.Core.DefaultConfiguration.Configuration
{
  /// <summary>
  /// Объект для чтения конфигурационных данных из секции webApiConfiguration (общие настройки сервиса)
  /// </summary>
  public class CompanyWebApiConfiguration : BaseWebApiConfiguration
  {
    private bool? _useHsts = true;

    /// <summary>
    /// Номер порта сервиса
    /// </summary>
    public int PortNumber { get; set; }

    /// <summary>
    /// Порт для Https
    /// </summary>
    public int? HttpsPort { get; set; }

    /// <summary>
    /// Блокирует использование Http
    /// </summary>
    public bool? UseStrictHttps { get; set; }

    /// <summary>
    /// Выключает отправку HSTS-заголовка в ответах
    /// </summary>
    public bool? UseHsts
    {
      get => _useHsts;
      set => _useHsts = value ?? true;
    }
  }
}
