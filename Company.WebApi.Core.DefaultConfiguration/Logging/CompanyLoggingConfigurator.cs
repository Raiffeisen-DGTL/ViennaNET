using Company.Logging;
using Company.Logging.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Company.WebApi.Core.DefaultConfiguration.Logging
{
  public static class CompanyLoggingConfigurator
  {
    /// <summary>
    /// Регистрирует логгер из Company.Logging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="container"></param>
    public static void Configure(IApplicationBuilder builder, IConfiguration configuration, IHostingEnvironment env, object container)
    {
      Logger.Configure(new LoggerJsonCfgFileConfiguration(configuration));
    }
  }
}
