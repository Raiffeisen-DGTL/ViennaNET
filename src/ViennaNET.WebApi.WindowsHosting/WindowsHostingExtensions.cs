using System.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.WindowsHosting
{
  /// <summary>
  ///   Класс с расширением для настройки сервисов как Windows-службы
  /// </summary>
  [ExcludeFromCodeCoverage]
  public static class WindowsHostingExtensions
  {
    /// <summary>
    ///   Настраивает сервис как Windows-службу, если не подключен дебаггер
    ///   и в аргументах не передан параметр запуска в режиме консоли
    /// </summary>
    /// <param name="hostBuilder">Сборщик сервиса</param>
    /// <param name="args">Аргументы командной строки</param>
    /// <returns>Сборщик сервиса</returns>
    public static ICompanyHostBuilder UseWindowsService(this ICompanyHostBuilder hostBuilder, string[] args)
    {
      var isService = !(Debugger.IsAttached || args.Contains("--console"));

      if (isService)
      {
        hostBuilder.RegisterHostBuilderAction(builder => builder.UseWindowsService());
      }

      return hostBuilder;
    }
  }
}