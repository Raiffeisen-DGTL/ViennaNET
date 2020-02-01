# Основная сборка для поднятия сервисов. Содержит билдер WebApi-сервисов. Использует только стандартные средства AspNetCore + Swagger

### Инструкция по применению

Основной класс - HostBuilder. Разбит на 3 файла:
*  HostBuilder.cs - содержит поля, хранящие настройки и собственно сам метод Build
*  HostBuilderActions.cs - содержит методы для заполнения билдера
*  HostBuilderValidation.cs - содержит метод валидации состояния билдера

Неконфигурируемые настройки:
*  Подключение Swagger
*  JSON-конфигурация

#### Настройка и запуск минимального сервиса:
1.  Создаем консольное приложение на .Net Core или Framework.
2.  Добавляем файл конфигурации **appsettings.json**, как видно, он содержит лишь настройку включения Swagger:

		{
		  "webApiConfiguration": {
			"swaggerSubmit": true
		  }
		}

3.  В Program.cs создаем экземпляр HostBuilder через Create().
4.  Так как сервер изначально не указан, то нужно это обязательно сделать. Например для подключения Kestrel (дополнительно пакет скачивать не нужно, так как Microsoft, зачем-то включила его в базовый метапакет для AspNetCore) с базовыми настройками достаточно вызвать метод билдера **UseServer((b, c) => { b.UseKestrel(); })**.
5.  Вызываем BuildWebHost()
6.  Вызываем Run()

Вот и всё, мы получили минимальный сервис, который запускается и работает.

_________________


### Список методов конфигурирования сервиса

*  AddMvcBuilderConfiguration - Позволяет дополнительно сконфигурировать MVC Builder
*  ConfigureApp - Конфигурирование внутреннего билдера приложения, здесь добавляются Middleware, применяется Cors и т.п.
*  AddMiddleware - Регистрирует стандартное middleware, без привязки к стороннему контейнеру
*  CreateContainer - Функция для создания объекта DI-контейнера. Если не была вызвана, то используется встроенный контейнер
*  VerifyContainer - Функция для вызова валидации стороннего контейнера
*  ConfigureContainer(Func) - Кофигурирует сторонний контейнер для **замещения** встроенного. Такой подход используют, например в Autofac
*  ConfigureContainer(Action) - Кофигурирует сторонний контейнер **без** замещения встроенного. Такой подход используют, например в SimpleInjector
*  InitializeContainer - Инициализация стороннего контейнера
*  RegisterServices - Регистрация сервисов в стандартном DI
*  UseServer - Настройка сервера (Kestrel, HttpSys, ...)
*  AddOnStartAction - Добавляет операцию, вызываемую после старта сервиса
*  AddOnStopAction - Добавляет операцию, вызываемую после остановки сервиса
*  AddHealthChecks - Добавляет HealthCheck-и (https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)

### Диагностика
Для проверки состояния сервиса есть три URL:
*  **/ping** - просто пингует сервис
*  **/diagnostic/diagnose** - запускает все зарегистрированные HealthCheck-и и возвращает результат проверок. Возможные ответы: 200 - если все успешно, 503 - если хотя бы одна проверка не прошла, при этом всегда возвращается json-объект с результатами. Требует авторизации
*  **/diagnostic/service-diagnose** - работает аналогично /diagnostic/diagnose, но не возвращает структуру с подробной информацией о системе, только код ответа. Не требует авторизации

### Мониторинг
Для получения статистики по запросам и ответам сервиса есть два URL:
*  **/metrics** - возвращяет JSON-объект со статистикой
*  **/metrics-text** - возвращяет строковый эквивалент статистики

Для включения мониторинга в конфиг необходимо добавить секцию в конфигурационный файл

		{
		  "metrics": {
			  "enabled": true,
        "reporter": "default"
		  }
		}

**enabled** - включает и отключает сбор (по умолчанию false)
**reporter** - необязательное поле, указывается формат ответа:

*  "default" - базовый формат (значение по умолчанию)
*  "prometheus" - формат для приложения Prometheus. Переопределяется только результат по URL "/metrics-text"

___

# Base service assembly. Enables WebApi-app builder with standard AspNetCore + Swagger features used

### Contents

Base class - HostBuilder. It is split into 3 files:
*  HostBuilder.cs - contains setup properties and the main Build method
*  HostBuilderActions.cs - contains methods for build configuration
*  HostBuilderValidation.cs - contains app-builder state validation methods

Non-configurable features:
*  Swagger
*  JSON-configuration

#### Minimal service setup and launch:
1.  Create a simple console app, using .Net Core or Framework template.
2.  Add configuration file **appsettings.json** (as you can see, it only contains enabled swagger feature):

		{
		  "webApiConfiguration": {
			"swaggerSubmit": true
		  }
		}

3.  In Program.cs create HostBuilder object, using Create() method.
4.  As web server is not specified by default, you need to configure it. Ex: for using Kestrel (no additional packages are needed, as Microsoft included it in basic AspNetCore) with base settings you are to call builder method **UseServer((b, c) => { b.UseKestrel(); })**.
5.  Call BuildWebHost()
6.  Call Run()

And voila, you have a basic service.

_________________


### Service configuration methods

*  AddMvcBuilderConfiguration - this method enables additional MVC Builder configuration
*  ConfigureApp - internal builder configurator, for adding middleware, enabling Cors, etc.
*  AddMiddleware - standard middleware registration, without any third party container usage
*  CreateContainer - this method is used to create third-party DI-container object. If you do not call this method, an internal container will be used
*  VerifyContainer - third-party DI container validation 
*  ConfigureContainer(Func) - third-party container configuration for **replacing** the internal one. Ex: Autofac
*  ConfigureContainer(Action) - third-party container configuration **without replacing** the internal one. Ex: SimpleInjector
*  InitializeContainer - third-party DI container initialization
*  RegisterServices - service registration in standard DI container
*  UseServer - Web server configuration (Kestrel, HttpSys, etc)
*  AddOnStartAction - this method adds operation, that will be called when the app starts
*  AddOnStopAction - this method adds operation, that will be called when the app stops
*  AddHealthChecks - this method adds HealthChecks (https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)

### Diagnostics
For app diagnostics you have following 3 endpoints:
*  **/ping** - a simple service availability check operation
*  **/diagnostic/diagnose** - all registered in-app HealthChecks will be launched. Possible outcomes: 200 - all is ok, 503 - at least one health check failed. A json-object with check result will be returned. Only for authorized calls
*  **/diagnostic/service-diagnose** - works similarto /diagnostic/diagnose, but does not return josn with app state, just code. Authorization is not needed

### Monitoring
To get request and response metrics, you have these endpoints:
*  **/metrics** - returns a JSON-object, containing statistics
*  **/metrics-text** - returns a string, containing metrics info

To enable monitoring, you need to add this section in app configuration file:

		{
		  "metrics": {
			  "enabled": true,
        "reporter": "default"
		  }
		}

**enabled** - enable/disable metrics collection (false by default)
**reporter** - optional, answer format:

*  "default" - base format (by default)
*  "prometheus" - Prometheus-ready format. Only "/metrics-text" endpoint is affected 
