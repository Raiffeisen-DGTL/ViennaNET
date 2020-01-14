using Company.Security.Jwt;
using Company.Security.Jwt.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.WebApi.Core.DefaultConfiguration.Security.Jwt
{
  public static class JwtFactoryConfigurator
  {
    /// <summary>
    /// Регистрирует фабрику для генерации
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void RegisterFactory(IServiceCollection services, IConfiguration configuration)
    {
      var securityKeysContainer = GetSecurityKeysContainer(configuration);

      services.AddSingleton<ISecurityKeysContainer>(securityKeysContainer);
      services.AddSingleton<IJwtTokenFactory, JwtTokenFactory>();
    }

    private static SecurityKeysContainer GetSecurityKeysContainer(IConfiguration configuration)
    {
      var jwtSettings = configuration.GetSection(JwtSecurityConfiguration.SectionName)
        .Get<JwtSecurityConfiguration>();

      return jwtSettings is null
        ? new SecurityKeysContainer()
        : new SecurityKeysContainer(jwtSettings.TokenKeyEnvVariable, jwtSettings.Audience, jwtSettings.Issuer, jwtSettings.SigningAlgorithm);
    }
  }
}