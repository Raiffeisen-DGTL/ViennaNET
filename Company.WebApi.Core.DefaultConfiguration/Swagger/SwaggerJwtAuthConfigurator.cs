using System.Collections.Generic;
using System.IO;
using Company.WebApi.Core.DefaultConfiguration.Swagger.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Company.WebApi.Core.DefaultConfiguration.Swagger
{
  public static class SwaggerJwtAuthConfigurator
  {
    public static void ConfigureSwaggerUiOptions(SwaggerUIOptions options, IConfiguration configuration)
    {
      options.HeadContent = GetSwaggerScript(configuration);
    }

    public static void ConfigureSwaggerGenOptions(SwaggerGenOptions options, IConfiguration configuration)
    {
      options.AddSecurityDefinition("Bearer",
                                    new ApiKeyScheme
                                    {
                                      Description =
                                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                                      Name = "Authorization",
                                      In = "header",
                                      Type = "apiKey",
                                    });

      var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, };

      options.AddSecurityRequirement(security);
    }

    private static string GetSwaggerScript(IConfiguration configuration)
    {
      var assembly = typeof(SwaggerJwtAuthConfigurator).Assembly;
      var stream = assembly.GetManifestResourceStream("Company.WebApi.Core.DefaultConfiguration.Swagger.swaggerAuth.js");

      if (stream is null)
      {
        return string.Empty;
      }

      var securityEndpoint = configuration.GetSection("swagger")
                                          .Get<SwaggerConfiguration>();
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
