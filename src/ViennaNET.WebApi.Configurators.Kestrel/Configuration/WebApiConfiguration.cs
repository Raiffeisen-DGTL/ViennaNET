namespace ViennaNET.WebApi.Configurators.Kestrel.Configuration
{
  /// <summary>
  /// Объект для чтения конфигурационных данных из секции webApiConfiguration (общие настройки сервиса)
  /// </summary>
  public class WebApiConfiguration
  {
    private bool? _useHsts = true;

    /// <summary>
    /// Название секции в кофнигурационном файле
    /// </summary>
    public const string SectionName = "webApiConfiguration";

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
    /// Относительный путь до файла сертификата
    /// </summary>
    public string CertificatePath { get; set; }

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
