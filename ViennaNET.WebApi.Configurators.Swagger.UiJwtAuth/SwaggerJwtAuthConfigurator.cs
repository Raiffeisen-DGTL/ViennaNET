using System;
using System.Collections.Generic;
using System.IO;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.Swagger.UiJwtAuth.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ViennaNET.WebApi.Configurators.Swagger.UiJwtAuth
{
  /// <summary>
  /// Подключает Swagger к 
  /// </summary>
  public static class SwaggerJwtAuthConfigurator
  {
    public static ICompanyHostBuilder UseSwaggerWithJwtAuth(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.UseSwaggerWithOptions(ConfigureSwaggerUiOptions, ConfigureSwaggerGenOptions);
    }

    public static void ConfigureSwaggerUiOptions(SwaggerUIOptions options, IConfiguration configuration)
    {
      options.HeadContent = GetSwaggerScript(configuration);
    }

    public static void ConfigureSwaggerGenOptions(SwaggerGenOptions options, IConfiguration configuration)
    {
      options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
      {
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
      });
      options.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" }
          },
          new string[] {}
        }
      });
    }

    private static string GetSwaggerScript(IConfiguration configuration)
    {
      var assembly = typeof(SwaggerJwtAuthConfigurator).Assembly;
      var stream = assembly.GetManifestResourceStream("ViennaNET.WebApi.Configurators.Swagger.UiJwtAuth.swaggerAuth.js");

      if (stream is null)
      {
        return string.Empty;
      }

      var securityEndpoint = configuration.GetSection(SwaggerConfigurationSection.SectionName)
                                          .Get<SwaggerConfigurationSection>();
      if (securityEndpoint is null)
      {
        return string.Empty;
      }

      string scriptText;
      using (var reader = new StreamReader(stream))
      {
        scriptText = reader.ReadToEnd();
      }

      var script = scriptText.Replace("<security-host>", securityEndpoint.SecurityServiceUrl);
      script = script.Replace("<request-method>", securityEndpoint.RequestMethod ?? "GET");

      return $"<script>{script}</script>";
    }

  }
}
