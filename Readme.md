# Репозиторий с инфраструктурными библиотеками, созданными под .NetStandart

### Содержимое


#### WebApi
*  [**Company.WebApi.Core**](Company.WebApi.Core/Readme.md) - основная сборка для поднятия сервисов. Содержит билдер WebApi-сервисов. Использует только стандартные средства AspNetCore + Swagger
*  [**Company.WebApi.Core.DefaultConfiguration**](Company.WebApi.Core.DefaultConfiguration/Readme.md) - содержит готовый набор конфигураторов (Kestrel, SimpleInjector, log4net, JWT, Middleware, Handlers, ... ) и преднастроенный билдер
*  [**Company.WebApi.Core.DefaultHttpSysRunner**](Company.WebApi.Core.DefaultHttpSysRunner/Readme.md) - конфигураторы для настройки WebApi-приложения под Windows с использованием NTLM и хостингом в качестве Windows-службы
*  **Company.HttpClient** - содержит билдер Http-клиента на основе интеграции со стандартным встроенным в AspNetCore DI-контейнером

#### Security
* **Company.Security** - сборка с базовыми интерфейсами
* **Company.Security.Jwt** - содержит реализацию фабрики токенов на основе NTLM-авторизации, а также ключи для шифровки/расшифровки (они должны перехать в место по-безопаснее)

#### Логирование
* **Company.Logging** - библиотека с логгером на основе log4net