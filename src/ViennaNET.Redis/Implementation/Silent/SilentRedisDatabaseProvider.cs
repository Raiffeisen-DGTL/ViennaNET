using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;

namespace ViennaNET.Redis.Implementation.Silent
{
  /// <inheritdoc />
  public class SilentRedisDatabaseProvider : ISilentRedisDatabaseProvider
  {
    private readonly IRedisDatabaseProvider _redisDatabaseProvider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="IConnectionConfiguration" />
    /// </summary>
    /// <param name="redisDatabaseProvider">Провайдер конфигурации</param>
    /// <param name="loggerFactory">Фабрика логгеров</param>
    /// <param name="logger">Логгер</param>
    public SilentRedisDatabaseProvider(IRedisDatabaseProvider redisDatabaseProvider, ILoggerFactory loggerFactory,
      ILogger<SilentRedisDatabaseProvider> logger)
    {
      _redisDatabaseProvider = redisDatabaseProvider;
      _loggerFactory = loggerFactory;
      _logger = logger;
    }

    /// <inheritdoc />
    public IRedisDatabase GetDatabase(bool useCompression = false, int database = 0, object asyncState = null)
    {
      try
      {
        var db = _redisDatabaseProvider.GetDatabase(useCompression, database, asyncState);
        return new SilentRedisDatabase(db, _loggerFactory.CreateLogger<SilentRedisDatabase>());
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Taking of the Redis database has been failed.");
        return new SilentRedisDatabase(null, null);
      }
    }

    /// <inheritdoc />
    public ISilentRedisServer GetServer(EndPoint endPoint, object asyncState = null)
    {
      try
      {
        var server = _redisDatabaseProvider.GetServer(endPoint, asyncState);
        return new SilentRedisServer(server, _loggerFactory.CreateLogger<SilentRedisServer>());
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Taking of the Redis server has been failed.");
        return new SilentRedisServer(null, null);
      }
    }

    /// <inheritdoc />
    public IEnumerable<ISilentRedisServer> GetAllServers(object asyncState = null)
    {
      try
      {
        var servers = _redisDatabaseProvider.GetAllServers(asyncState) ?? Enumerable.Empty<IRedisServer>();
        return servers.Select(x => new SilentRedisServer(x, _loggerFactory.CreateLogger<SilentRedisServer>()));
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Taking of all Redis servers has been failed.");
        return Enumerable.Empty<ISilentRedisServer>();
      }
    }
  }
}
