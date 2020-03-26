using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ViennaNET.Logging;

namespace ViennaNET.Redis.Implementation.Silent
{
  /// <inheritdoc />
  public class SilentRedisDatabaseProvider : ISilentRedisDatabaseProvider
  {
    private readonly IRedisDatabaseProvider _redisDatabaseProvider;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="IConnectionConfiguration" />
    /// </summary>
    /// <param name="redisDatabaseProvider">Провайдер конфигурации</param>
    public SilentRedisDatabaseProvider(IRedisDatabaseProvider redisDatabaseProvider)
    {
      _redisDatabaseProvider = redisDatabaseProvider;
    }

    /// <inheritdoc />
    public IRedisDatabase GetDatabase(bool useCompression = false, int database = 0, object asyncState = null)
    {
      try
      {
        var db = _redisDatabaseProvider.GetDatabase(useCompression, database, asyncState);
        return new SilentRedisDatabase(db);
      }
      catch (Exception e)
      {
        Logger.LogErrorFormat(e, "Taking of the Redis database has been failed.");
        return new SilentRedisDatabase(null);
      }
    }

    /// <inheritdoc />
    public ISilentRedisServer GetServer(EndPoint endPoint, object asyncState = null)
    {
      try
      {
        var server = _redisDatabaseProvider.GetServer(endPoint, asyncState);
        return new SilentRedisServer(server);
      }
      catch (Exception e)
      {
        Logger.LogErrorFormat(e, "Taking of the Redis server has been failed.");
        return new SilentRedisServer(null);
      }
    }

    /// <inheritdoc />
    public IEnumerable<ISilentRedisServer> GetAllServers(object asyncState = null)
    {
      try
      {
        var servers = _redisDatabaseProvider.GetAllServers(asyncState) ?? Enumerable.Empty<IRedisServer>();
        return servers.Select(x => new SilentRedisServer(x));
      }
      catch (Exception e)
      {
        Logger.LogErrorFormat(e, "Taking of all Redis servers has been failed.");
        return Enumerable.Empty<ISilentRedisServer>();
      }
    }
  }
}
