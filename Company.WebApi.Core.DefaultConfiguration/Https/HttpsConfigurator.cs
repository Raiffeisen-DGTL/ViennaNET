using System;
using Company.WebApi.Core.DefaultConfiguration.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.WebApi.Core.DefaultConfiguration.Https
{
  public static class HttpsConfigurator
  {
    /// <summary>
    /// Включает Hsts для продуктовой среды
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="env"></param>
    /// <param name="container"></param>
    public static void Configure(IApplicationBuilder builder, IConfiguration configuration, IHostingEnvironment env, object container)
    {
      var hostConfiguration = configuration.GetSection(CompanyWebApiConfiguration.SectionName)
                                           .Get<CompanyWebApiConfiguration>();

      if (!hostConfiguration.HttpsPort.HasValue)
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
    public static void ConfigureRedirect(IServiceCollection services, IConfiguration configuration)
    {
      var hostConfiguration = configuration.GetSection(CompanyWebApiConfiguration.SectionName)
                                     .Get<CompanyWebApiConfiguration>();

      if (!hostConfiguration.HttpsPort.HasValue)
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
