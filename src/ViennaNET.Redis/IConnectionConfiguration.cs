namespace ViennaNET.Redis
{
  /// <summary>
  ///   Провайдер конфигурации Redis
  /// </summary>
  public interface IConnectionConfiguration
  {
    /// <summary>
    ///   Возвращает конфигурацию подключения к Redis
    /// </summary>
    /// <returns>Конфигурация подключения к Redis</returns>
    ConnectionOptions GetConnectionConfigurationOptions();
  }
}