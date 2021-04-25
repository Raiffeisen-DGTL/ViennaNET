using System.Linq;
using ViennaNET.Utils;

namespace ViennaNET.WebApi.Net.IpTools
{
  /// <summary>
  /// Преобразование локального IP-адреса (петли) в реальный
  /// </summary>
  public class LoopbackIpFilter : ILoopbackIpFilter
  {
    private static readonly string[] localIpValues =
    {
      "127.0.0.1", "::1", "0.0.0.1", "localhost", "none"
    };

    private readonly ILocalIpProvider _localIpProvider;

    public LoopbackIpFilter(ILocalIpProvider localIpProvider)
    {
      _localIpProvider = localIpProvider.ThrowIfNull(nameof(localIpProvider));
    }

    /// <summary>
    /// Метод нормализации значения IP-адреса
    /// </summary>
    /// <remarks>
    /// Преобразует пустые и локальные адреса (петли) в реальные значения
    /// </remarks>
    /// <param name="ip">строковое значение IP-адреса</param>    
    /// <returns>итоговый IP-адрес</returns>
    public string FilterIp(string ip)
    {
      return string.IsNullOrEmpty(ip) || localIpValues.Contains(ip)
        ? _localIpProvider.GetCurrentIp()
        : ip;
    }
  }
}
