using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.Kestrel.Configuration;

namespace ViennaNET.WebApi.Configurators.Kestrel
{
  /// <summary>
  /// Конфигуратор для регистрации Kestrel в качестве сервера
  /// </summary>
  public static class KestrelConfigurator
  {
    /// <summary>
    /// Регистрирует Kestrel как сервер на основе параметров в конфигурации
    /// </summary>
    public static IViennaHostBuilder UseKestrel(this IViennaHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.UseServer(ConfigureKestrel)
                               .ConfigureApp(ConfigureHsts)
                               .RegisterServices(ConfigureRedirect);
    }

    internal static void ConfigureKestrel(IWebHostBuilder builder)
    {
      builder.UseKestrel(SetKestrelOptions);
    }

    internal static void SetKestrelOptions(WebHostBuilderContext context, KestrelServerOptions options)
    {
      var hostConfiguration = context.Configuration.GetSection(WebApiConfiguration.SectionName)
                                     .Get<WebApiConfiguration>();

      if (hostConfiguration == null)
      {
        return;
      }

      options.Listen(IPAddress.Any, hostConfiguration.PortNumber);

      if (!hostConfiguration.HttpsPort.HasValue)
      {
        return;
      }

      var certificatePath = Path.Combine(Directory.GetCurrentDirectory(), hostConfiguration.CertificatePath);
      if (!File.Exists(certificatePath))
      {
        throw new FileNotFoundException("Файл сертификата для HTTPS не найден");
      }

      options.Listen(IPAddress.Any, hostConfiguration.HttpsPort.Value, listenOptions =>
      {
        listenOptions.UseHttps();
      });
    }

    /// <summary>
    /// Настраивает Hsts
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="env"></param>
    /// <param name="container"></param>
    internal static void ConfigureHsts(IApplicationBuilder builder, IConfiguration configuration, IHostEnvironment env, object container)
    {
      var hostConfiguration = configuration.GetSection(WebApiConfiguration.SectionName)
                                           .Get<WebApiConfiguration>();

      if (hostConfiguration?.HttpsPort == null)
      {
        return;
      }

      if (hostConfiguration.UseHsts == true && !env.IsDevelopment())
      {
        builder.UseHsts();
      }

      if (hostConfiguration.UseStrictHttps.HasValue && hostConfiguration.UseStrictHttps.Value)
      {
        builder.UseHttpsRedirection();
      }
    }

    /// <summary>
    /// Настраивает редиректы с Http на Https
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    internal static void ConfigureRedirect(IServiceCollection services, IConfiguration configuration)
    {
      var hostConfiguration = configuration.GetSection(WebApiConfiguration.SectionName)
                                           .Get<WebApiConfiguration>();

      if (hostConfiguration?.HttpsPort == null)
      {
        return;
      }

      if (hostConfiguration.UseHsts == true)
      {
        services.AddHsts(options =>
        {
          options.Preload = true;
          options.IncludeSubDomains = true;
          options.MaxAge = TimeSpan.FromDays(60);
        });
      }

      if (hostConfiguration.UseStrictHttps.HasValue && hostConfiguration.UseStrictHttps.Value)
      {
        services.AddHttpsRedirection(options =>
        {
          options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
          options.HttpsPort = hostConfiguration.HttpsPort;
        });
      }
    }
  }
}
