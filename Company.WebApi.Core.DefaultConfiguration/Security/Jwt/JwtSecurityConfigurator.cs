using System;
using Company.Security;
using Company.Security.Jwt.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Company.WebApi.Core.DefaultConfiguration.Security.Jwt
{
  /// <summary>
  /// Настраивает JWT-авторизацию
  /// </summary>
  public static class JwtSecurityConfigurator
  {
    public static void UseAuthentication(IApplicationBuilder app, IConfiguration configuration, IHostingEnvironment env,
      object container)
    {
      app.UseAuthentication();
    }

    /// <summary>
    /// Добавляет обязательность авторизации ко всем запросам
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    public static void ConfigureMvcBuilder(IMvcCoreBuilder builder, IConfiguration configuration)
    {
      var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
      builder.AddAuthorization()
        .AddMvcOptions(o => o.Filters.Add(new AuthorizeFilter(policy)));
    }

    /// <summary>
    /// Добавляет JWT-авторизацию, параметры валидации токена,
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
      var securityKeysContainer = GetSecurityKeysContainer(configuration);

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(jwtBearerOptions =>
        {
          jwtBearerOptions.ForwardAuthenticate = JwtBearerDefaults.AuthenticationScheme;
          jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKeysContainer.GetDecodingKey()
              .GetKey(),
            ValidateIssuer = true,
            ValidIssuer = securityKeysContainer.Issuer(),
            ValidateAudience = true,
            ValidAudience = securityKeysContainer.Audience(),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5)
          };
        });

      services.AddSingleton<ISecurityContextFactory, JwtSecurityContextFactory>();
    }

    private static SecurityKeysContainer GetSecurityKeysContainer(IConfiguration configuration)
    {
      var jwtSettings = configuration.GetSection(JwtSecurityConfiguration.SectionName)
        .Get<JwtSecurityConfiguration>();

      return jwtSettings is null
        ? new SecurityKeysContainer()
        : new SecurityKeysContainer(jwtSettings.TokenKeyEnvVariable, jwtSettings.Audience, jwtSettings.Issuer,
          jwtSettings.SigningAlgorithm);
    }
  }
}