# ViennaNET.Extensions.Http

Библиотека предоставляет API для типовой настройки типизированных и именнованных HTTP клиентов с использованием фабрики
[IHttpClientFactory][http-client-factory]. Облегчаются задачи по добавлению политик повтора и таймайту политик,
настройки
способа аутентификации, ведения журнала.

## Оглавление

* [Введение](#введение)
* [Установка](#установка)
* [Руководство пользователя](#руководство-пользователя)

## Введение

API построен по принципу расширения платформы .NET. Библиотека добавляет метод расширения AddHttpClient и абстрактный
класс ClientOptionsBase,
для обеспечения унифицированного способа конфигурации [типизированных][typed-httpclient]
или [именованных][named-http-client] HTTP клиентов.
Однако, она будет вам полезной, даже если вы не используете этот вариант. В библиотеке объявлены API расширяющие
IHttpClientBuilder,
UseNegotiateAuthentication, AddLoggingHttpMessageBodyHandler, AddDefaultPolicyRegistry и др... Вы можете использовать
их, для настройки
любых реализаций HTTP Client использующих [IHttpClientFactory][http-client-factory].

## Установка

Добавьте в проект сслыку на пакте ViennaNET.Extensions.Http.

```shell
dotnet add package ViennaNET.Extensions.Http
```

## Руководство пользователя

Рекомендуем использовать подход с [типизированными][typed-httpclient] HTTP клиентами.

Объявите интерфейс клиента и его реализацию, как описано в документации по [типизированным клиентам][typed-httpclient].
Объявите класс MyHttpClientOptions который должен быть наследником базового класса ClientOptionsBase.

```csharp
public class MyHttpClientOptions : ClientOptionsBase
{
    // Рекомендую использовать общую секцию DefaultSectionRootName для всех создаваемых клиентов, 
    // добавляя вложенную секцию как в этом примере. Итоговая секция в примере будет: Endpoints:MyApi.
    public const string SectionName = DefaultSectionRootName + "MyApi";
    
    // Если нужно, то расширяйте его дополнительными свойствами.
}
```

Далее в классе Program/Startup или в любом другом, где вы регистрируете службы в DI, выполните код.
Пример с WebApplication подходом.

```csharp
builder.Services
    .AddHttpClient<IMyApiClient, MyApiClient, MyApiClientOptions>(
        options => _builder.Build().GetSection(TestHttpClientOptions.SectionName).Bind(options))
     .UseNegotiateAuthentication<TestHttpClientOptions>(); // включаем аутентификацию Negotiate
```

В параметрах конфигурации приложения укажите:

```json
{
  "Endpoints": {
    "MyApi": {
      "BaseAddress": "https://tst.raif.ru:5001",
      "Authentication": {
        "Negotiate": {
          "UseDefaultCredentials": true
        }
      }
    }
  }
}
```

Далее получите IMyApiClient в конструкторе любого другого типа и используйте как обычно.
В данном примере, вы получите HttpClient с настроенными политиками повтора и таймаут политиками по умолчанию, и
аутентификацией Negotiate.

Чтобы добавить журналирование тела запроса и ответа допишите код.

```csharp
builder.Services
    .AddHttpClient<IMyApiClient, MyApiClient, MyApiClientOptions>(
        options => _builder.Build().GetSection(TestHttpClientOptions.SectionName).Bind(options))
     .UseNegotiateAuthentication<TestHttpClientOptions>() // включаем аутентификацию Negotiate
     .AddLoggingHttpMessageBodyHandler(); // включаем журналирование тела сообщений. Максимальная длина 100 000 символов.
```

Затем настройте уровень ведения журнала на Trace. Например:

> ⚠️ Запись тела сообщения поддерживается только на уровне Trace. Не допускайте этого, в производственной среде!

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace"
    }
  }
}
```

### Параметры конфигурации

| Параметр                                       | Описание                                                                                                                                                                                                                         | Значение по умолчанию |                                Обязательный                                 |
|------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:---------------------:|:---------------------------------------------------------------------------:|
| BaseAddress                                    | Базовый адрес службы, с которой общается клиент.                                                                                                                                                                                 |                       |                                     ДА                                      |
| UseReplayPolicy                                | Если true, клиент будет использовать политики повтора.                                                                                                                                                                           |         false         |                                     НЕТ                                     |
| UseReplayPolicyOnlyIdempotentRequest           | Если true, клиент будет использовать политики повтора только для идемпотентных запросов (GET, HEAD, PUT, DELETE)                                                                                                                 |         true          |                                     НЕТ                                     |
| UseTimeoutPolicy                               | Если true, клиент будет использовать политику, которая будет асинхронно ожидать завершения делегата в течение заданного периода времени TryTimeout.                                                                              |         false         |                                     НЕТ                                     |
| OverallTimeout                                 | Общее время ожидания для всех попыток. В секундах. Этот параметр определяет [HttpClient.Timeout][httpclient-timeout]. Который определяет общее время ожидания. Учитывайте это при настройки TryTimeout, RetryDelay и RetryCount. |  2 минуты (120 сек)   |                                     НЕТ                                     |
| TryTimeout                                     | Время ожидания для каждой отдельной попыти. В секундах.                                                                                                                                                                          |        10 сек         |                                     НЕТ                                     |
| RetryDelay                                     | Время задержки первой повторной попытки в секундах.                                                                                                                                                                              |         2 сек         |                                     НЕТ                                     |
| RetryCount                                     | Максимальное количество повторных попыток.                                                                                                                                                                                       |           6           |                                     НЕТ                                     |
| Authentication:Negotiate:UseDefaultCredentials | Если true, то используются учётные данные текущего пользователя, выполнившего вход на узле. Параметры UserName и Password игнорируются.                                                                                          |         true          |                 ДА (если вызван UseNegotiateAuthentication)                 |
| Authentication:Negotiate:UserName              | Имя пользователя, от лица которого необходимо выполнить запрос.                                                                                                                                                                  |                       | ДА (если вызван UseNegotiateAuthentication и UseDefaultCredentials = false) |
| Authentication:Negotiate:Password              | Пароль от учётной записи UserName.                                                                                                                                                                                               |                       | ДА (если вызван UseNegotiateAuthentication и UseDefaultCredentials = false) |


[http-client-factory]: <https://learn.microsoft.com/ru-ru/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests>

[named-http-client]: <https://learn.microsoft.com/ru-ru/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0#named-clients>

[typed-httpclient]: <https://learn.microsoft.com/ru-ru/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0#typed-clients>

[httpclient-timeout]: <https://learn.microsoft.com/ru-ru/dotnet/api/system.net.http.httpclient.timeout?view=net-7.0>
