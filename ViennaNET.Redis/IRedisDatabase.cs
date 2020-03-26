using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViennaNET.Redis.Exceptions;
using StackExchange.Redis;

namespace ViennaNET.Redis
{
  /// <summary>
  /// Описывает возможности, которые предоставлены для работы с сервером Redis. 
  /// </summary>
  public interface IRedisDatabase
  {
    /// <summary>
    /// Получение значения по ключу. Если ключ не существует, возвращается NULL. Возвращается исключение, если ключ не является строкой или структура объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/get</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение ключа или NULL.</returns>
    T ObjectGet<T>(string key, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение значений всех указанных ключей. Для каждого ключа, который не существует или не является строкой, возвращается NULL. Возвращается исключение, если структура хотя бы одного объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/mget</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="keys">Имена ключей</param>
    /// <param name="flags">Флаги выполнения</param>
    List<T> ObjectGet<T>(List<string> keys, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Сохранение объекта по ключу. Если ключ существует, то значение перезаписывается независимо от его типа. Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="expiry">Время жизни ключа</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    bool ObjectSet(string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объекта по ключу. Если ключ существует, то значение перезаписывается независимо от его типа. Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="lifetime">Ключ словаря времени жизни из конфигурации</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    bool ObjectSet(string key, object value, string lifetime, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объектов по указанным ключам. Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/mset</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="values">Список ключей со значениями</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если все ключи были установлены, иначе False.</returns>
    bool ObjectSet(Dictionary<string, object> values, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Получение значения по ключу. Если ключ не существует, возвращается NULL. Возвращается исключение, если ключ не является строкой или структура объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/get</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение ключа или NULL.</returns>
    Task<T> ObjectGetAsync<T>(string key, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение значений всех указанных ключей. Для каждого ключа, который не существует или не является строкой, возвращается NULL. Возвращается исключение, если структура хотя бы одного объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/mget</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="keys">Имена ключей</param>
    /// <param name="flags">Флаги выполнения</param>
    Task<List<T>> ObjectGetAsync<T>(List<string> keys, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Сохранение объекта по ключу. Если ключ существует, то значение перезаписывается независимо от его типа. Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="expiry">Время жизни ключа</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    Task<bool> ObjectSetAsync(string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объекта по ключу. Если ключ существует, то значение перезаписывается независимо от его типа. Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="lifetime">Ключ словаря времен жизни из конфигурации</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    Task<bool> ObjectSetAsync(string key, object value, string lifetime, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объектов по указанным ключам. Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/mset</remarks>
    /// <exception cref="Exceptions.RedisException">При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.</exception>
    /// <param name="values">Список ключей со значениями</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если все ключи были установлены, иначе False.</returns>
    Task<bool> ObjectSetAsync(Dictionary<string, object> values, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Получение значения по ключу. Если ключ не существует, возвращается NULL. Возвращается исключение, если ключ не является строкой.
    /// </summary>
    /// <remarks>http://redis.io/commands/get</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение ключа или NULL.</returns>
    string StringGet(string key, CommandFlags flags = default);

    /// <summary>
    /// Получение значений всех указанных ключей. Для каждого ключа, который не существует или не является строкой, возвращается NULL.
    /// </summary>
    /// <remarks>http://redis.io/commands/mget</remarks>
    /// <param name="keys">Имена ключей</param>
    /// <param name="flags">Флаги выполнения</param>
    List<string> StringGet(List<string> keys, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строкового значения по ключу. Если ключ существует, то значение перезаписывается независимо от его типа.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="expiry">Время жизни ключа</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    bool StringSet(string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default, bool isDiagnostic = false);

    /// <summary>
    /// Сохранение строкового значения по ключу. Если ключ существует, то значение перезаписывается независимо от его типа.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="lifetime">Ключ словаря времен жизни из конфигурации</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    bool StringSet(string key, string value, string lifetime, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строковых значений по указанным ключам.
    /// </summary>
    /// <remarks>http://redis.io/commands/mset</remarks>
    /// <param name="values">Список ключей со значениями</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если все ключи были установлены, иначе False.</returns>
    bool StringSet(Dictionary<string, string> values, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Получение значения по ключу. Если ключ не существует, возвращается NULL. Возвращается исключение, если ключ не является строкой.
    /// </summary>
    /// <remarks>http://redis.io/commands/get</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение ключа или NULL.</returns>
    Task<string> StringGetAsync(string key, CommandFlags flags = default);

    /// <summary>
    /// Получение значений всех указанных ключей. Для каждого ключа, который не существует или не является строкой, возвращается NULL.
    /// </summary>
    /// <remarks>http://redis.io/commands/mget</remarks>
    /// <param name="keys">Имена ключей</param>
    /// <param name="flags">Флаги выполнения</param>
    Task<List<string>> StringGetAsync(List<string> keys, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строкового значения по ключу. Если ключ существует, то значение перезаписывается независимо от его типа.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="expiry">Время жизни ключа</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строкового значения по ключу. Если ключ существует, то значение перезаписывается независимо от его типа.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="lifetime">Ключ словаря времен жизни из конфигурации</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    Task<bool> StringSetAsync(string key, string value, string lifetime, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строковых значений по указанным ключам.
    /// </summary>
    /// <remarks>http://redis.io/commands/mset</remarks>
    /// <param name="values">Список ключей со значениями</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если все ключи были установлены, иначе False.</returns>
    Task<bool> StringSetAsync(Dictionary<string, string> values, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Удаляет указанный ключ. При его отсутствии, он игнорируется.
    /// </summary>
    /// <remarks>http://redis.io/commands/del</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был удален, иначе False.</returns>
    bool KeyDelete(string key, CommandFlags flags = default, bool isDiagnostic = false);

    /// <summary>
    /// Удаляет указанные ключи. При их отсутствии, они игнорируются.
    /// </summary>
    /// <remarks>http://redis.io/commands/del</remarks>
    /// <param name="keys">Имена ключей</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Количество удаленных ключей.</returns>
    long KeyDelete(List<string> keys, CommandFlags flags = default);

    /// <summary>
    /// Удаляет указанный ключ. При его отсутствии, он игнорируется.
    /// </summary>
    /// <remarks>http://redis.io/commands/del</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был удален, иначе False.</returns>
    Task<bool> KeyDeleteAsync(string key, CommandFlags flags = default);

    /// <summary>
    /// Удаляет указанные ключи. При их отсутствии, они игнорируются.
    /// </summary>
    /// <remarks>http://redis.io/commands/del</remarks>
    /// <param name="keys">Имена ключей</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Количество удаленных ключей.</returns>
    Task<long> KeyDeleteAsync(List<string> keys, CommandFlags flags = default);
  }
}
