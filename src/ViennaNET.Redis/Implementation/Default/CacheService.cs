using System;

namespace ViennaNET.Redis.Implementation.Default
{
  public class CacheService : CacheServiceBase, ICacheService
  {
    public CacheService(IRedisDatabaseProvider redisDatabaseProvider) : base(redisDatabaseProvider) { }

    public bool TryGetObject<T>(string name, out T obj, params object[] keyIdentifier) where T : class
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

    public void SetObject<T>(string name, string lifetime, T obj, params object[] keyIdentifier) where T : class
    {
      ThrowIfNoneKeyIdentifier(keyIdentifier);
      var redisKey = GetRedisKey(name, keyIdentifier);
      _redis.Value.ObjectSet(redisKey, obj, lifetime);
    }

    public void SetObject<T>(string name, TimeSpan? expiry, T obj, params object[] keyIdentifier) where T : class
    {
      ThrowIfNoneKeyIdentifier(keyIdentifier);
      var redisKey = GetRedisKey(name, keyIdentifier);
      _redis.Value.ObjectSet(redisKey, obj, expiry);
    }
  }
}