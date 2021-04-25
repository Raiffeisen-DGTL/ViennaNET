# Сборка, содержащая классы для работы с Redis. Обертка библиотеки StackExchange.Redis

### Основные сущности

* **RedisDatabaseProvider** - считывает из конфигурации параметры подключения к Redis и создает подключение к Redis. Необходим для создания абстракций **RedisDatabase** и **RedisServer**.

* **RedisDatabase** - посредник для работы с хранилищем. Позволяет считывать и записывать данные по ключам. 

* **RedisServer** - посредник для работы с функциональностью сервера. Поддерживает массовые и сервисные операции.

### Silent-версии сущностей

Для исключения необходимости вручную перехватывать исключения в случае отказа Redis дополнительно к основным сущностям разработаны их Silent-версии. Они перехватывают все исключения, только логируя их. Это позволяет работать с Redis как с дополнительным ресурсом, в случае отказа которого не возникнет проблем с основным функционалом приложения.

#### Инструкция по применению:

1.  Добавляем в класс зависимость от **ISilentRedisDatabaseProvider**.
2.  Добавляем файл конфигурации **appsettings.json**,  

	"redis": {
      "key": "9050",
      "connection": "localhost:6379",
      "keylifetimes": [
        {
          "name": "default",
          "time": "0.8:0:0"
        }
      ]
    }

* key - дополнительный префикс сервиса. Префиксы нужны, чтобы в общем адресном пространстве сервиса ключи с совпадающими именами, созданные различными сервисами, не пересекались и не затирали значения друг друга.
* connection - строка подключения в формате библиотеки StackExchange.Redis.
* keylifetimes - Коллекция именованных значений времени жизни ключей.

3.  Для осуществления кеширования создаем объект **SilentRedisDatabase**, вызывая метод **ISilentRedisDatabaseProvider** GetDatabase.

    public ReadingService(ISilentRedisDatabaseProvider silentRedisDatabaseProvider)
    {
      _redis = silentRedisDatabaseProvider.ThrowIfNull(nameof(silentRedisDatabaseProvider)).GetDatabase();
    }

4.  Используем для работы стандартный подход из 3 пунктов:

* В методе запрашиваем значение ключа из Redis. Если оно не равно null, возвращаем его. 
* Получаем данные из источника. 
* Сохраняем данные по ключу в Redis.

      var objects = _redis.ObjectGet<List<T>>(redisKey);
      if (objects != null)
      {
        return objects;
      }

      objects = _entityFactoryService.CreateCustomQueryExecutor<T>()
                                     .CustomQuery(query)
                                     .ToList();
      _redis.ObjectSet(redisKey, objects, lifetimeKey);

      return objects;