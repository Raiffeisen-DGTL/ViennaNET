# ViennaNET.KasperskyScanEngine.Client

Библиотека предоставляет API HTTP-клиента, реализующего протокол [KAV версии 3][kav-protocol], позволяющий взаимодействовать
в режиме HTTP с службой [Kaspersky Scan Engine][kse], для сканирования файлов на наличие вирусов.

## Оглавление

* [Введение](#введение)
* [Установка](#установка)
* [Руководство пользователя](#руководство-пользователя)

## Введение

Клиент использует протокол [KAV][kav-protocol] для взаимодействия с Kaspersky Scan Engine посредством HTTP-запросов,
и осуществления сканирования файлов или url адресов, передаваемых пользователем, на наличие вирусов, а так же
выполнения других, поддерживаемых [KAV][kav-protocol] операций. HTTP клиент, создан с использованием
[ViennaNET.Extensions.Http][viennanet-ext-http-docs] поддерживает все параметры конфигурации определённые
[`ClientOptionsBase`][client-opt-base], в томчисле политики повтора.

## Установка

Добавьте в проект сслыку на пакте ViennaNET.KasperskyScanEngine.Client.

```shell
dotnet add package ViennaNET.KasperskyScanEngine.Client
```

## Руководство пользователя

Добавьте `IKasperskyScanEngineApi` в коллекцию служб.

```csharp
builder.Services.AddKasperskyScanEngineApi(builder.Configuration);
```

Определите конфигурацию в appsettings.json или с использованием любого другого поставщика конфигурации,
по умолчанию конфигурация ожидается в секции: `Endpoints:KasperskyScanEngine`, как определено
в `KseClientOption.SectionName`.

```json
{
    "Endpoints": {
        "KasperskyScanEngine": {
            "BaseAddress": "https://kse.example.ru/api/v3.0/"
        }
    }
}
```

> ⚠️ Cуффикс api/{ver}/ должен быть указан.

Вы можете указать другую секцию из которой необходимо считывать параметры конфигурации.

```csharp
builder.Services.AddKasperskyScanEngineApi(builder.Configuration, 
    opt => builder.Configuration.GetSection("MyKseOptions").Bind(opt));
```

Когда конфигурация определена, внедрите `IKasperskyScanEngineApi` в конструкторе целевого класса
и отправьте запрос, например:

```csharp
var content = Convert.ToBase64String(File.ReadAllBytes("test.txt"));
var response = await api.PostScanMemoryAsync(new ScanMemoryRequest(content, "test.txt"));

if(response.IsDetect)
{
    // предпринимаем меры.
}

if(response.IsClean)
{
    // Всё хорошо.
}

```

Если [KSE][kse] требует аутентификации и авторизации с помощью токена определите его в конфигурации.

```json
{
    "Endpoints": {
        "KasperskyScanEngine": {
            "BaseAddress": "https://kse.example.ru/api/v3.0/",
            "AuthorizationToken": "SldYQTUyOUNVMnE3VWR2N3Izamk2QkVNc2hhLTV5dTBLcVUzeXZLdGYtNkkrVFUyQUVRQUNLQUFCSWdwRUlJTQ=="
        }
    }
}
```

После указание параметра `AuthorizationToken`, все запросы к [KSE][kse] будут включать
заголовок `Authorization: Bearer {ваш токен}`.

Остальные параметры HTTP-клиента смотрите в [ViennaNET.Extensions.Http][viennanet-ext-http-docs].

[kav-protocol]: <https://support.kaspersky.com/help/ScanEngine/2.1/ru-RU/181167.htm> "О протоколе KAV"
[kse]: <https://support.kaspersky.com/help/ScanEngine/2.1/ru-RU/192968.htm> "О Kaspersky Scan Engine"
[viennanet-ext-http-docs]: <../ViennaNET.Extensions.Http/README.md>
[client-opt-base]: <../ViennaNET.Extensions.Http/ClientOptionsBase.cs>
