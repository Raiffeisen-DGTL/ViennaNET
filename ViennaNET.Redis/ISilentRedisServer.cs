using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ViennaNET.Redis
{
  /// <summary>
  /// Описывает возможности, для работы с локальным сервером Redis и в кластере.
  /// </summary>
  public interface ISilentRedisServer
  {
    /// <summary>
    /// Является ли сервер активным и доступным для использования.
    /// </summary>
    bool? IsConnected { get; }

    /// <summary>
    /// Адрес сервера.
    /// </summary>
    EndPoint EndPoint { get; }

    /// <summary>
    /// Режим работы сервера.
    /// </summary>
    ServerType? ServerType { get; }

    /// <summary>
    /// Версия сервера.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Является ли сервер репликой / подчиненным.
    /// </summary>
    bool? IsSlave { get; }

    /// <summary>
    /// Получение коллекции ключей по шаблону. ПРЕДУПРЕЖДЕНИЕ: Использовать данную команду нужно с особой осторожностью. Во время ее выполнения блокируется весь сервер Redis. Нельзя использовать ее в повседневной разработке.
    /// </summary>
    /// <remarks>
    /// https://redis.io/commands/keys,
    /// https://redis.io/commands/scan
    /// </remarks>
    /// <param name="pattern"></param>
    /// <param name="database"></param>
    /// <param name="pageSize"></param>
    /// <param name="flags"></param>
    IEnumerable<string> Keys(
      string pattern, int database = 0, int pageSize = 10, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Явный запрос, для сохранения текущего состояния на диск.
    /// </summary>
    /// <remarks>
    /// https://redis.io/commands/bgrewriteaof,
    /// https://redis.io/commands/bgsave,
    /// https://redis.io/commands/save,
    /// https://redis.io/topics/persistence
    /// </remarks>
    /// <param name="saveType"></param>
    /// <param name="flags"></param>
    void Save(SaveType saveType = SaveType.BackgroundSave, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Явный запрос, для сохранения текущего состояния на диск.
    /// </summary>
    /// <remarks>
    /// https://redis.io/commands/bgrewriteaof,
    /// https://redis.io/commands/bgsave,
    /// https://redis.io/commands/save,
    /// https://redis.io/topics/persistence
    /// </remarks>
    /// <param name="saveType"></param>
    /// <param name="flags"></param>
    Task SaveAsync(SaveType saveType = SaveType.BackgroundSave, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Получение всех параметров конфигурации, соответствующих указанному шаблону.
    /// </summary>
    /// <remarks>https://redis.io/commands/config-get</remarks>
    /// <param name="pattern"></param>
    /// <param name="flags"></param>
    /// <returns>Все подходящие параметры конфигурации.</returns>
    KeyValuePair<string, string>[] ConfigGet(string pattern = default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Получение всех параметров конфигурации, соответствующих указанному шаблону.
    /// </summary>
    /// <remarks>https://redis.io/commands/config-get</remarks>
    /// <param name="pattern"></param>
    /// <param name="flags"></param>
    /// <returns>Все подходящие параметры конфигурации.</returns>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(string pattern = default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Команда используется для перенастройки сервера во время выполнения без необходимости перезапуска Redis.
    /// </summary>
    /// <remarks>https://redis.io/commands/config-set</remarks>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="flags"></param>
    void ConfigSet(string key, string value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Команда используется для перенастройки сервера во время выполнения без необходимости перезапуска Redis.
    /// </summary>
    /// <remarks>https://redis.io/commands/config-set</remarks>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="flags"></param>
    Task ConfigSetAsync(string key, string value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Команда возвращает информацию и статистику о сервере в формате, который легко парсится и читается.
    /// </summary>
    /// <remarks>https://redis.io/commands/info</remarks>
    /// <param name="section"></param>
    /// <param name="flags"></param>
    IGrouping<string, KeyValuePair<string, string>>[] Info(string section, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Команда возвращает информацию и статистику о сервере в формате, который легко парсится и читается.
    /// </summary>
    /// <remarks>https://redis.io/commands/info</remarks>
    /// <param name="section"></param>
    /// <param name="flags"></param>
    Task<IGrouping<string, KeyValuePair<string, string>>[]> InfoAsync(string section, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Возвращает время последнего успешного сохранения в базу данных.
    /// </summary>
    /// <remarks>https://redis.io/commands/lastsave</remarks>
    /// <param name="flags"></param>
    DateTime? LastSave(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Возвращает время последнего успешного сохранения в базу данных.
    /// </summary>
    /// <remarks>https://redis.io/commands/lastsave</remarks>
    /// <param name="flags"></param>
    Task<DateTime?> LastSaveAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Возвращает текущее время сервера.
    /// </summary>
    /// <remarks>https://redis.io/commands/time</remarks>
    /// <param name="flags"></param>
    /// <returns>Текущее время сервера.</returns>
    DateTime? Time(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Возвращает текущее время сервера.
    /// </summary>
    /// <remarks>https://redis.io/commands/time</remarks>
    /// <param name="flags"></param>
    /// <returns>Текущее время сервера.</returns>
    Task<DateTime?> TimeAsync(CommandFlags flags = CommandFlags.None);
  }
}
