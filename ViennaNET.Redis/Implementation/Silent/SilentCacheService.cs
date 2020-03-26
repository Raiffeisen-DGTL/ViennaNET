using ViennaNET.Logging;
using System;

namespace ViennaNET.Redis.Implementation.Silent
{
  public class SilentCacheService : CacheServiceBase, ISilentCacheService
  {
    public SilentCacheService(IRedisDatabaseProvider redisDatabaseProvider) : base(redisDatabaseProvider) { }

    private static void LogError(Exception e)
    {
      Logger.LogErrorFormat(e, "Action Redis has been failed.");
    }

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