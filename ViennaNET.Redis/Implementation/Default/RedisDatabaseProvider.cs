using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ViennaNET.Logging;
using ViennaNET.Utils;
using StackExchange.Redis;

namespace ViennaNET.Redis.Implementation.Default
{
  /// <inheritdoc />
  public class RedisDatabaseProvider : IRedisDatabaseProvider
  {
    private readonly ConnectionOptions _connectionOptions;
    private readonly object _locker = new object();
    private readonly string _prefixKey;

    private volatile ConnectionMultiplexer _connection;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="IConnectionConfiguration" />
    /// </summary>
    /// <param name="configuration">Провайдер конфигурации</param>
    public RedisDatabaseProvider([NotNull] IConnectionConfiguration configuration)
    {
      _connectionOptions = configuration.ThrowIfNull(nameof(configuration))
                                        .GetConnectionConfigurationOptions()
                                        .ThrowIfNull(nameof(_connectionOptions));

      _prefixKey = string.IsNullOrEmpty(_connectionOptions.Key)
        ? string.Empty
        : _connectionOptions.Key + ":";
    }

    /// <inheritdoc />
    public IRedisDatabase GetDatabase(bool useCompression = false, int database = 0, object asyncState = null)
    {
      var db = GetDatabaseInternal(database, asyncState);

      return new RedisDatabase(useCompression, db, _connectionOptions.KeyLifetimes, _prefixKey);
    }

    /// <inheritdoc />
    public IRedisServer GetServer(EndPoint endPoint, object asyncState = null)
    {
      return GetServerInternal(endPoint, asyncState);
    }

    /// <inheritdoc />
    public IEnumerable<IRedisServer> GetAllServers(object asyncState = null)
    {
      return GetConnection()
             .GetEndPoints()
             .Select(endPoint => GetServerInternal(endPoint, asyncState));
    }

    private ConnectionMultiplexer CreateConnection()
    {
      var connection = ConnectionMultiplexer.Connect(_connectionOptions.GetConfigurationOptions()
                                                     ?? throw new InvalidOperationException("Redis connection is not found"));
      connection.ConnectionFailed += (s, e) =>
        Logger.LogErrorFormat(e.Exception,
                              $"Connection to Redis has been failed. ConnectionType = {e.ConnectionType}. "
                              + $"EndPoint = {e.EndPoint}. FailureType = {e.FailureType}");
      connection.ConnectionRestored += (s, e) =>
        Logger.LogDebug($"Connection to Redis has been restored. ConnectionType = {e.ConnectionType}. "
                        + $"EndPoint = {e.EndPoint}. FailureType = {e.FailureType}");
      connection.ErrorMessage += (s, e) =>
        Logger.LogErrorFormat(e.Message, $"Error has been received. Message = {e.Message}. EndPoint = {e.EndPoint}.");

      ClearServiceKeys(connection);

      return connection;
    }

    private ConnectionMultiplexer GetConnection()
    {
      if (_connection != null)
      {
        return _connection;
      }

      lock (_locker)
      {
        if (_connection == null)
        {
          _connection = CreateConnection();
        }
      }

      return _connection;
    }

    private IRedisServer GetServerInternal(EndPoint endPoint, object asyncState)
    {
      return new RedisServer(GetConnection()
                               .GetServer(endPoint, asyncState), _prefixKey);
    }

    private void ClearServiceKeys(IConnectionMultiplexer connection)
    {
      var db = connection.GetDatabase();

      foreach (var endPoint in connection.GetEndPoints())
      {
        var server = connection.GetServer(endPoint);
        foreach (var key in server.Keys(pattern: $"{_prefixKey}*"))
        {
          db.KeyDelete(key);
        }
      }
    }

    private IDatabase GetDatabaseInternal(int database = 0, object asyncState = null)
    {
      return GetConnection()
        .GetDatabase(database, asyncState);
    }
  }
}
