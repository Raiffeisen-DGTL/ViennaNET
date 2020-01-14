using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Company.WebApi.Core.Swagger
{
  /// <summary>
  /// Конфигурирует Swagger
  /// </summary>
  public static class SwaggerConfigurator
  {
    /// <summary>
    /// Включает интерфейс Swagger'а
    /// </summary>
    /// <param name="app"></param>
    /// <param name="configureOptions"></param>
    public static void ConfigureSwagger(this IApplicationBuilder app, Action<SwaggerUIOptions> configureOptions = null)
    {
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service API V1");
        configureOptions?.Invoke(c);
      });
    }

    /// <summary>
    /// Включает генерацию файла-описания публичного API сервиса
    /// </summary>
    /// <param name="services"></param>
    /// <param name="title"></param>
    /// <param name="configureOptions"></param>
    public static void AddSwagger(this IServiceCollection services, string title, Action<SwaggerGenOptions> configureOptions = null)
    {
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info { Title = title, Version = "v1" });
        configureOptions?.Invoke(c);

        var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
        foreach (var xmlFile in xmlFiles)
        {
          c.IncludeXmlComments(xmlFile);
        }
      });
    }
  }
}
