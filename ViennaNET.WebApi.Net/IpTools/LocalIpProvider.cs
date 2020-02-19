using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ViennaNET.Logging;

namespace ViennaNET.WebApi.Net.IpTools
{
  /// <summary>
  /// Получение локального IPv4-адреса
  /// </summary>
  public class LocalIpProvider : ILocalIpProvider
  {
    /// <summary>
    /// Метод для получения IPv4-адреса в локальной сети
    /// </summary>
    /// <remarks>
    /// Получает текущий IPv4-адрес сетевого адаптера
    /// </remarks>
    /// <returns>IP-адрес</returns>
    public string GetCurrentIp()
    {
      var adapters = NetworkInterface.GetAllNetworkInterfaces();

      UnicastIPAddressInformationCollection addresses = null;
      if (adapters.Any())
      {
        addresses = adapters[0]
                    ?.GetIPProperties()
                    ?.UnicastAddresses;

        if (adapters.Length > 1)
        {
          var addressesWithDns = adapters.Where(adapter => adapter.OperationalStatus == OperationalStatus.Up)
                                         .FirstOrDefault(adapter => adapter.GetIPProperties()
                                                                           ?.DnsSuffix
                                                                           ?.Trim()
                                                                           .ToLower() == "raiffeisen.ru")
                                         ?.GetIPProperties()
                                         ?.UnicastAddresses;

          if (addressesWithDns != null && addressesWithDns.Any())
          {
            addresses = addressesWithDns;
          }
        }
      }

      if (addresses == null)
      {
        Logger.LogWarning("Network adapter not found, can not determine ip address");
        return string.Empty;
      }

      foreach (var ip in addresses)
      {
        if (ip?.Address?.AddressFamily != AddressFamily.InterNetwork ||
            IPAddress.IsLoopback(ip.Address))
        {
          continue;
        }

        return ip.Address.ToString();
      }

      Logger.LogWarning("No network adapters with an IPv4 address in the system!");
      return string.Empty;
    }
  }
}
