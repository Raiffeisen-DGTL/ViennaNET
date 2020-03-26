namespace ViennaNET.ArcSight.Configuration
{
  /// <summary>
  /// Секция для загрузки параметров ArcSight из конфигурации
  /// </summary>
  public class ArcSightSection
  {
    /// <summary>
    /// Имя сервера ArcSight.
    /// </summary>
    public string ServerHost { get; set; }

    /// <summary>
    /// Порт сервера ArcSight.
    /// </summary>
    public int ServerPort { get; set; }

    /// <summary>
    /// Версия протокола сериализцации данных.
    /// </summary>
    /// <remarks>
    /// Допускаются значения rfc3164 и rfc5424. <see cref="ArcSightClient"/>
    /// </remarks>
    public string SyslogVersion { get; set; }

    /// <summary>
    /// Версия протокола сериализцации данных.
    /// </summary>
    /// <remarks>
    /// Допускаются значения tcp, udp и local. <see cref="CefSenderFactory"/>
    /// </remarks>
    public string Protocol { get; set; }
  }
}
