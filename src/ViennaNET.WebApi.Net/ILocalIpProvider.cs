namespace ViennaNET.WebApi.Net
{
  /// <summary>
  /// Интерфейс для получения локального IPv4-адреса
  /// </summary>
  public interface ILocalIpProvider
  {
    /// <summary>
    /// Метод для получения IPv4-адреса в локальной сети
    /// </summary>
    /// <remarks>
    /// Получает текущий IPv4-адрес сетевого адаптера
    /// </remarks>
    /// <returns>IP-адрес</returns>
    string GetCurrentIp();
  }
}
