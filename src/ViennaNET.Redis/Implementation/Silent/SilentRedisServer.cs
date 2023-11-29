using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ViennaNET.Redis.Implementation.Silent
{
  internal class SilentRedisServer : ISilentRedisServer
  {
    private readonly ILogger _logger;
    private readonly IRedisServer _redisServer;

    public SilentRedisServer(IRedisServer redisServer, ILogger<SilentRedisServer> logger)
    {
      _redisServer = redisServer;
      _logger = logger;
    }

    public bool? IsConnected
    {
      get
      {
        try
        {
          return _redisServer?.IsConnected;
        }
        catch (Exception e)
        {
          LogError(e);
          return null;
        }
      }
    }

    public EndPoint EndPoint
    {
      get
      {
        try
        {
          return _redisServer?.EndPoint;
        }
        catch (Exception e)
        {
          LogError(e);
          return null;
        }
      }
    }

    public ServerType? ServerType
    {
      get
      {
        try
        {
          return _redisServer?.ServerType;
        }
        catch (Exception e)
        {
          LogError(e);
          return null;
        }
      }
    }

    public Version Version
    {
      get
      {
        try
        {
          return _redisServer?.Version;
        }
        catch (Exception e)
        {
          LogError(e);
          return null;
        }
      }
    }

    public bool? IsSlave
    {
      get
      {
        try
        {
          return _redisServer?.IsSlave;
        }
        catch (Exception e)
        {
          LogError(e);
          return null;
        }
      }
    }

    public IEnumerable<string> Keys(
      string pattern, int database = 0, int pageSize = 10, CommandFlags flags = CommandFlags.None)
    {
      try
      {
        return _redisServer?.Keys(pattern, database, pageSize, flags);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public void Save(SaveType saveType = SaveType.BackgroundSave, CommandFlags flags = CommandFlags.None)
    {
      try
      {
        _redisServer?.Save(saveType, flags);
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    public async Task SaveAsync(SaveType saveType = SaveType.BackgroundSave, CommandFlags flags = CommandFlags.None)
    {
      try
      {
        if (_redisServer != null)
        {
          await _redisServer.SaveAsync(saveType, flags)
            .ConfigureAwait(false);
        }
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    public KeyValuePair<string, string>[] ConfigGet(string pattern = default, CommandFlags flags = CommandFlags.None)
    {
      try
      {
        return _redisServer?.ConfigGet(pattern, flags);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(string pattern = default,
      CommandFlags flags = CommandFlags.None)
    {
      try
      {
        return _redisServer == null
          ? null
          : await _redisServer.ConfigGetAsync(pattern, flags)
            .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public void ConfigSet(string key, string value, CommandFlags flags = CommandFlags.None)
    {
      try
      {
        _redisServer?.ConfigSet(key, value, flags);
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    public async Task ConfigSetAsync(string key, string value, CommandFlags flags = CommandFlags.None)
    {
      try
      {
        if (_redisServer != null)
        {
          await _redisServer.ConfigSetAsync(key, value)
            .ConfigureAwait(false);
        }
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    public IGrouping<string, KeyValuePair<string, string>>[] Info(string section,
      CommandFlags flags = CommandFlags.None)
    {
      try
      {
        return _redisServer?.Info(section, flags);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public async Task<IGrouping<string, KeyValuePair<string, string>>[]> InfoAsync(string section,
      CommandFlags flags = CommandFlags.None)
    {
      try
      {
        return _redisServer == null
          ? null
          : await _redisServer.InfoAsync(section, flags)
            .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public DateTime? LastSave(CommandFlags flags = CommandFlags.None)
    {
      try
      {
        return _redisServer?.LastSave(flags);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public async Task<DateTime?> LastSaveAsync(CommandFlags flags = CommandFlags.None)
    {
      try
      {
        return _redisServer == null
          ? (DateTime?)null
          : await _redisServer.LastSaveAsync(flags)
            .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public DateTime? Time(CommandFlags flags = CommandFlags.None)
    {
      try
      {
        return _redisServer?.Time(flags);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public async Task<DateTime?> TimeAsync(CommandFlags flags = CommandFlags.None)
    {
      try
      {
        return _redisServer == null
          ? (DateTime?)null
          : await _redisServer.TimeAsync(flags)
            .ConfigureAwait(false);
      }
      catch (Exception e)
      {
        LogError(e);
        return null;
      }
    }

    public void FlushAllDatabases(CommandFlags flags = CommandFlags.None)
    {
      try
      {
        _redisServer?.FlushAllDatabases(flags);
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    public async Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None)
    {
      try
      {
        if (_redisServer != null)
        {
          await _redisServer.FlushAllDatabasesAsync(flags)
            .ConfigureAwait(false);
        }
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    public void FlushDatabase(int database = 0, CommandFlags flags = CommandFlags.None)
    {
      try
      {
        _redisServer?.FlushDatabase(database, flags);
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    public async Task FlushDatabaseAsync(int database = 0, CommandFlags flags = CommandFlags.None)
    {
      try
      {
        if (_redisServer != null)
        {
          await _redisServer.FlushDatabaseAsync(database, flags)
            .ConfigureAwait(false);
        }
      }
      catch (Exception e)
      {
        LogError(e);
      }
    }

    private void LogError(Exception e)
    {
      _logger.LogError(e, "Action Redis has been failed.");
    }
  }
}