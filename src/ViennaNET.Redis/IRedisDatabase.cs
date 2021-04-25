using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using StackExchange.Redis;
using ViennaNET.Redis.Exceptions;

namespace ViennaNET.Redis
{
  /// <summary>
  /// Описывает возможности, которые предоставлены для работы с сервером Redis. 
  /// </summary>
  public interface IRedisDatabase
  {
    /// <summary>
    /// Получение значения по ключу. Если ключ не существует, возвращается NULL. 
    /// Возвращается исключение, если ключ не является строкой или структура объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/get</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение ключа или NULL.</returns>
    T ObjectGet<T>(string key, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение значений всех указанных ключей. Для каждого ключа, который не существует или не является строкой, возвращается NULL. 
    /// Возвращается исключение, если структура хотя бы одного объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/mget</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="keys">Имена ключей</param>
    /// <param name="flags">Флаги выполнения</param>
    Collection<T> ObjectGet<T>(IEnumerable<string> keys, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение значения поля хэша по ключу и имени поля. Если ключ не существует, возвращается NULL. 
    /// Возвращается исключение, если ключ не является строкой или структура объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/hget</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля хэша</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение поля или NULL.</returns>
    T HashObjectGet<T>(string key, string field, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение значений всех указанных полей хэша. Для каждого поля, которое не существует или не является строкой, возвращается NULL. 
    /// Возвращается исключение, если структура хотя бы одного объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/hmget</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="fields">Имена полей</param>
    /// <param name="flags">Флаги выполнения</param>
    Collection<T> HashObjectGet<T>(string key, IEnumerable<string> fields, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение значений всех полей хэша.
    /// Возвращается исключение, если структура хотя бы одного объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/hgetall</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    Dictionary<string, T> HashObjectGetAll<T>(string key, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Сохранение объекта по ключу. Если ключ существует, то значение перезаписывается независимо от его типа. 
    /// Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="expiry">Время жизни ключа</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    bool ObjectSet(string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объекта по ключу. Если ключ существует, то значение перезаписывается независимо от его типа. 
    /// Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
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
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="values">Список ключей со значениями</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если все ключи были установлены, иначе False.</returns>
    bool ObjectSet(IDictionary<string, object> values, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объекта в хэш по ключу и имени поля. Если поле существует, то значение перезаписывается независимо от его типа. 
    /// Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/hset</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если поле было установлен, иначе False.</returns>
    bool HashObjectSet(string key, string field, object value, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объектов в хэш по указанным полям. Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/hmset</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="values">Список полей со значениями</param>
    /// <param name="flags">Флаги выполнения</param>
    void HashObjectSet(string key, IDictionary<string, object> values, CommandFlags flags = default);

    /// <summary>
    /// Получение значения по ключу. Если ключ не существует, возвращается NULL. 
    /// Возвращается исключение, если ключ не является строкой или структура объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/get</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение ключа или NULL.</returns>
    Task<T> ObjectGetAsync<T>(string key, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение значений всех указанных ключей. Для каждого ключа, который не существует или не является строкой, возвращается NULL. 
    /// Возвращается исключение, если структура хотя бы одного объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/mget</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="keys">Имена ключей</param>
    /// <param name="flags">Флаги выполнения</param>
    Task<Collection<T>> ObjectGetAsync<T>(IEnumerable<string> keys, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение значения из хэша по ключу и имени поля. Если поле не существует, возвращается NULL. 
    /// Возвращается исключение, если структура объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/hget</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение поля или NULL.</returns>
    Task<T> HashObjectGetAsync<T>(string key, string field, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение из хэша значений всех указанных полей. Для каждого поля, которое не существует, возвращается NULL. 
    /// Возвращается исключение, если структура хотя бы одного объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/hmget</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="fields">Имена полей</param>
    /// <param name="flags">Флаги выполнения</param>
    Task<Collection<T>> HashObjectGetAsync<T>(string key, IEnumerable<string> fields, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Получение значений всех полей хэша.
    /// Возвращается исключение, если структура хотя бы одного объекта в памяти повреждена.
    /// </summary>
    /// <remarks>http://redis.io/commands/hgetall</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    Task<Dictionary<string, T>> HashObjectGetAllAsync<T>(string key, CommandFlags flags = default) where T : class;

    /// <summary>
    /// Сохранение объекта по ключу. Если ключ существует, то значение перезаписывается независимо от его типа. 
    /// Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="expiry">Время жизни ключа</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    Task<bool> ObjectSetAsync(string key, object value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объекта по ключу. Если ключ существует, то значение перезаписывается независимо от его типа. 
    /// Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
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
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="values">Список ключей со значениями</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если все ключи были установлены, иначе False.</returns>
    Task<bool> ObjectSetAsync(IDictionary<string, object> values, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объекта в хэш по ключу и имени поля. Если поле существует, то значение перезаписывается независимо от его типа. 
    /// Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/hset</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если поле было установлено, иначе False.</returns>
    Task<bool> HashObjectSetAsync(string key, string field, object value, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение объектов в хэш по указанным полям. Возвращается исключение, если произошла непредвиденная ошибка при записи.
    /// </summary>
    /// <remarks>http://redis.io/commands/hmset</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="values">Список полей со значениями</param>
    /// <param name="flags">Флаги выполнения</param>
    Task HashObjectSetAsync(string key, IDictionary<string, object> values, CommandFlags flags = default);

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
    Collection<string> StringGet(IEnumerable<string> keys, CommandFlags flags = default);

    /// <summary>
    /// Получение из хэша значения по ключу и имени поля. Если поле не существует, возвращается NULL.
    /// </summary>
    /// <remarks>http://redis.io/commands/hget</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение поля или NULL.</returns>
    string HashStringGet(string key, string field, CommandFlags flags = default);

    /// <summary>
    /// Получение из хэша значений всех указанных полей. Для каждого поля, которое не существует, возвращается NULL.
    /// </summary>
    /// <remarks>http://redis.io/commands/hmget</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="fields">Имена полей</param>
    /// <param name="flags">Флаги выполнения</param>
    Collection<string> HashStringGet(string key, IEnumerable<string> fields, CommandFlags flags = default);

    /// <summary>
    /// Получение значений всех полей хэша.
    /// </summary>
    /// <remarks>http://redis.io/commands/hgetall</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    Dictionary<string, string> HashStringGetAll(string key, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строкового значения по ключу. Если ключ существует, то значение перезаписывается независимо от его типа.
    /// </summary>
    /// <remarks>http://redis.io/commands/set</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="expiry">Время жизни ключа</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <param name="isDiagnostic">Диагностический уровень логирования</param>
    /// <returns>True если ключ был установлен, иначе False.</returns>
    bool StringSet(string key, string value, TimeSpan? expiry = null, When when = default, CommandFlags flags = default,
      bool isDiagnostic = false);

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
    bool StringSet(IDictionary<string, string> values, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строкового значения в хэш по ключу и имени поля. 
    /// Если поле существует, то значение перезаписывается независимо от его типа.
    /// </summary>
    /// <remarks>http://redis.io/commands/hset</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если поле было установлено, иначе False.</returns>
    bool HashStringSet(string key, string field, string value, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строковых значений в хэш по указанным полям.
    /// </summary>
    /// <remarks>http://redis.io/commands/hmset</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="values">Список полей со значениями</param>
    /// <param name="flags">Флаги выполнения</param>
    void HashStringSet(string key, IDictionary<string, string> values, CommandFlags flags = default);

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
    Task<Collection<string>> StringGetAsync(IEnumerable<string> keys, CommandFlags flags = default);

    /// <summary>
    /// Получение из хэша значения по ключу и имени поля. Если поле не существует, возвращается NULL.
    /// </summary>
    /// <remarks>http://redis.io/commands/hget</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Значение ключа или NULL.</returns>
    Task<string> HashStringGetAsync(string key, string field, CommandFlags flags = default);

    /// <summary>
    /// Получение из хэша значений всех указанных полей. Для каждого поля, которое не существует, возвращается NULL.
    /// </summary>
    /// <remarks>http://redis.io/commands/hmget</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="fields">Имена полей</param>
    /// <param name="flags">Флаги выполнения</param>
    Task<Collection<string>> HashStringGetAsync(string key, IEnumerable<string> fields, CommandFlags flags = default);

    /// <summary>
    /// Получение значений всех полей хэша.
    /// </summary>
    /// <remarks>http://redis.io/commands/hgetall</remarks>
    /// <exception cref="RedisException">
    /// При использовании <see cref="ISilentRedisDatabaseProvider"/> исключения не произойдет, будет возвращен NULL.
    /// </exception>
    /// <param name="key">Имя ключа</param>
    /// <param name="flags">Флаги выполнения</param>
    Task<Dictionary<string, string>> HashStringGetAllAsync(string key, CommandFlags flags = default);

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
    Task<bool> StringSetAsync(IDictionary<string, string> values, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строкового значения в хэш по ключу и имени поля. 
    /// Если поле существует, то значение перезаписывается независимо от его типа.
    /// </summary>
    /// <remarks>http://redis.io/commands/hset</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля</param>
    /// <param name="value">Значение для записи по ключу</param>
    /// <param name="when">Признак проверки наличия значения</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если поле было установлено, иначе False.</returns>
    Task<bool> HashStringSetAsync(string key, string field, string value, When when = default, CommandFlags flags = default);

    /// <summary>
    /// Сохранение строковых значений в хэш по указанным полям.
    /// </summary>
    /// <remarks>http://redis.io/commands/hmset</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="values">Список полей со значениями</param>
    /// <param name="flags">Флаги выполнения</param>
    Task HashStringSetAsync(string key, IDictionary<string, string> values, CommandFlags flags = default);

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
    long KeyDelete(IEnumerable<string> keys, CommandFlags flags = default);

    /// <summary>
    /// Удаляет указанное поле из хэша. При его отсутствии, оно игнорируется.
    /// </summary>
    /// <remarks>http://redis.io/commands/hdel</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если ключ был удален, иначе False.</returns>
    bool HashDelete(string key, string field, CommandFlags flags = default);

    /// <summary>
    /// Удаляет указанные поля из хэша. При их отсутствии, они игнорируются.
    /// </summary>
    /// <remarks>http://redis.io/commands/hdel</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="fields">Имена полей</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Количество удаленных полей.</returns>
    long HashDelete(string key, IEnumerable<string> fields, CommandFlags flags = default);

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
    Task<long> KeyDeleteAsync(IEnumerable<string> keys, CommandFlags flags = default);

    /// <summary>
    /// Удаляет указанное поле из хэша. При его отсутствии, оно игнорируется.
    /// </summary>
    /// <remarks>http://redis.io/commands/hdel</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="field">Имя поля</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>True если поле было удалено, иначе False.</returns>
    Task<bool> HashDeleteAsync(string key, string field, CommandFlags flags = default);

    /// <summary>
    /// Удаляет указанные поля из хэша. При их отсутствии, они игнорируются.
    /// </summary>
    /// <remarks>http://redis.io/commands/hdel</remarks>
    /// <param name="key">Имя ключа</param>
    /// <param name="fields">Имена полей</param>
    /// <param name="flags">Флаги выполнения</param>
    /// <returns>Количество удаленных ключей.</returns>
    Task<long> HashDeleteAsync(string key, IEnumerable<string> fields, CommandFlags flags = default);
  }
}
