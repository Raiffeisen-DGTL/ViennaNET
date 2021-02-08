using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ViennaNET.Redis.Implementation.Silent
{
  internal class SilentRedisDatabase : IRedisDatabase
  {
    private readonly IRedisDatabase _redisDatabase;
    private readonly ILogger _logger;

    public SilentRedisDatabase(IRedisDatabase redisDatabase, ILogger<SilentRedisDatabase> logger)
    {
      _redisDatabase = redisDatabase;
      _logger = logger;
    }

    public T ObjectGet<T>(string key, CommandFlags flags = default) where T : class
    {
      try
      {
        return _redisDatabase?.ObjectGet<T>(key, flags);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public List<T> ObjectGet<T>(List<string> keys, CommandFlags flags = default) where T : class
    {
      try
      {
        return _redisDatabase?.ObjectGet<T>(keys, flags);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public bool ObjectSet(
      string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase?.ObjectSet(key, value, expiry, when, flags) ?? false;
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public bool ObjectSet(string key, object value, string lifetime, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase?.ObjectSet(key, value, lifetime, when, flags) ?? false;
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public bool ObjectSet(Dictionary<string, object> values, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase?.ObjectSet(values, when, flags) ?? false;
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public async Task<T> ObjectGetAsync<T>(string key, CommandFlags flags = default) where T : class
    {
      try
      {
        return _redisDatabase == null
          ? null
          : await _redisDatabase.ObjectGetAsync<T>(key, flags)
                                .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public async Task<List<T>> ObjectGetAsync<T>(List<string> keys, CommandFlags flags = default) where T : class
    {
      try
      {
        return _redisDatabase == null
          ? null
          : await _redisDatabase.ObjectGetAsync<T>(keys, flags)
                                .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public async Task<bool> ObjectSetAsync(
      string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase != null && await _redisDatabase.ObjectSetAsync(key, value, expiry, when, flags)
                                                             .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public async Task<bool> ObjectSetAsync(
      string key, object value, string lifetime, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase != null && await _redisDatabase.ObjectSetAsync(key, value, lifetime, when, flags)
                                                             .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public async Task<bool> ObjectSetAsync(
      Dictionary<string, object> values, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase != null && await _redisDatabase.ObjectSetAsync(values, when, flags)
                                                             .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public string StringGet(string key, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase?.StringGet(key, flags);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public List<string> StringGet(List<string> keys, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase?.StringGet(keys, flags);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public bool StringSet(
      string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default, bool isDiagnostic = false)
    {
      try
      {
        return _redisDatabase?.StringSet(key, value, expiry, when, flags) ?? false;
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public bool StringSet(string key, string value, string lifetime, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase?.StringSet(key, value, lifetime, when, flags) ?? false;
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public bool StringSet(Dictionary<string, string> values, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase?.StringSet(values, when, flags) ?? false;
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public async Task<string> StringGetAsync(string key, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase == null
          ? null
          : await _redisDatabase.StringGetAsync(key, flags)
                                .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public async Task<List<string>> StringGetAsync(List<string> keys, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase == null
          ? null
          : await _redisDatabase.StringGetAsync(keys, flags)
                                .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public async Task<bool> StringSetAsync(
      string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase != null && await _redisDatabase.StringSetAsync(key, value, expiry, when, flags)
                                                             .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public async Task<bool> StringSetAsync(
      string key, string value, string lifetime, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase != null && await _redisDatabase.StringSetAsync(key, value, lifetime, when, flags)
                                                             .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public async Task<bool> StringSetAsync(
      Dictionary<string, string> values, When when = default, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase != null && await _redisDatabase.StringSetAsync(values, when, flags)
                                                             .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public bool KeyDelete(string key, CommandFlags flags = default, bool isDiagnostic = false)
    {
      try
      {
        return _redisDatabase?.KeyDelete(key, flags) ?? false;
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public long KeyDelete(List<string> keys, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase?.KeyDelete(keys, flags) ?? 0;
      }
      catch (Exception e)
      {
        LogError(e);
        return 0;
      }
    }

    public async Task<bool> KeyDeleteAsync(string key, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase != null && await _redisDatabase.KeyDeleteAsync(key, flags)
                                                             .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return false;
      }
    }

    public async Task<long> KeyDeleteAsync(List<string> keys, CommandFlags flags = default)
    {
      try
      {
        return _redisDatabase == null ? 0 : await _redisDatabase.KeyDeleteAsync(keys, flags)
                                                                .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return 0;
      }
    }

    private void LogError(Exception e)
    {
      _logger.LogError(e, "Action Redis has been failed.");
    }
  }
}
