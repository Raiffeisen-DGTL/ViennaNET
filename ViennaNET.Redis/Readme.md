# An assembly containing classes for working with Redis. StackExchange.Redis library wrapper

### Key Entities

* **RedisDatabaseProvider** - reads the parameters for connecting to Redis from the configuration and creates a connection to Redis. Required to create abstractions **RedisDatabase** and **RedisServer**.

* **RedisDatabase** - a repository intermediary. Allows reading and writing key data.

* **RedisServer** - an intermediary for working with server functionality. Supports mass and service operations.

### Silent Entities

To eliminate the need to manually catch exceptions in case of Redis failure, in addition to the main entities, their Silent versions are developed. They catch all exceptions, only logging them. This allows you to work with Redis as an additional resource, in case of failure of which there will be no problems with the main functionality of the application.

#### Instructions for use:

1. Add a dependency on **ISilentRedisDatabaseProvider** to the class.
2. Add the configuration file **appsettings.json**,

```
"redis": {
      "key": "9050",
      "connection": "localhost: 6379",
      "keylifetimes": [
        {
          "name": "default",
          "time": "0.8: 0: 0"
        }
      ]
    }
```

* key - an additional service prefix. Prefixes are needed so that in the general address space of the service, keys with matching names created by different services do not overlap and do not override each other's values.
* connection - connection string in the format of the StackExchange.Redis library.
* keylifetimes - A collection of named key lifetimes.

3. To implement caching, create an **SilentRedisDatabase** object by calling the **ISilentRedisDatabaseProvider** GetDatabase method.

```csharp
    public ReadingService (ISilentRedisDatabaseProvider silentRedisDatabaseProvider)
    {
      _redis = silentRedisDatabaseProvider.ThrowIfNull(nameof(silentRedisDatabaseProvider)).GetDatabase();
    }
```

4. We use the standard 3-point approach for work:

* In the method, we request the key value from Redis. If it is not null, return it.
* Get data from the source.
* Save key data in Redis.

```csharp
      var objects = _redis.ObjectGet<List<T>>(redisKey);
      if (objects! = null)
      {
        return objects;
      }

      objects = _entityFactoryService.CreateCustomQueryExecutor<T>()
                                     .CustomQuery(query)
                                     .ToList();
      _redis.ObjectSet(redisKey, objects, lifetimeKey);

      return objects;
```      
