# ViennaNET.Extensions.Configuration.HashicorpVault

Предоставляет API поставщика конфигурации на основе секретов в хранилище Hashicorp Vault.
Позволяет добавить, в качестве источника секрет в формате ключ/значение или JSON.

# Оглавление

* [Введение](#Введение)
* [Установка](#Установка)
* [Руководство пользователя](#Руководство-пользователя)
* [Известные проблемы и ограничения](#Известные-проблемы-и-ограничения)

# Введение

Библиотека добавляет поставщик конфигурации, который поддерживает загрузку секретов указанной версии
или послденей активной версии, с возможностью последующей перезагрузки с заданным интервалом.

# Установка

Добавьте в проект веб-службы сслыку на пакте ViennaNET.Extensions.Configuration.HashicorpVault.

```shell
dotnet add package ViennaNET.Extensions.Configuration.HashicorpVault
```

# Руководство пользователя

Создайте секрет в Vault используя KV Secrets Engine и один из двух форматов представленных ниже.

<details>
<summary>В классическом формате словаря (Key/Value).</summary>

```text
  "SecretKey1": "SecretExampleValueString"
  "SecretKey2": "true"
  "SecretKey3": "123456"
```

Чтобы отразить [иерархию][binding-hierarchies] используйте в качестве разделителя символы: `-`, `.` или `_`.

```text
  "Secret-Example-Key1": "SecretExampleValueString",
  "Secret-Example-Key2": "true",
  "Secret-Example-Key3": "123456"
```

</details>

<details>
<summary>В формате JSON (соответствует формату appsettings.json).</summary>

```json
{
  "Secret": {
    "Example": {
      "Key1": "SecretExampleValueString",
      "Key2": true,
      "Key3": 123456
    }
  }
}
```

</details>

Затем добавьте и настройте поставщик конфигурации.

```csharp

// Для получения параметров HTTP клиента Vault API.
builder.Configuration.AddEnvironmentVariables("VAULT_");

// Средонезависимая конфигурация. Без перезагрузки.
// В итоговый путь будет равен: /kv/appsettings.json
builder.Configuration.AddVault(options => builder.Configuration.Bind(options), "appsettings.json", 1);

// Средозависимая конфигурация, по аналогии с appsettings.Development.json и т. д.
// Без перезагрузки.
builder.Configuration.AddVault(options => builder.Configuration.Bind(options), 
    $"appsettings.{builder.Environment.EnvironmentName}.json");

// Средонезависимая конфигурация. Перезагружается раз в 10 секунд.
// Загружает последнюю доступную версию секрета "some-secret-name".
builder.Configuration.AddVault(options => builder.Configuration.Bind(options),
    "some-secret-name", reloadInterval: TimeSpan.FromSeconds(10));
```

Перед запуском приложения установите значения переменных сред любым, допустимым способом.
Например с помощью ConfigMap и Secret в K8S или с помощью Ansible в не контейнерезированных средах.

| Variable            | Описание                                              | Обязательно |
|---------------------|-------------------------------------------------------|:-----------:|
| VAULT_BASE_ADDRESS  | Адрес службы HashicorpVault, где размещены хранилища. |     ДА      |
| VAULT_APP_ROLE_ID   | Учётные данные клиента: [App Role ID][approle-auth]   |     ДА      |
| VAULT_APP_SECRET_ID | Учётные данные клиента: [App Secret ID][approle-auth] |     ДА      |

Далее используйте платформу [конфигурации][configuration], как обычно.

# Известные проблемы и ограничения

1. Поставщик конфигурации поддерживает только KV Secrets Engine.
2. Каждая версия секрета, считается не изменяемой, поэтому автоматическая перезагрузка возможна,
   только если вы не указываете версию. Тогда, при каждой перезагружке, поставщик запрашивает latest.
3. Если целевая версия отмечена как удалённая, возникает исключение.

[configuration]: <https://learn.microsoft.com/ru-ru/dotnet/core/extensions/configuration>

[binding-hierarchies]: <https://learn.microsoft.com/ru-ru/dotnet/core/extensions/configuration#binding-hierarchies>

[approle-auth]: <https://developer.hashicorp.com/vault/docs/auth/approle>