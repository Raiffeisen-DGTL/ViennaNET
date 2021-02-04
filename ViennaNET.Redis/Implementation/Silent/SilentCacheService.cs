using System;
using Microsoft.Extensions.Logging;

namespace ViennaNET.Redis.Implementation.Silent
{
  /// <inheritdoc cref="ISilentCacheService"/> />
  public class SilentCacheService : CacheServiceBase, ISilentCacheService
  {
    private readonly ILogger _logger;

    /// <summary>
    /// Создаёт экземпляр класса
    /// </summary>
    /// <param name="redisDatabaseProvider">Провайдер БД</param>
    /// <param name="logger">Интерфейс логгирования</param>
    public SilentCacheService(IRedisDatabaseProvider redisDatabaseProvider, ILogger<SilentCacheService> logger) : base(
      redisDatabaseProvider)
    {
      _logger = logger;
    }

    private void LogError(Exception e)
    {
      _logger.LogError(e, "Action Redis has been failed.");
    }

    /// <inheritdoc />
    public bool TryGetObject<T>(string name, out T obj, params object[] keyIdentifier) where T : class
    {
      try
      {
        ThrowIfNoneKeyIdentifier(keyIdentifier);
        var redisKey = GetRedisKey(name, keyIdentifier);
        var redisValue = _redis.Value.ObjectGet<T>(redisKey);

        if (redisValue == null)
        {
          obj = default;
          return false;
        }

        obj = redisValue;
        return true;
      }
      catch (Exception e)
      {
        LogError(e);
        obj = default;
        return false;
      }
    }

    /// <inheritdoc />
    public void SetObject<T>(string name, string lifetime, T obj, params object[] keyIdentifier) where T : class
    {
      try
      {
        ThrowIfNoneKeyIdentifier(keyIdentifier);
        var redisKey = GetRedisKey(name, keyIdentifier);
        _redis.Value.ObjectSet(redisKey, obj, lifetime);
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    /// <inheritdoc />
    public void SetObject<T>(string name, TimeSpan? expiry, T obj, params object[] keyIdentifier) where T : class
    {
      try
      {
        ThrowIfNoneKeyIdentifier(keyIdentifier);
        var redisKey = GetRedisKey(name, keyIdentifier);
        _redis.Value.ObjectSet(redisKey, obj, expiry);
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }
  }
}