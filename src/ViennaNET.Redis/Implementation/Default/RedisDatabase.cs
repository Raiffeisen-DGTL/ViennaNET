using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using ViennaNET.Redis.Utils;
using ViennaNET.Utils;
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

    public Collection<T> ObjectGet<T>(IEnumerable<string> keys, CommandFlags flags = default) where T : class
    {
      LogDebug("ObjectGet", $"Keys = {keys}; Flags = {flags}");
      var stringListValues = StringGetInternal(keys, flags);
      return new Collection<T>(stringListValues.Select(JsonUtils.DeserializeObject<T>).ToList());
    }

    public T HashObjectGet<T>(string key, string field, CommandFlags flags = default) where T : class
    {
      LogDebug(nameof(HashObjectGet), $"Key = {key}; Field = {field}; Flags = {flags}");
      var stringValue = HashStringGetInternal(key, field, flags);
      return JsonUtils.DeserializeObject<T>(stringValue);
    }

    public Collection<T> HashObjectGet<T>(string key, IEnumerable<string> fields, CommandFlags flags = default)
      where T : class
    {
      LogDebug(nameof(HashObjectGet), $"Key = {key}; Fields = {fields}; Flags = {flags}");
      var stringListValues = HashStringGetInternal(key, fields, flags);
      return new Collection<T>(stringListValues.Select(JsonUtils.DeserializeObject<T>).ToList());
    }

    public Dictionary<string, T> HashObjectGetAll<T>(string key, CommandFlags flags = default) where T : class
    {
      LogDebug(nameof(HashObjectGetAll), $"Key = {key}; Flags = {flags}");
      var stringDictionary = HashStringGetAllInternal(key, flags);
      return stringDictionary.ToDictionary(kvp => kvp.Key, kvp => JsonUtils.DeserializeObject<T>(kvp.Value));
    }

    public bool ObjectSet(string key, object value, TimeSpan? expiry = null, When when = default,
      CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var valueLength = GetValueLength(stringValue);
      LogDebug("ObjectSet",
        $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      return StringSetInternal(key, stringValue, expiry, when, flags);
    }

    public bool ObjectSet(string key, object value, string lifetime, When when = default, CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var valueLength = GetValueLength(stringValue);
      LogDebug("ObjectSet",
        $"Keys = {key}; Value-length = {valueLength}; Lifetime = {lifetime}; When = {when}; Flags = {flags}");
      return StringSetInternal(key, stringValue, lifetime, when, flags);
    }

    public bool ObjectSet(IDictionary<string, object> values, When when = default, CommandFlags flags = default)
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

    public bool HashObjectSet(string key, string field, object value, When when = default, CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var valueLength = GetValueLength(stringValue);
      LogDebug(nameof(HashObjectSet),
        $"Key = {key}; Field = {field}; Value-length = {valueLength}; When = {when}; Flags = {flags}");
      return HashStringSetInternal(key, field, stringValue, when, flags);
    }

    public void HashObjectSet(string key, IDictionary<string, object> values, CommandFlags flags = default)
    {
      LogDebug(nameof(HashObjectSet), $"Key = {key}; Values-count = {values.Count}; Flags = {flags}");
      var stringDictValues = new Dictionary<string, string>();
      foreach (var pair in values)
      {
        var stringValue = JsonUtils.SerializeObject(pair.Value);
        stringDictValues.Add(pair.Key, stringValue);
      }

      HashStringSetInternal(key, stringDictValues, flags);
    }

    public async Task<T> ObjectGetAsync<T>(string key, CommandFlags flags = default) where T : class
    {
      var result = StringGetInternalAsync(key, flags).ConfigureAwait(false);
      LogDebug("ObjectGetAsync", $"Key = {key}; Flags = {flags}");
      var stringValue = await result;
      return JsonUtils.DeserializeObject<T>(stringValue);
    }

    public async Task<Collection<T>> ObjectGetAsync<T>(IEnumerable<string> keys, CommandFlags flags = default)
      where T : class
    {
      var result = StringGetInternalAsync(keys, flags).ConfigureAwait(false);
      LogDebug("ObjectGetAsync", $"Keys = {keys}; Flags = {flags}");
      var stringListValues = await result;
      return new Collection<T>(stringListValues.Select(JsonUtils.DeserializeObject<T>).ToList());
    }

    public async Task<T> HashObjectGetAsync<T>(string key, string field, CommandFlags flags = default) where T : class
    {
      var result = HashStringGetInternalAsync(key, field, flags).ConfigureAwait(false);
      LogDebug(nameof(HashObjectGetAsync), $"Key = {key}; Field = {field}; Flags = {flags}");
      var stringValue = await result;
      return JsonUtils.DeserializeObject<T>(stringValue);
    }

    public async Task<Collection<T>> HashObjectGetAsync<T>(string key, IEnumerable<string> fields,
      CommandFlags flags = default) where T : class
    {
      var result = HashStringGetInternalAsync(key, fields, flags);
      LogDebug(nameof(HashObjectGetAsync), $"Key = {key}; Fields = {fields}; Flags = {flags}");
      var stringListValues = await result;
      return new Collection<T>(stringListValues.Select(JsonUtils.DeserializeObject<T>).ToList());
    }

    public async Task<Dictionary<string, T>> HashObjectGetAllAsync<T>(string key, CommandFlags flags = default)
      where T : class
    {
      LogDebug(nameof(HashObjectGetAllAsync), $"Key = {key}; Flags = {flags}");
      var stringDictionary = await HashStringGetAllInternalAsync(key, flags).ConfigureAwait(false);
      return stringDictionary.ToDictionary(kvp => kvp.Key, kvp => JsonUtils.DeserializeObject<T>(kvp.Value));
    }

    public async Task<bool> ObjectSetAsync(
      string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var result = StringSetInternalAsync(key, stringValue, expiry, when, flags);
      var valueLength = GetValueLength(stringValue);
      LogDebug("ObjectSetAsync",
        $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> ObjectSetAsync(
      string key, object value, string lifetime, When when = default, CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var result = StringSetInternalAsync(key, stringValue, lifetime, when, flags);
      var valueLength = GetValueLength(stringValue);
      LogDebug("ObjectSetAsync",
        $"Keys = {key}; Value-length = {valueLength}; Lifetime = {lifetime}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> ObjectSetAsync(IDictionary<string, object> values, When when = default,
      CommandFlags flags = default)
    {
      var stringDictValues = new Dictionary<string, string>();
      foreach (var pair in values)
      {
        var stringValue = JsonUtils.SerializeObject(pair.Value);
        stringDictValues.Add(pair.Key, stringValue);
      }

      var result = StringSetInternalAsync(stringDictValues, when, flags);
      LogDebug("ObjectSetAsync", $"Values-count = {values.Count}; When = {when}; Flags = {flags}");

      return await result;
    }

    public async Task<bool> HashObjectSetAsync(string key, string field, object value,
      When when = default, CommandFlags flags = default)
    {
      var stringValue = JsonUtils.SerializeObject(value);
      var result = HashStringSetInternalAsync(key, field, stringValue, when, flags).ConfigureAwait(false);
      var valueLength = GetValueLength(stringValue);
      LogDebug("ObjectSetAsync",
        $"Key = {key}; Field = {field}; Value-length = {valueLength};  When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task HashObjectSetAsync(string key, IDictionary<string, object> values, CommandFlags flags = default)
    {
      var stringDictValues = new Dictionary<string, string>();
      foreach (var pair in values)
      {
        var stringValue = JsonUtils.SerializeObject(pair.Value);
        stringDictValues.Add(pair.Key, stringValue);
      }

      var result = HashStringSetInternalAsync(key, stringDictValues, flags).ConfigureAwait(false);
      LogDebug("ObjectSetAsync", $"Key = {key}; Values-count = {values.Count}; Flags = {flags}");

      await result;
    }

    public string StringGet(string key, CommandFlags flags = default)
    {
      LogDebug("StringGet", $"Key = {key}; Flags = {flags}");
      return StringGetInternal(key, flags);
    }

    public Collection<string> StringGet(IEnumerable<string> keys, CommandFlags flags = default)
    {
      LogDebug("StringGet", $"Keys = {keys}; Flags = {flags}");
      return StringGetInternal(keys, flags);
    }

    public string HashStringGet(string key, string field, CommandFlags flags = default)
    {
      LogDebug(nameof(HashStringGet), $"Key = {key}; Field = {field}; Flags = {flags}");
      return HashStringGetInternal(key, field, flags);
    }

    public Collection<string> HashStringGet(string key, IEnumerable<string> fields, CommandFlags flags = default)
    {
      LogDebug(nameof(HashStringGet), $"Key = {key}; Fields = {fields}; Flags = {flags}");
      return HashStringGetInternal(key, fields, flags);
    }

    public Dictionary<string, string> HashStringGetAll(string key, CommandFlags flags = default)
    {
      LogDebug(nameof(HashStringGetAll), $"Key = {key}; Flags = {flags}");
      return HashStringGetAllInternal(key, flags);
    }

    public bool StringSet(string key, string value, TimeSpan? expiry = null,
      When when = default, CommandFlags flags = default, bool isDiagnostic = false)
    {
      var valueLength = GetValueLength(value);
      if (isDiagnostic)
      {
        LogDiagnostic("StringSet",
          $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      }
      else
      {
        LogDebug("StringSet",
          $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      }

      return StringSetInternal(key, value, expiry, when, flags);
    }

    public bool StringSet(
      string key, string value, string lifetime, When when = default, CommandFlags flags = default)
    {
      var valueLength = GetValueLength(value);
      LogDebug("StringSet",
        $"Keys = {key}; Value-length = {valueLength}; Lifetime = {lifetime}; When = {when}; Flags = {flags}");
      return StringSetInternal(key, value, lifetime, when, flags);
    }

    public bool StringSet(IDictionary<string, string> values, When when = default, CommandFlags flags = default)
    {
      LogDebug("StringSet", $"Values-count = {values.Count}; When = {when}; Flags = {flags}");
      return StringSetInternal(values, when, flags);
    }

    public bool HashStringSet(string key, string field, string value, When when = default, CommandFlags flags = default)
    {
      var valueLength = GetValueLength(value);
      LogDebug(nameof(HashStringSet),
        $"Key = {key}; Field = {field}; Value-length = {valueLength}; When = {when}; Flags = {flags}");
      return HashStringSetInternal(key, field, value, when, flags);
    }

    public void HashStringSet(string key, IDictionary<string, string> values, CommandFlags flags = default)
    {
      LogDebug(nameof(HashStringSet), $"Key = {key}; Values-count = {values.Count}; Flags = {flags}");
      HashStringSetInternal(key, values, flags);
    }

    public async Task<string> StringGetAsync(string key, CommandFlags flags = default)
    {
      var result = StringGetInternalAsync(key, flags);
      LogDebug("StringGetAsync", $"Key = {key}; Flags = {flags}");
      return await result;
    }

    public async Task<Collection<string>> StringGetAsync(IEnumerable<string> keys, CommandFlags flags = default)
    {
      var result = StringGetInternalAsync(keys, flags).ConfigureAwait(false);
      LogDebug("StringGetAsync", $"Keys = {keys}; Flags = {flags}");
      return await result;
    }

    public async Task<string> HashStringGetAsync(string key, string field, CommandFlags flags = default)
    {
      var result = HashStringGetInternalAsync(key, field, flags).ConfigureAwait(false);
      LogDebug(nameof(HashStringGetAsync), $"Key = {key}; Field = {field}; Flags = {flags}");
      return await result;
    }

    public async Task<Collection<string>> HashStringGetAsync(string key, IEnumerable<string> fields,
      CommandFlags flags = default)
    {
      var result = HashStringGetInternalAsync(key, fields, flags).ConfigureAwait(false);
      LogDebug(nameof(HashStringGetAsync), $"Key = {key}; Fields = {fields}; Flags = {flags}");
      return await result;
    }

    public async Task<Dictionary<string, string>> HashStringGetAllAsync(string key, CommandFlags flags = default)
    {
      var result = HashStringGetAllInternalAsync(key, flags);
      LogDebug(nameof(HashStringGetAllAsync), $"Key = {key}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> StringSetAsync(
      string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      var result = StringSetInternalAsync(key, value, expiry, when, flags);
      var valueLength = GetValueLength(value);
      LogDebug("StringSetAsync",
        $"Keys = {key}; Value-length = {valueLength}; Expiry = {expiry}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> StringSetAsync(
      string key, string value, string lifetime, When when = default, CommandFlags flags = default)
    {
      var result = StringSetInternalAsync(key, value, lifetime, when, flags);
      var valueLength = GetValueLength(value);
      LogDebug("StringSetAsync",
        $"Keys = {key}; Value-length = {valueLength}; Lifetime = {lifetime}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> StringSetAsync(IDictionary<string, string> values,
      When when = default, CommandFlags flags = default)
    {
      var result = StringSetInternalAsync(values, when, flags);
      LogDebug("StringSetAsync", $"Values-count = {values.Count}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task<bool> HashStringSetAsync(string key, string field, string value, When when = default,
      CommandFlags flags = default)
    {
      var result = HashStringSetInternalAsync(key, field, value, when, flags).ConfigureAwait(false);
      var valueLength = GetValueLength(value);
      LogDebug(nameof(HashStringSetAsync),
        $"Key = {key}; Field = {field}; Value-length = {valueLength}; When = {when}; Flags = {flags}");
      return await result;
    }

    public async Task HashStringSetAsync(string key, IDictionary<string, string> values, CommandFlags flags = default)
    {
      var result = HashStringSetInternalAsync(key, values, flags).ConfigureAwait(false);
      LogDebug(nameof(HashStringSetAsync), $"Key = {key}; Values-count = {values.Count}; Flags = {flags}");
      await result;
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

    public long KeyDelete(IEnumerable<string> keys, CommandFlags flags = default)
    {
      LogDebug("KeyDelete", $"Keys = {keys}; Flags = {flags}");
      return _database.KeyDelete(keys.Select(k => (RedisKey)GetKeyName(k)).ToArray(), flags);
    }

    public async Task<bool> KeyDeleteAsync(string key, CommandFlags flags = default)
    {
      LogDebug("KeyDeleteAsync", $"Key = {key}; Flags = {flags}");
      return await _database.KeyDeleteAsync(GetKeyName(key), flags);
    }

    public async Task<long> KeyDeleteAsync(IEnumerable<string> keys, CommandFlags flags = default)
    {
      LogDebug("KeyDeleteAsync", $"Keys = {keys}; Flags = {flags}");
      return await _database.KeyDeleteAsync(keys.Select(k => (RedisKey)GetKeyName(k)).ToArray(), flags);
    }

    public bool HashDelete(string key, string field, CommandFlags flags = default)
    {
      LogDebug(nameof(HashDelete), $"Key = {key}; Field = {field}; Flags = {flags}");

      return _database.HashDelete(GetKeyName(key), field, flags);
    }

    public long HashDelete(string key, IEnumerable<string> fields, CommandFlags flags = default)
    {
      LogDebug(nameof(HashDelete), $"Key = {key}; Fields = {fields}; Flags = {flags}");
      return _database.HashDelete(GetKeyName(key), fields.Select(f => (RedisValue)f).ToArray(), flags);
    }

    public async Task<bool> HashDeleteAsync(string key, string field, CommandFlags flags = default)
    {
      var result = _database.HashDeleteAsync(GetKeyName(key), field, flags).ConfigureAwait(false);
      LogDebug(nameof(HashDeleteAsync), $"Key = {key}; Field = {field}; Flags = {flags}");
      return await result;
    }

    public async Task<long> HashDeleteAsync(string key, IEnumerable<string> fields, CommandFlags flags = default)
    {
      var result = _database.HashDeleteAsync(GetKeyName(key), fields.Select(f => (RedisValue)f).ToArray(), flags)
        .ConfigureAwait(false);
      LogDebug(nameof(HashDeleteAsync), $"Key = {key}; Fields = {fields}; Flags = {flags}");
      return await result;
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

    private Collection<string> StringGetInternal(IEnumerable<string> keys, CommandFlags flags = default)
    {
      var result = _database.StringGet(keys.Select(k => (RedisKey)GetKeyName(k))
        .ToArray(), flags);
      return new Collection<string>(result.Select(v => GetDecompressedString(v)).ToList());
    }

    private string HashStringGetInternal(string key, string field, CommandFlags flags = default)
    {
      var result = _database.HashGet(GetKeyName(key), field, flags);
      return GetDecompressedString(result);
    }

    private Collection<string> HashStringGetInternal(string key, IEnumerable<string> fields,
      CommandFlags flags = default)
    {
      var result = _database.HashGet(GetKeyName(key), fields.Select(f => (RedisValue)f).ToArray(), flags);
      return new Collection<string>(result.Select(v => GetDecompressedString(v)).ToList());
    }

    private Dictionary<string, string> HashStringGetAllInternal(string key, CommandFlags flags = default)
    {
      var result = _database.HashGetAll(GetKeyName(key), flags);
      return result.ToDictionary(entry => entry.Name.ToString(), entry => GetDecompressedString(entry.Value));
    }

    private async Task<Dictionary<string, string>> HashStringGetAllInternalAsync(string key,
      CommandFlags flags = default)
    {
      var result = await _database.HashGetAllAsync(GetKeyName(key), flags).ConfigureAwait(false);
      return result.ToDictionary(entry => entry.Name.ToString(), entry => GetDecompressedString(entry.Value));
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
      IDictionary<string, string> values, When when = default, CommandFlags flags = default)
    {
      var arrayValues = values.Select(p =>
          new KeyValuePair<RedisKey, RedisValue>(GetKeyName(p.Key), SetCompressedString(p.Value)))
        .ToArray();
      return _database.StringSet(arrayValues, when, flags);
    }

    private bool HashStringSetInternal(string key,
      string field,
      string value,
      When when = default,
      CommandFlags flags = default)
    {
      return _database.HashSet(GetKeyName(key), field, value, when, flags);
    }

    private void HashStringSetInternal(string key, IDictionary<string, string> values, CommandFlags flags = default)
    {
      var arrayValues = values.Select(p => new HashEntry(p.Key, p.Value))
        .ToArray();
      _database.HashSet(GetKeyName(key), arrayValues, flags);
    }

    private async Task<string> StringGetInternalAsync(string key, CommandFlags flags = default)
    {
      var result = await _database.StringGetAsync(GetKeyName(key), flags);
      return GetDecompressedString(result);
    }

    private async Task<Collection<string>> StringGetInternalAsync(IEnumerable<string> keys,
      CommandFlags flags = default)
    {
      var result = await _database.StringGetAsync(keys.Select(k => (RedisKey)GetKeyName(k))
        .ToArray(), flags);
      return new Collection<string>(result.Select(v => GetDecompressedString(v)).ToList());
    }

    private async Task<string> HashStringGetInternalAsync(string key, string field, CommandFlags flags = default)
    {
      var result = _database.HashGetAsync(GetKeyName(key), field, flags).ConfigureAwait(false);
      return GetDecompressedString(await result);
    }

    private async Task<Collection<string>> HashStringGetInternalAsync(string key, IEnumerable<string> fields,
      CommandFlags flags = default)
    {
      var result = await _database.HashGetAsync(GetKeyName(key), fields.Select(f => (RedisValue)f).ToArray(), flags)
        .ConfigureAwait(false);
      return new Collection<string>(result.Select(v => GetDecompressedString(v)).ToList());
    }

    private async Task<bool> StringSetInternalAsync(
      string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default)
    {
      return await _database.StringSetAsync(GetKeyName(key), SetCompressedString(value), expiry, when, flags);
    }

    private async Task<bool> StringSetInternalAsync(
      string key, string value, string lifetime, When when = default, CommandFlags flags = default)
    {
      return await _database.StringSetAsync(GetKeyName(key), SetCompressedString(value), GetKeyLifetime(lifetime), when,
        flags);
    }

    private async Task<bool> StringSetInternalAsync(
      IDictionary<string, string> values, When when = default, CommandFlags flags = default)
    {
      var arrayValues = values.Select(p =>
          new KeyValuePair<RedisKey, RedisValue>(GetKeyName(p.Key), SetCompressedString(p.Value)))
        .ToArray();
      return await _database.StringSetAsync(arrayValues, when, flags);
    }

    private async Task<bool> HashStringSetInternalAsync(string key, string field, string value,
      When when = default, CommandFlags commandFlags = default)
    {
      var result = _database.HashSetAsync(GetKeyName(key), field, value, when, commandFlags).ConfigureAwait(false);
      return await result;
    }

    private async Task HashStringSetInternalAsync(string key, IDictionary<string, string> values,
      CommandFlags flags = default)
    {
      var arrayValues = values.Select(kvp => new HashEntry(kvp.Key, SetCompressedString(kvp.Value)))
        .ToArray();
      await _database.HashSetAsync(GetKeyName(key), arrayValues, flags).ConfigureAwait(false);
    }

    private static int GetValueLength(string value)
    {
      return value?.Length ?? 0;
    }

    private TimeSpan GetKeyLifetime(string lifetime)
    {
      if (!_keyLifetimes.TryGetValue(lifetime, out var expiry))
      {
        throw new RedisException(
          $"Lifetime with name {lifetime} could not be found. Check an app.config file of the executing assembly");
      }

      return expiry;
    }
  }
}