using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Utils;

namespace ViennaNET.Redis.Diagnostic
{
  /// <summary>
  /// Проверяет функцию доступа к Redis.
  /// </summary>
  public class RedisConnectionChecker : IDiagnosticImplementor
  {
    private const string redisKey = "redis";
    private const string localhost = "localhost";

    private readonly IRedisDatabaseProvider _provider;
    private readonly ILogger _logger;

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="IRedisDatabaseProvider" />
    /// </summary>
    /// <param name="provider">Ссылка на интерфейс, представляющий провайдер БД Redis</param>
    /// <param name="logger">Логгер</param>
    public RedisConnectionChecker(IRedisDatabaseProvider provider, ILogger<RedisConnectionChecker> logger)
    {
      _provider = provider.ThrowIfNull(nameof(provider));
      _logger = logger.ThrowIfNull(nameof(logger));
    }

    /// <summary>
    /// Проверяет подключение к БД Redis, добавляет временный ключ и тут же удаляет его 
    /// </summary>
    /// <returns>Результат диагностики</returns>
    public Task<IEnumerable<DiagnosticInfo>> Diagnose()
    {
      try
      {
        var database = _provider.GetDatabase();
        var temporaryKey = Guid.NewGuid()
                               .ToString();
        var result = database.StringSet(temporaryKey, redisKey, isDiagnostic: true);
        if (!result)
        {
          _logger.LogTrace("Diagnostic of redis provider has been failed, cannot set value to redis");
          return Task.FromResult(new[]
          {
            new DiagnosticInfo(redisKey, localhost, DiagnosticStatus.DbConnectionError, string.Empty,
                               "Cannot save value to redis database", true)
          }.AsEnumerable());
        }
        var deleteRes = database.KeyDelete(temporaryKey, isDiagnostic: true);
        if (!deleteRes)
        {
          _logger.LogTrace("Diagnostic of redis provider has been failed, cannot delete value from redis");
          return Task.FromResult(new[]
          {
            new DiagnosticInfo(redisKey, localhost, DiagnosticStatus.DbConnectionError, string.Empty,
                               "Cannot delete value from redis database", true)
          }.AsEnumerable());
        }
        _logger.LogTrace("Redis provider has been diagnosed successfully");
        return Task.FromResult(new[] { new DiagnosticInfo(redisKey, localhost, isSkipResult: true) }.AsEnumerable());
      }
      catch (Exception e)
      {
        _logger.LogTrace($"Diagnostic of redis provider has been failed with error: {e}");
        return Task.FromResult(new[]
        {
          new DiagnosticInfo(redisKey, localhost, DiagnosticStatus.DbConnectionError, string.Empty, e.ToString(),true)
        }.AsEnumerable());
      }
    }

    /// <inheritdoc />
    public string Key => redisKey;
  }
}