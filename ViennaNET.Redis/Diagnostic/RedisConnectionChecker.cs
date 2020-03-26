using ViennaNET.Diagnostic;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Logging;
using ViennaNET.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    /// <summary>
    /// Инициализирует экземпляр ссылкой на <see cref="IRedisDatabaseProvider" />
    /// </summary>
    /// <param name="provider">Ссылка на интерфейс, представляющий провайдер БД Redis</param>
    public RedisConnectionChecker(IRedisDatabaseProvider provider)
    {
      _provider = provider.ThrowIfNull(nameof(provider));
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
        var result = database.StringSet(temporaryKey, redisKey, isDiagnostic:true);
        if (!result)
        {
          Logger.LogDiagnostic("Diagnostic of redis provider has been failed, cannot set value to redis");
          return Task.FromResult(new[]
          {
            new DiagnosticInfo(redisKey, localhost, DiagnosticStatus.DbConnectionError, string.Empty,
                               "Cannot save value to redis database", true)
          }.AsEnumerable());
        }
        var deleteRes = database.KeyDelete(temporaryKey, isDiagnostic: true);
        if (!deleteRes)
        {
          Logger.LogDiagnostic("Diagnostic of redis provider has been failed, cannot delete value from redis");
          return Task.FromResult(new[]
          {
            new DiagnosticInfo(redisKey, localhost, DiagnosticStatus.DbConnectionError, string.Empty,
                               "Cannot delete value from redis database", true)
          }.AsEnumerable());
        }
        Logger.LogDiagnostic("Redis provider has been diagnosed successfully");
        return Task.FromResult(new[] { new DiagnosticInfo(redisKey, localhost, isSkipResult: true) }.AsEnumerable());
      }
      catch (Exception e)
      {
        Logger.LogDiagnostic($"Diagnostic of redis provider has been failed with error: {e}");
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