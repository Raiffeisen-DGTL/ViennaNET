using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public T ObjectGet<T>(string key, CommandFlags flags = default) where T : class =>
      TryAct(db => db.ObjectGet<T>(key, flags));

    public Collection<T> ObjectGet<T>(IEnumerable<string> keys, CommandFlags flags = default) where T : class =>
      TryAct(db => db.ObjectGet<T>(keys, flags));

    public bool ObjectSet(string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default) =>
      TryAct(db => db.ObjectSet(key, value, expiry, when, flags));

    public bool ObjectSet(string key, object value, string lifetime, When when = default, CommandFlags flags = default) =>
      TryAct(db => db.ObjectSet(key, value, lifetime, when, flags));

    public bool ObjectSet(IDictionary<string, object> values, When when = default, CommandFlags flags = default) =>
      TryAct(db => db.ObjectSet(values, when, flags));

    public Task<T> ObjectGetAsync<T>(string key, CommandFlags flags = default) where T : class =>
      TryActAsync(db => db.ObjectGetAsync<T>(key, flags));

    public Task<Collection<T>> ObjectGetAsync<T>(IEnumerable<string> keys, CommandFlags flags = default) where T : class =>
      TryActAsync(db => db.ObjectGetAsync<T>(keys, flags));

    public Task<bool> ObjectSetAsync(string key, object value, TimeSpan? expiry = null, 
      When when = default, CommandFlags flags = default) =>
      TryActAsync(db => db.ObjectSetAsync(key, value, expiry, when, flags));

    public Task<bool> ObjectSetAsync(
      string key, object value, string lifetime, When when = default, CommandFlags flags = default) =>
      TryAct(db => db.ObjectSetAsync(key, value, lifetime, when, flags));

    public Task<bool> ObjectSetAsync(
      IDictionary<string, object> values, When when = default, CommandFlags flags = default) =>
      TryActAsync(db => db.ObjectSetAsync(values, when, flags));

    public string StringGet(string key, CommandFlags flags = default) =>
      TryAct(db => db.StringGet(key, flags));

    public Collection<string> StringGet(IEnumerable<string> keys, CommandFlags flags = default) =>
      TryAct(db => db.StringGet(keys, flags));

    public bool StringSet(string key, string value, TimeSpan? expiry = null, 
      When when = default, CommandFlags flags = default, bool isDiagnostic = false) =>
      TryAct(db => db.StringSet(key, value, expiry, when, flags));

    public bool StringSet(string key, string value, string lifetime, When when = default, CommandFlags flags = default) =>
      TryAct(db => db.StringSet(key, value, lifetime, when, flags));

    public bool StringSet(IDictionary<string, string> values, When when = default, CommandFlags flags = default) =>
      TryAct(db => db.StringSet(values, when, flags));

    public Task<string> StringGetAsync(string key, CommandFlags flags = default) =>
      TryActAsync(db => db.StringGetAsync(key, flags));

    public Task<Collection<string>> StringGetAsync(IEnumerable<string> keys, CommandFlags flags = default) =>
      TryActAsync(db => db.StringGetAsync(keys, flags));

    public Task<bool> StringSetAsync(
      string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default) =>
      TryActAsync(db => db.StringSetAsync(key, value, expiry, when, flags));

    public Task<bool> StringSetAsync(
      string key, string value, string lifetime, When when = default, CommandFlags flags = default) =>
      TryActAsync(db => db.StringSetAsync(key, value, lifetime, when, flags));

    public Task<bool> StringSetAsync(
      IDictionary<string, string> values, When when = default, CommandFlags flags = default) =>
      TryActAsync(db => db.StringSetAsync(values, when, flags));

    public bool KeyDelete(string key, CommandFlags flags = default, bool isDiagnostic = false) =>
      TryAct(db => db.KeyDelete(key, flags));

    public long KeyDelete(IEnumerable<string> keys, CommandFlags flags = default) =>
      TryAct(db => db.KeyDelete(keys, flags));

    public Task<bool> KeyDeleteAsync(string key, CommandFlags flags = default) =>
      TryActAsync(db => db.KeyDeleteAsync(key, flags));

    public Task<long> KeyDeleteAsync(IEnumerable<string> keys, CommandFlags flags = default) =>
      TryActAsync(db => db.KeyDeleteAsync(keys, flags));    

    public T HashObjectGet<T>(string key, string field, CommandFlags flags = CommandFlags.None) where T : class =>
      TryAct(db => db.HashObjectGet<T>(key, field, flags));

    public Collection<T> HashObjectGet<T>(string key, IEnumerable<string> fields, 
      CommandFlags flags = CommandFlags.None) where T : class =>
      TryAct(db => db.HashObjectGet<T>(key, fields, flags));

    public bool HashObjectSet(string key, string field, object value, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
      TryAct(db => db.HashObjectSet(key, field, value, when, flags));

    public void HashObjectSet(string key, IDictionary<string, object> values, CommandFlags flags = CommandFlags.None) =>
      TryAct(db => db.HashObjectSet(key, values, flags));

    public Task<T> HashObjectGetAsync<T>(string key, string field, CommandFlags flags = CommandFlags.None) where T : class =>
      TryActAsync(db => db.HashObjectGetAsync<T>(key, field, flags));

    public Task<Collection<T>> HashObjectGetAsync<T>(string key, IEnumerable<string> fields, CommandFlags flags = CommandFlags.None)
      where T : class =>
      TryActAsync(db => db.HashObjectGetAsync<T>(key, fields, flags));

    public Task<bool> HashObjectSetAsync(string key, string field, object value, 
      When when = When.Always, CommandFlags flags = CommandFlags.None) =>
      TryActAsync(db => db.HashObjectSetAsync(key, field, value, when, flags));

    public Task HashObjectSetAsync(string key, IDictionary<string, object> values, CommandFlags flags = CommandFlags.None) =>
      TryActAsync(db => db.HashObjectSetAsync(key, values, flags));

    public string HashStringGet(string key, string field, CommandFlags flags = CommandFlags.None) =>
      TryAct(db => db.HashStringGet(key, field, flags));

    public Collection<string> HashStringGet(string key, IEnumerable<string> fields, CommandFlags flags = CommandFlags.None) =>
      TryAct(db => db.HashStringGet(key, fields, flags));

    public bool HashStringSet(string key, string field, string value, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
      TryAct(db => db.HashStringSet(key, field, value, when, flags));

    public void HashStringSet(string key, IDictionary<string, string> values, CommandFlags flags = CommandFlags.None) =>
      TryAct(db => db.HashStringSet(key, values, flags));

    public Task<string> HashStringGetAsync(string key, string field, CommandFlags flags = CommandFlags.None) =>
      TryActAsync(db => db.HashStringGetAsync(key, field, flags));

    public Task<Collection<string>> HashStringGetAsync(string key, IEnumerable<string> fields, CommandFlags flags = CommandFlags.None) =>
      TryActAsync(db => db.HashStringGetAsync(key, fields, flags));

    public Task<bool> HashStringSetAsync(string key, string field, string value, 
      When when = When.Always, CommandFlags flags = CommandFlags.None) =>
      TryActAsync(db => db.HashStringSetAsync(key, field, value, when, flags));

    public Task HashStringSetAsync(string key, IDictionary<string, string> values, CommandFlags flags = CommandFlags.None) =>
      TryActAsync(db => db.HashStringSetAsync(key, values, flags));

    public bool HashDelete(string key, string field, CommandFlags flags = CommandFlags.None) =>
      TryAct(db => db.HashDelete(key, field, flags));

    public long HashDelete(string key, IEnumerable<string> fields, CommandFlags flags = CommandFlags.None) =>
      TryAct(db => db.HashDelete(key, fields, flags));

    public Task<bool> HashDeleteAsync(string key, string field, CommandFlags flags = CommandFlags.None) =>
      TryActAsync(db => db.HashDeleteAsync(key, field, flags));

    public Task<long> HashDeleteAsync(string key, IEnumerable<string> fields, CommandFlags flags = CommandFlags.None) =>
      TryActAsync(db => db.HashDeleteAsync(key, fields, flags));

    public Dictionary<string, T> HashObjectGetAll<T>(string key, CommandFlags flags = CommandFlags.None) where T : class =>
      TryAct(db => db.HashObjectGetAll<T>(key, flags));

    public Task<Dictionary<string, T>> HashObjectGetAllAsync<T>(string key, CommandFlags flags = CommandFlags.None) where T : class =>
      TryActAsync(db => db.HashObjectGetAllAsync<T>(key, flags));

    public Dictionary<string, string> HashStringGetAll(string key, CommandFlags flags = CommandFlags.None) =>
      TryAct(db => db.HashStringGetAll(key, flags));

    public Task<Dictionary<string, string>> HashStringGetAllAsync(string key, CommandFlags flags = CommandFlags.None) =>
      TryActAsync(db => db.HashStringGetAllAsync(key, flags));

    private T TryAct<T>(Func<IRedisDatabase, T> action, T defaultResult = default)
    {
      try
      {
        return _redisDatabase == null ? defaultResult : action(_redisDatabase);
      }
      catch (Exception e)
      {
        LogError(e);
        return defaultResult;
      }
    }

    private void TryAct(Action<IRedisDatabase> action) =>
      TryAct<object>(db =>
      {
        action(db);
        return null;
      });

    private Task<T> TryActAsync<T>(Func<IRedisDatabase, Task<T>> action, T defaultResult = default) =>
      TryAct(async db => await action(db).ConfigureAwait(false), Task.FromResult(defaultResult));
    
    private Task TryActAsync(Func<IRedisDatabase, Task> action) =>
      TryAct(async db => await action(db).ConfigureAwait(false));

    private void LogError(Exception e)
    {
      _logger.LogError(e, "Action Redis has been failed.");
    }
  }
}
