# Репозиторий с инфраструктурными библиотеками, созданными под .NET Standard

### Содержимое


#### WebApi
*  [**Company.WebApi.Core**](Company.WebApi.Core/Readme.md) - основная сборка для поднятия сервисов. Содержит билдер WebApi-сервисов. Использует только стандартные средства AspNetCore + Swagger
*  [**Company.WebApi.Core.DefaultConfiguration**](Company.WebApi.Core.DefaultConfiguration/Readme.md) - содержит готовый набор конфигураторов (Kestrel, SimpleInjector, log4net, JWT, Middleware, Handlers, ... ) и преднастроенный билдер
*  [**Company.WebApi.Core.DefaultHttpSysRunner**](Company.WebApi.Core.DefaultHttpSysRunner/Readme.md) - конфигураторы для настройки WebApi-приложения под Windows с использованием NTLM и хостингом в качестве Windows-службы
*  **Company.HttpClient** - содержит билдер Http-клиента на основе интеграции со стандартным встроенным в AspNetCore DI-контейнером

#### Security
* **Company.Security** - сборка с базовыми интерфейсами
* **Company.Security.Jwt** - содержит реализацию фабрики токенов

#### Логирование
* **Company.Logging** - библиотека с логгером на основе log4net
___

# Repo, containing .NET Standard based infrastructure libs

### Table of contents


#### WebApi
*  [**Company.WebApi.Core**](Company.WebApi.Core/Readme.md) - main service library. Contains WebApi-service builder with common AspNetCore & Swagger features enabled
*  [**Company.WebApi.Core.DefaultConfiguration**](Company.WebApi.Core.DefaultConfiguration/Readme.md) - all-in-one app configuration tools (Kestrel, SimpleInjector, log4net, JWT, Middleware, Handlers, ... ) and a pre-configured builder
*  [**Company.WebApi.Core.DefaultHttpSysRunner**](Company.WebApi.Core.DefaultHttpSysRunner/Readme.md) - Windows web-api configuration tools with integrated NTLM auth and run-as-windows-service hosting
*  **Company.HttpClient** - Http-client builder, integrated in standard AspNetCore DI-container

#### Security
* **Company.Security** - base security interface abstractions library
* **Company.Security.Jwt** - token factory 

#### Logging
* **Company.Logging** - a logging library based on log4net