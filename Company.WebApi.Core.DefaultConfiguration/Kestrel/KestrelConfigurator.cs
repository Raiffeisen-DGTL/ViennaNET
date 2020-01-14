using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net;
using Company.WebApi.Core.DefaultConfiguration.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Company.WebApi.Core.Validation;

namespace Company.WebApi.Core.DefaultConfiguration.Kestrel
{
  public static class KestrelConfigurator
  {
    /// <summary>
    /// Регистрирует Kestrel как сервер
    /// </summary>
    /// <param name="builder"></param>
    public static void Configure(IWebHostBuilder builder)
    {
      builder.UseKestrel(SetKestrelOptions);
    }

    private static void SetKestrelOptions(WebHostBuilderContext context, KestrelServerOptions options)
    {
      var hostConfiguration = context.Configuration.GetSection(CompanyWebApiConfiguration.SectionName)
                                     .Get<CompanyWebApiConfiguration>();

      options.Listen(IPAddress.Any, hostConfiguration.PortNumber);

      if (!hostConfiguration.HttpsPort.HasValue)
      {
        return;
      }

      var certificatePath = Directory.GetCurrentDirectory() + "\\AO Raiffeisenbank RootCA.crt";
      if (!File.Exists(certificatePath))
      {
        throw new HostBuilderValidationException("Файл сертификата для HTTPS не найден");
      }

      options.Listen(IPAddress.Any, hostConfiguration.HttpsPort.Value, listenOptions =>
      {
        listenOptions.UseHttps();
      });
    }
  }
}
