using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Company.WebApi.Core.StaticFiles
{
  /// <summary>
  /// Конфигуратор для настройки доступа к файлам через сервис
  /// </summary>
  public static class StaticFilesConfigurator
  {
    private const int defaultCacheInterval = 3600;

    /// <summary>
    /// Считывает секцию конфигурации webApiStaticFiles, если она есть и включает доступ к файлам
    /// </summary>
    /// <param name="app"></param>
    /// <param name="configuration"></param>
    public static void ConfigureStaticFiles(this IApplicationBuilder app, IConfiguration configuration)
    {
      var config = configuration.GetSection("webApiStaticFiles")
                                ?.Get<StaticFilesConfiguration>();
      if (config == null)
      {
        app.UseStaticFiles();
        return;
      }

      var cacheInterval = config.CacheInterval ?? defaultCacheInterval;

      var path = Path.Combine(Directory.GetCurrentDirectory(), config.Folder);
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }

      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(path),
        RequestPath = config.UrlPrefix.StartsWith("/") ? config.UrlPrefix : $"/{config.UrlPrefix}",
        OnPrepareResponse = ctx =>
        {
          ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cacheInterval}");
        }
      });
    }
  }
}
