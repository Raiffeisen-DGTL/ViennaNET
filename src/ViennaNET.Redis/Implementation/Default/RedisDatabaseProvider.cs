using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using ViennaNET.Utils;

namespace ViennaNET.Redis.Implementation.Default
{
  /// <inheritdoc />
  [ExcludeFromCodeCoverage]
  public class RedisDatabaseProvider : IRedisDatabaseProvider
  {
    private readonly ConnectionOptions _connectionOptions;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly object _locker = new object();
    private readonly string _prefixKey;

    private volatile ConnectionMultiplexer _connection;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="IConnectionConfiguration" />
    /// </summary>
    /// <param name="configuration">Провайдер конфигурации</param>
    /// <param name="logger">Логгер</param>
    /// <param name="loggerFactory">Фабрика логгеров</param>
    public RedisDatabaseProvider([System.Diagnostics.CodeAnalysis.NotNull] IConnectionConfiguration configuration,
      ILogger<RedisDatabaseProvider> logger, ILoggerFactory loggerFactory)
    {
      _logger = logger.ThrowIfNull(nameof(logger));
      _loggerFactory = loggerFactory.ThrowIfNull(nameof(loggerFactory));
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

      return new RedisDatabase(useCompression, db, _loggerFactory.CreateLogger<RedisDatabase>(),
        _connectionOptions.KeyLifetimes, _prefixKey);
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
        _logger.LogError(e.Exception,
                              "Connection to Redis has been failed. ConnectionType = {connectionType}. "
                              + "EndPoint = {endPoint}. FailureType = {failureType}", e.ConnectionType, e.EndPoint, e.FailureType);
      connection.ConnectionRestored += (s, e) =>
        _logger.LogDebug("Connection to Redis has been restored. ConnectionType = {connectionType}. "
                        + "EndPoint = {endPoint}. FailureType = {failureType}", e.ConnectionType, e.EndPoint, e.FailureType);
      connection.ErrorMessage += (s, e) =>
        _logger.LogError(e.Message, "Error has been received. Message = {message}. EndPoint = {endPoint}.", e.Message, e.EndPoint);

      if (_connectionOptions.ClearOnStartup)
      {
        ClearServiceKeys(connection);
      }

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
                               .GetServer(endPoint, asyncState), _loggerFactory.CreateLogger<RedisServer>(), _prefixKey);
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
