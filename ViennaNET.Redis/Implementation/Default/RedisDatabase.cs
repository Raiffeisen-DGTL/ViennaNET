using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Redis.Utils;
using ViennaNET.Utils;
using StackExchange.Redis;
using RedisException = ViennaNET.Redis.Exceptions.RedisException;

namespace ViennaNET.Redis.Implementation.Default
{
  internal class RedisDatabase : IRedisDatabase
  {
    private readonly IDatabase _database;
    private readonly IDictionary<string, TimeSpan> _keyLifetimes;
    private readonly ILogger _logger;
    private readonly string _prefixKey;
    private readonly bool _useCompression;

    public RedisDatabase(bool useCompression, IDatabase database, ILogger<RedisDatabase> logger,
      IDictionary<string, TimeSpan> keyLifetimes, string prefixKey)
    {
      _database = database.ThrowIfNull(nameof(database));
      _logger = logger.ThrowIfNull(nameof(logger));
      _prefixKey = prefixKey;
      _keyLifetimes = keyLifetimes;
      _useCompression = useCompression;
    }

    public T ObjectGet<T>(string key, CommandFlags flags = default) where T : class
    {
      LogDebug("ObjectGet", $"Key = {key}; Flags = {flags}");
      var stringValue = StringGetInternal(key, flags);
      return JsonUtils.DeserializeObject<T>(stringValue);
    }

    public List<T> ObjectGet<T>(List<string> keys, CommandFlags flags = default) where T : class
    {
      LogDebug("ObjectGet", $"Keys = {keys}; Flags = {flags}");
      var stringListValues = StringGetInternal(keys, flags);
      return stringListValues.Select(JsonUtils.DeserializeObject<T>)
                             .ToList();
    }

    public bool ObjectSet(
      string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var valueLength = GetValueLength(stringValue);
      LogDebug("ObjectSet", $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      return StringSetInternal(key, stringValue, expiry, when, flags);
    }

    public bool ObjectSet(
      string key, object value, string lifetime, When when = default, CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var valueLength = GetValueLength(stringValue);
      LogDebug("ObjectSet", $"Keys = {key}; Value-length = {valueLength}; Lifetime = {lifetime}; When = {when}; Flags = {flags}");
      return StringSetInternal(key, stringValue, lifetime, when, flags);
    }

    public bool ObjectSet(Dictionary<string, object> values, When when = default, CommandFlags flags = default)
    {
      LogDebug("ObjectSet", $"Values-count = {values.Count}; When = {when}; Flags = {flags}");
      var stringDictValues = new Dictionary<string, string>();
      foreach (var pair in values)
      {
        var stringValue = JsonUtils.SerializeObject(pair.Value);
        stringDictValues.Add(pair.Key, stringValue);
      }
      return StringSetInternal(stringDictValues, when, flags);
    }

    public async Task<T> ObjectGetAsync<T>(string key, CommandFlags flags = default) where T : class
    {
      var result = StringGetAsyncInternal(key, flags);
      LogDebug("ObjectGetAsync", $"Key = {key}; Flags = {flags}");
      var stringValue = await result;
      return JsonUtils.DeserializeObject<T>(stringValue);
    }

    public async Task<List<T>> ObjectGetAsync<T>(List<string> keys, CommandFlags flags = default) where T : class
    {
      var result = StringGetAsyncInternal(keys, flags);
      LogDebug("ObjectGetAsync", $"Keys = {keys}; Flags = {flags}");
      var stringListValues = await result;
      return stringListValues.Select(JsonUtils.DeserializeObject<T>)
                             .ToList();
    }

    public async Task<bool> ObjectSetAsync(
      string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var result = StringSetAsyncInternal(key, stringValue, expiry, when, flags);
      var valueLength = GetValueLength(stringValue);
      LogDebug("ObjectSetAsync", $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> ObjectSetAsync(
      string key, object value, string lifetime, When when = default, CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var result = StringSetAsyncInternal(key, stringValue, lifetime, when, flags);
      var valueLength = GetValueLength(stringValue);
      LogDebug("ObjectSetAsync", $"Keys = {key}; Value-length = {valueLength}; Lifetime = {lifetime}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> ObjectSetAsync(
      Dictionary<string, object> values, When when = default, CommandFlags flags = default)
    {
      var stringDictValues = new Dictionary<string, string>();
      foreach (var pair in values)
      {
        var stringValue = JsonUtils.SerializeObject(pair.Value);
        stringDictValues.Add(pair.Key, stringValue);
      }
      var result = StringSetAsyncInternal(stringDictValues, when, flags);
      LogDebug("ObjectSetAsync", $"Values-count = {values.Count}; When = {when}; Flags = {flags}");

      return await result;
    }

    public string StringGet(string key, CommandFlags flags = default)
    {
      LogDebug("StringGet", $"Key = {key}; Flags = {flags}");
      return StringGetInternal(key, flags);
    }

    public List<string> StringGet(List<string> keys, CommandFlags flags = default)
    {
      LogDebug("StringGet", $"Keys = {keys}; Flags = {flags}");
      return StringGetInternal(keys, flags);
    }

    public bool StringSet(
      string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default,
      bool isDiagnostic = false)
    {
      var valueLength = GetValueLength(value);
      if (isDiagnostic)
      {
        LogDiagnostic("StringSet", $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      }
      else
      {
        LogDebug("StringSet", $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      }

      return StringSetInternal(key, value, expiry, when, flags);
    }

    public bool StringSet(
      string key, string value, string lifetime, When when = default, CommandFlags flags = default)
    {
      var valueLength = GetValueLength(value);
      LogDebug("StringSet", $"Keys = {key}; Value-length = {valueLength}; Lifetime = {lifetime}; When = {when}; Flags = {flags}");
      return StringSetInternal(key, value, lifetime, when, flags);
    }

    public bool StringSet(Dictionary<string, string> values, When when = default, CommandFlags flags = default)
    {
      LogDebug("StringSet", $"Values-count = {values.Count}; When = {when}; Flags = {flags}");
      return StringSetInternal(values, when, flags);
    }

    public async Task<string> StringGetAsync(string key, CommandFlags flags = default)
    {
      var result = StringGetAsyncInternal(key, flags);
      LogDebug("StringGetAsync", $"Key = {key}; Flags = {flags}");
      return await result;
    }

    public async Task<List<string>> StringGetAsync(List<string> keys, CommandFlags flags = default)
    {
      var result = StringGetAsyncInternal(keys, flags);
      LogDebug("StringGetAsync", $"Keys = {keys}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> StringSetAsync(
      string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      var result = StringSetAsyncInternal(key, value, expiry, when, flags);
      var valueLength = GetValueLength(value);
      LogDebug("StringSetAsync", $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> StringSetAsync(
      string key, string value, string lifetime, When when = default, CommandFlags flags = default)
    {
      var result = StringSetAsyncInternal(key, value, lifetime, when, flags);
      var valueLength = GetValueLength(value);
      LogDebug("StringSetAsync", $"Keys = {key}; Value-length = {valueLength}; Lifetime = {lifetime}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> StringSetAsync(
      Dictionary<string, string> values, When when = default, CommandFlags flags = default)
    {
      var result = StringSetAsyncInternal(values, when, flags);
      LogDebug("StringSetAsync", $"Values-count = {values.Count}; When = {when}; Flags = {flags}");
      return await result;
    }

    public bool KeyDelete(string key, CommandFlags flags = default, bool isDiagnostic = false)
    {
      if (isDiagnostic)
      {
        LogDiagnostic("KeyDelete", $"Key = {key}; Flags = {flags}");
      }
      else
      {
        LogDebug("KeyDelete", $"Key = {key}; Flags = {flags}");
      }

      return _database.KeyDelete(GetKeyName(key), flags);
    }

    public long KeyDelete(List<string> keys, CommandFlags flags = default)
    {
      LogDebug("KeyDelete", $"Keys = {keys}; Flags = {flags}");
      return _database.KeyDelete(keys.Select(k => (RedisKey)GetKeyName(k)).ToArray(), flags);
    }

    public async Task<bool> KeyDeleteAsync(string key, CommandFlags flags = default)
    {
      LogDebug("KeyDeleteAsync", $"Key = {key}; Flags = {flags}");
      return await _database.KeyDeleteAsync(GetKeyName(key), flags);
    }

    public async Task<long> KeyDeleteAsync(List<string> keys, CommandFlags flags = default)
    {
      LogDebug("KeyDeleteAsync", $"Keys = {keys}; Flags = {flags}");
      return await _database.KeyDeleteAsync(keys.Select(k => (RedisKey)GetKeyName(k)).ToArray(), flags);
    }

    private void LogDebug(string action, string arguments)
    {
      _logger.LogDebug("Action Redis: {action}. Arguments: {arguments}.", action, arguments);
    }

    private void LogDiagnostic(string action, string arguments)
    {
      _logger.LogTrace("Action Redis: {action}. Arguments: {arguments}.", action, arguments);
    }

    private string GetDecompressedString(string data)
    {
      return _useCompression ? CompressUtils.DecompressString(data) : data;
    }

    private string SetCompressedString(string data)
    {
      return _useCompression ? CompressUtils.CompressString(data) : data;
    }

    private string GetKeyName(string key)
    {
      return _prefixKey + key;
    }

    private string StringGetInternal(string key, CommandFlags flags = default)
    {
      var result = _database.StringGet(GetKeyName(key), flags);
      return GetDecompressedString(result);
    }

    private List<string> StringGetInternal(IEnumerable<string> keys, CommandFlags flags = default)
    {
      var result = _database.StringGet(keys.Select(k => (RedisKey)GetKeyName(k))
                                           .ToArray(), flags);
      return result.Select(v => GetDecompressedString(v))
                   .ToList();
    }

    private bool StringSetInternal(
      string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      return _database.StringSet(GetKeyName(key), SetCompressedString(value), expiry, when, flags);
    }

    private bool StringSetInternal(
      string key, string value, string lifetime, When when = default, CommandFlags flags = default)
    {
      return _database.StringSet(GetKeyName(key), SetCompressedString(value), GetKeyLifetime(lifetime), when, flags);
    }

    private bool StringSetInternal(
      Dictionary<string, string> values, When when = default, CommandFlags flags = default)
    {
      var arrayValues = values.Select(p => new KeyValuePair<RedisKey, RedisValue>(GetKeyName(p.Key), SetCompressedString(p.Value)))
                              .ToArray();
      return _database.StringSet(arrayValues, when, flags);
    }

    private async Task<string> StringGetAsyncInternal(string key, CommandFlags flags = default)
    {
      var result = await _database.StringGetAsync(GetKeyName(key), flags);
      return GetDecompressedString(result);
    }

    private async Task<List<string>> StringGetAsyncInternal(IEnumerable<string> keys, CommandFlags flags = default)
    {
      var result = await _database.StringGetAsync(keys.Select(k => (RedisKey)GetKeyName(k))
                                                      .ToArray(), flags);
      return result.Select(v => GetDecompressedString(v))
                   .ToList();
    }

    private async Task<bool> StringSetAsyncInternal(
      string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      return await _database.StringSetAsync(GetKeyName(key), SetCompressedString(value), expiry, when, flags);
    }

    private async Task<bool> StringSetAsyncInternal(
      string key, string value, string lifetime, When when = default, CommandFlags flags = default)
    {
      return await _database.StringSetAsync(GetKeyName(key), SetCompressedString(value), GetKeyLifetime(lifetime), when, flags);
    }

    private async Task<bool> StringSetAsyncInternal(
      Dictionary<string, string> values, When when = default, CommandFlags flags = default)
    {
      var arrayValues = values.Select(p => new KeyValuePair<RedisKey, RedisValue>(GetKeyName(p.Key), SetCompressedString(p.Value)))
                              .ToArray();
      return await _database.StringSetAsync(arrayValues, when, flags);
    }

    private static int GetValueLength(string value)
    {
      return value?.Length ?? 0;
    }

    private TimeSpan GetKeyLifetime(string lifetime)
    {
      if (!_keyLifetimes.TryGetValue(lifetime, out var expiry))
      {
        throw new RedisException($"Lifetime with name {lifetime} could not be found. Check an app.config file of the executing assembly");
      }

      return expiry;
    }
  }
}
