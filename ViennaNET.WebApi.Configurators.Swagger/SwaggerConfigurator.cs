using System;
using System.IO;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.Swagger.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ViennaNET.WebApi.Configurators.Swagger
{
  /// <summary>
  /// Подключает Swagger
  /// </summary>
  public static class SwaggerConfigurator
  {
    public static ICompanyHostBuilder UseSwagger(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.ConfigureApp((a, c, e, o) => ConfigureSwagger(a, c, e, o, null))
                               .RegisterServices((s, c) => AddSwagger(s, c, null));
    }

    public static ICompanyHostBuilder UseSwaggerWithOptions(
      this ICompanyHostBuilder companyHostBuilder, Action<SwaggerUIOptions, IConfiguration> swaggerUiConfig,
      Action<SwaggerGenOptions, IConfiguration> swaggerGenConfig)
    {
      return companyHostBuilder.ConfigureApp((a, c, e, o) => ConfigureSwagger(a, c, e, o, swaggerUiConfig))
                               .RegisterServices((s, c) => AddSwagger(s, c, swaggerGenConfig));
    }

    /// <summary>
    /// Включает интерфейс Swagger'а
    /// </summary>
    /// <param name="app"></param>
    /// <param name="configuration"></param>
    /// <param name="hostEnvironment"></param>
    /// <param name="container"></param>
    /// <param name="swaggerUiConfig"></param>
    internal static void ConfigureSwagger(
      IApplicationBuilder app, IConfiguration configuration, IHostEnvironment hostEnvironment, object container,
      Action<SwaggerUIOptions, IConfiguration> swaggerUiConfig)
    {
      var swaggerConfiguration = configuration.GetSection(SwaggerConfigurationSection.SectionName)
                                              .Get<SwaggerConfigurationSection>();

      if (swaggerConfiguration?.UseSwagger != true)
      {
        return;
      }

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service API V1");
        swaggerUiConfig?.Invoke(c, configuration);
      });
    }

    /// <summary>
    /// Включает генерацию файла-описания публичного API сервиса
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="swaggerGenConfig"></param>
    internal static void AddSwagger(
      IServiceCollection services, IConfiguration configuration, Action<SwaggerGenOptions, IConfiguration> swaggerGenConfig)
    {
      var swaggerConfiguration = configuration.GetSection(SwaggerConfigurationSection.SectionName)
                                              .Get<SwaggerConfigurationSection>();

      if (swaggerConfiguration?.UseSwagger != true)
      {
        return;
      }

      var name = configuration.GetValue<string>("serviceAssemblyName");
      var version = configuration.GetValue<string>("serviceAssemblyVersion");

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{name}, version: {version}", Version = "v1" });
        swaggerGenConfig?.Invoke(c, configuration);

        var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
        foreach (var xmlFile in xmlFiles)
        {
          c.IncludeXmlComments(xmlFile);
        }
      });
    }
  }
}
