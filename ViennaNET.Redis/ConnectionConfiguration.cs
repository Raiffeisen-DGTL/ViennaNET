using System;
using System.Linq;
using ViennaNET.Redis.Configuration;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace ViennaNET.Redis
{
  /// <inheritdoc/>
  public class ConnectionConfiguration : IConnectionConfiguration
  {
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="IConfiguration"/>
    /// </summary>
    /// <param name="configuration">Ссылка на интерфейс, обеспечивающий доступ к конфигурации</param>
    public ConnectionConfiguration(IConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc/>
    public ConnectionOptions GetConnectionConfigurationOptions()
    {
      var redisConfiguration = _configuration.GetSection("redis").Get<RedisConfiguration>();
      return new ConnectionOptions(ConfigurationOptions.Parse(redisConfiguration.Connection), redisConfiguration.Key,
                                   redisConfiguration.ExpirationMinValue, redisConfiguration.ExpirationMaxValue,
                                   redisConfiguration.KeyLifetimes.ToDictionary(x => x.Name, x => x.Time));
    }
  }
}
