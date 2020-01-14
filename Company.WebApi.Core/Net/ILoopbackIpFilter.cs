namespace Company.WebApi.Core.Net
{
  /// <summary>
  /// Интерфейс для преобразования локального IP-адреса (петли) в реальный
  /// </summary>
  public interface ILoopbackIpFilter
  {
    /// <summary>
    /// Метод нормализации значения IP-адреса
    /// </summary>
    /// <remarks>
    /// Преобразует пустые и локальные адреса (петли) в реальные значения
    /// </remarks>
    /// <param name="ip">строковое значение IP-адреса</param>    
    /// <returns>итоговый IP-адрес</returns>
    string FilterIp(string ip);
  }
}
