using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using ViennaNET.Utils;

namespace ViennaNET.Redis.Implementation.Default
{
  internal class RedisServer : IRedisServer
  {
    private readonly string _prefixKey;
    private readonly IServer _server;
    private readonly ILogger _logger;

    public RedisServer(IServer server, ILogger<RedisServer> logger, string prefixKey)
    {
      _server = server.ThrowIfNull(nameof(server));
      _logger = logger.ThrowIfNull(nameof(logger));
      _prefixKey = prefixKey;
    }

    public bool IsConnected => _server.IsConnected;

    public EndPoint EndPoint => _server.EndPoint;

    public ServerType ServerType => _server.ServerType;

    public Version Version => _server.Version;

    public bool IsSlave => _server.IsSlave;

    private void LogDebug(string action, string arguments)
    {
      _logger.LogDebug("Action Redis: {action}. Arguments: {arguments}.", action, arguments);
    }

    public IEnumerable<string> Keys(
      string pattern, int database = 0, int pageSize = 10, CommandFlags flags = default)
    {
      LogDebug("Keys", $"Database = {database}; Pattern = {pattern}; Page size = {pageSize}; Flags = {flags}");
      return _server.Keys(database, $"{_prefixKey}{pattern}", pageSize, flags)
                    .Select(v => ((string)v)?.Remove(0, _prefixKey.Length));
    }

    public void Save(SaveType saveType = SaveType.BackgroundSave, CommandFlags flags = default)
    {
      LogDebug("Save", $"Save type = {saveType}; Flags = {flags}");
      _server.Save(saveType, flags);
    }

    public async Task SaveAsync(SaveType saveType = SaveType.BackgroundSave, CommandFlags flags = default)
    {
      var result = _server.SaveAsync(saveType, flags);
      LogDebug("SaveAsync", $"Save type = {saveType}; Flags = {flags}");
      await result;
    }

    public KeyValuePair<string, string>[] ConfigGet(string pattern = default, CommandFlags flags = default)
    {
      LogDebug("ConfigGet", $"Pattern = {pattern}; Flags = {flags}");
      return _server.ConfigGet(pattern, flags);
    }

    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(string pattern = default, CommandFlags flags = default)
    {
      var result = _server.ConfigGetAsync(pattern, flags);
      LogDebug("ConfigGetAsync", $"Pattern = {pattern}; Flags = {flags}");
      return await result;
    }

    public void ConfigSet(string key, string value, CommandFlags flags = default)
    {
      var valueLength = GetValueLength(value);
      LogDebug("ConfigSet", $"Key = {key}; Value-length = {valueLength}; Flags = {flags}");
      _server.ConfigSet(key, value, flags);
    }

    public async Task ConfigSetAsync(string key, string value, CommandFlags flags = default)
    {
      var result = _server.ConfigSetAsync(key, value, flags);
      var valueLength = GetValueLength(value);
      LogDebug("ConfigSetAsync", $"Key = {key}; Value-length = {valueLength}; Flags = {flags}");
      await result;
    }

    public IGrouping<string, KeyValuePair<string, string>>[] Info(string section, CommandFlags flags = default)
    {
      LogDebug("Info", $"Section = {section}; Flags = {flags}");
      return _server.Info(section, flags);
    }

    public async Task<IGrouping<string, KeyValuePair<string, string>>[]> InfoAsync(string section, CommandFlags flags = default)
    {
      var result = _server.InfoAsync(section, flags);
      LogDebug("InfoAsync", $"Section = {section}; Flags = {flags}");
      return await result;
    }

    public DateTime LastSave(CommandFlags flags = default)
    {
      LogDebug("LastSave", $"Flags = {flags}");
      return _server.LastSave(flags);
    }

    public async Task<DateTime> LastSaveAsync(CommandFlags flags = default)
    {
      var result = _server.LastSaveAsync(flags);
      LogDebug("LastSaveAsync", $"Flags = {flags}");
      return await result;
    }

    public DateTime Time(CommandFlags flags = default)
    {
      LogDebug("Time", $"Flags = {flags}");
      return _server.Time(flags);
    }

    public async Task<DateTime> TimeAsync(CommandFlags flags = default)
    {
      var result = _server.TimeAsync(flags);
      LogDebug("TimeAsync", $"Flags = {flags}");
      return await result;
    }

    public void FlushAllDatabases(CommandFlags flags = CommandFlags.None)
    {
      _server.FlushAllDatabases(flags);
      LogDebug("FlushAllDatabases", $"Flags = {flags}");
    }

    public async Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None)
    {
      var result = _server.FlushAllDatabasesAsync(flags);
      LogDebug("FlushAllDatabasesAsync", $"Flags = {flags}");
      await result.ConfigureAwait(false);
    }

    public void FlushDatabase(int database = 0, CommandFlags flags = CommandFlags.None)
    {
      _server.FlushDatabase(database, flags);
      LogDebug("FlushAllDatabasesAsync", $"Database = {database}; Flags = {flags}");
    }

    public async Task FlushDatabaseAsync(int database = 0, CommandFlags flags = CommandFlags.None)
    {
      var result = _server.FlushDatabaseAsync(database, flags);
      LogDebug("FlushAllDatabasesAsync", $"Database = {database}; Flags = {flags}");
      await result.ConfigureAwait(false);
    }

    private static int GetValueLength(string value)
    {
      return value?.Length ?? 0;
    }
  }
}
