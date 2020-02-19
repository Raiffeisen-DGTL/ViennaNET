using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Diagnostics;
using System.Linq;

namespace ViennaNET.WebApi.WindowsHosting
{
  /// <summary>
  /// Класс с расширением для запуска сервисов как Windows-службы
  /// </summary>
  public static class WindowsHostingExtensions
  {
    /// <summary>
    /// Запускает сервис как Windows-службу, если не подключен дебаггер
    /// и в аргументах не передан параметр запуска в режиме консоли
    /// </summary>
    /// <param name="webHost">Сконфигурированный WebHost</param>
    /// <param name="args">Аргументы командной строки</param>
    public static void RunAsWindowsService(this IWebHost webHost, string[] args)
    {
      var isService = !(Debugger.IsAttached || args.Contains("--console"));

      if (isService)
      {
        webHost.RunAsService();
        return;
      }

      webHost.Run();
    }
  }
}
