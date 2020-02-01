# Сборка с набором конфигураторов для настройки WebApi-приложения

### Набор конфигураторов

#### Сервера
*  KestrelConfigurator - Конфигуратор для регистрации Kestrel в качестве сервера. Для хостинга под Linux
*  HttpSysConfigurator - Конфигуратор для регистрации HttpSys в качестве сервера. Для хостинга под Windows и поддержки NTLM-аутентификации

Секция в файле конфигурации:

		"webApiConfiguration": {
			"portNumber": 9050,
			...
		},

_________________

#### Https
*  HttpsConfigurator - Конфигуратор для включения Https и настройки редиректов в Http на Https

Для включения https необходимо в конфигурационной секции **webApiConfiguration** добавить порт для https **httpsPort**.
Также имеется опция **useStrictHttps** - при установке значения **true**, все запросы по http будут принудительно перенаправляться на https.
Опция **useHsts** позволяет выключить отправку HSTS-заголовка, который активирует принудительные редиректы на HTTPS из браузера к текущему хосту

#### Http-клиенты

##### Обработчики исходящих запросов
*  BaseCompanyRequestHeadersHandler - Обработчик для добавления/проброски следующих заголовков в исходящие Http-запросы

		X-Request-Id
		X-User-Id
		X-User-Domain
		X-Caller-Ip

*  RequestAuthorizationHeaderHandler - Обработчик для проброски Authorization-заголовка в исходящие Http-запросы

##### Конфигураторы
*  JwtHttpClientsConfigurator - Конфигуратор для регистрации Http-клиентов с JwtRequestHeadersHandler
*  NtlmHttpClientsConfigurator - Конфигуратор для регистрации Http-клиентов со специальной настройкой для поддержки NTLM

Секция в файле конфигурации:

		"webApiEndpoints": [
			{
				"name": "<название>",
				"url": "<базовый_адрес>",
				"timeout": <таймаут_в_секундах> // необязательный параметр, 30 секунд по умолчанию
			}
		],

_________________

#### Logging
*  CompanyLoggingConfigurator - Конфигуратор для регистрации логгера из Company.Logging. В конфигурации обязательно нужно указать секцию "logger" с необходимыми параметрами:

		"logger": {
			"listeners": [
				{
					"type": "console",
					"category": "All",
					"minLevel": "Debug"
				},
				{
					"type": "textFile",
					"category": "All",
					"minLevel": "Debug",
					"params": {
						"maxSize": "50000",
						"append": "append",
						"rollBackBackups": "10",
						"filePatternName": "C:\\Logs\\TestCoreService\\TestCoreService_yyyy.MM.dd.log"
					}
				},
				{
					"type": "textFile",
					"category": "All",
					"minLevel": "Error",
					"params": {
						"maxSize": "50000",
						"append": "append",
						"rollBackBackups": "10",
						"filePatternName": "C:\\Logs\\TestCoreService\\TestCoreService_yyyy.MM.dd.err"
					}
				}
			]
		}

_________________

#### Middleware
*  CustomMiddlewareConfigurator - Конфигуратор для регистрации сторонних middleware в приложении и DI контейнере (SimpleInjector)
*  RequestRegistrationMiddleware - Добавляет заголовок X-Request-Id к входящему запросу, если он отсутствует. Вызывается в первую очередь
*  SetUpLoggerMiddleware (LoggingMiddlewareBase) - Заполняет в логгере поля RequestId и User из входящего запроса
*  LogRequestAndResponseMiddleware - Логирует входящий запрос и исходящий ответ сервиса

_________________

#### Security

##### JWT
*  JwtSecurityConfigurator - Конфигуратор для регистрации логики проверки JWT
*  JwtSecurityContextFactory - Реализация ISecurityContextFactory на основе получения данных из токена
*  SecurityContext - Реализация ISecurityContext. Наполняется всеми необходимыми данными через конструктор

Ключи и параметры для шифрования и расшифровки токена можно переопределить в необязательной секции в файле конфигурации:

		"jwtSecuritySettings": {
			"issuer": "<издатель>",
			"audience": "<аудитория>",
			"tokenKeyEnvVariable": "<имя_переменной_окружения>"
		}

Все параметры необязательные.
*  issuer - кто выдает токен, значение по умолчанию "Raiffeisenbank"
*  audience - для кого выдан токен, значение по умолчанию "RaiffeisenbankEmployee"
*  tokenKeyEnvVariable - название переменной окружения, хранящей кодовое слово для шифрования, значение по умолчанию "TOKEN_SECRET_KEY"

> **Внимание!** Если в сервисе формирующем токены были переопределены какие-либо из параметров, то их ьтакже нужно переопределить и в других сервисах

##### NTLM
*  NtlmSecurityConfigurator - Регистрирует во встроенном DI схему аутентификации для NTLM, ISecurityContextFactory и WithPermissionsAttribute
*  NtlmSecurityContextFactory - Реализация ISecurityContextFactory на основе получения данных из заголовков, учетной записи Windows из запроса, либо текущей сервисной учетной записи
*  NtlmSecurityContext - Реализация ISecurityContext с учетом запроса полномочий пользователя из security-сервиса
*  WithPermissionsAttribute - Атрибут авторизации для контроллеров и действий, проверяющий полномочия пользователя

_________________

#### SimpleInjector
*  SimpleInjectorConfigurator - Конфигуратор для интеграции SimpleInjector в сервис. Настройка интеграции со встроенным DI, поиск и инициализация пакетов (инсталлеров), валидация контейнера

_________________

#### Swagger
*  SwaggerJwtAuthConfigurator - Конфигуратор для внедрения JS-файла (swaggerAuth.js) в интерфейс Swagger'а, который автоматически производит авторизацию посредством JWT
> **Внимание!** Токен запрашивается при загрузке страницы, поэтому, когда срок его жизни истечет, нужно перезагрузить страницу, чтобы обновить токен

_________________

#### DefaultKestrelRunner
Создает и запускает **HostBuilder** с вышеперечисленными конфигураторами. Применена JWT-авторизация и сервер Kestrel. Для заапкска сервиса необходимо вызвать BuildWebHost() и, затем, Run()

___

# Assembly with a set of configurators for setting up a WebApi application

### Available configurators

#### Web servers
*  KestrelConfigurator - a configurator for registering Kestrel as web-server. Use it for hosting under Linux
*  HttpSysConfigurator - a configurator for registering HttpSys as web-server. Use it for hosting under Windows and support for NTLM authentication

Config section:

		"webApiConfiguration": {
			"portNumber": 9050,
			...
		},

_________________

#### Https
*  HttpsConfigurator - configurator, that enables https and sets up http to https redirection.

To enable https, you need to add the port for https in **httpsPort** option in the **webApiConfiguration** configuration section.
There is also a **useStrictHttps** option - when setting its value to **true**, all requests via http will be forcibly redirected to https.
**useHsts** option allows you to disable sending the HSTS header, which activates forced redirects to HTTPS from the browser to the current host

#### Http-clients

##### Incoming request handlers
*  BaseCompanyRequestHeadersHandler - a handler for adding / forwarding the following headers to outgoing Http requests

		X-Request-Id
		X-User-Id
		X-User-Domain
		X-Caller-Ip

*  RequestAuthorizationHeaderHandler - a handler for authorization header forwarding to outgoing Http requests

##### Configurators
*  JwtHttpClientsConfigurator - configurator for registering HTTP clients with JwtRequestHeadersHandler
*  NtlmHttpClientsConfigurator - configurator for registering HTTP clients, tuned for NTLM support

Config section:

		"webApiEndpoints": [
			{
				"name": "<name>",
				"url": "<base address>",
				"timeout": <timeout_in_seconds> // optional, 30 seconds by default
			}
		],

_________________

#### Logging
*  CompanyLoggingConfigurator - configurator for registering a logger from Company.Logging. In the configuration file, you must setup the "logger" section with the necessary parameters:

		"logger": {
			"listeners": [
				{
					"type": "console",
					"category": "All",
					"minLevel": "Debug"
				},
				{
					"type": "textFile",
					"category": "All",
					"minLevel": "Debug",
					"params": {
						"maxSize": "50000",
						"append": "append",
						"rollBackBackups": "10",
						"filePatternName": "C:\\Logs\\TestCoreService\\TestCoreService_yyyy.MM.dd.log"
					}
				},
				{
					"type": "textFile",
					"category": "All",
					"minLevel": "Error",
					"params": {
						"maxSize": "50000",
						"append": "append",
						"rollBackBackups": "10",
						"filePatternName": "C:\\Logs\\TestCoreService\\TestCoreService_yyyy.MM.dd.err"
					}
				}
			]
		}

_________________

#### Middleware
*  CustomMiddlewareConfigurator - a configurator for registering third-party middleware in the application and DI container (SimpleInjector)
*  RequestRegistrationMiddleware - adds an X-Request-Id header to an incoming request, if one is missing. This middleware is the first one being called.
*  SetUpLoggerMiddleware (LoggingMiddlewareBase) - fills the RequestId and User fields in incoming requests
*  LogRequestAndResponseMiddleware - this middleware logs incoming requests and outgoing service responses

_________________

#### Security

##### JWT
*  JwtSecurityConfigurator - a configurator for registering JWT validation logic
*  JwtSecurityContextFactory - ISecurityContextFactory implementation based to data recieved in tokens
*  SecurityContext - ISecurityContext implementation. All nesessary data is supplied in constructor.

	The keys and parameters for encrypting and decrypting the token can be redefined in the optional section in the configuration file:

		"jwtSecuritySettings": {
			"issuer": "<token issuer>",
			"audience": "<token audience>",
			"tokenKeyEnvVariable": "<environment variable name>"
		}

All parameters are optional.
*  issuer - token issuer, default value is "Raiffeisenbank"
*  audience - token audience, default value "RaiffeisenbankEmployee"
*  tokenKeyEnvVariable - environment variable name, storing encription code word, default value is "TOKEN_SECRET_KEY"

> **NOTE!** If any parameter was redefined in the token-generating service, it need to be redefined in other services

##### NTLM
*  NtlmSecurityConfigurator - registers in the internal DI NTLM auth schema, ISecurityContextFactory and WithPermissionsAttribute
*  NtlmSecurityContextFactory - ISecurityContextFactory implementation using request headers data, Windows account from the request, or current service account
*  NtlmSecurityContext - ISecurityContext implementation with user rights check from security service
*  WithPermissionsAttribute - an authorization attribute for controllers & actions, use it to check user rights

_________________

#### SimpleInjector
*  SimpleInjectorConfigurator - Configurator for SimpleInjector integration. It setups internal DI integration, searches & initializes packages (installers) and validates the DI container

_________________

#### Swagger
*  SwaggerJwtAuthConfigurator - Configurator for JS-file (swaggerAuth.js) generation for Swagger UI with JWT auth enabled.
> **NOTE!** Token is specified on the page load, so when it expires, you need to reload the page to refresh the token.

_________________

#### DefaultKestrelRunner
Creates and launches **HostBuilder** with the above configurators. Применена JWT-авторизация и сервер Kestrel. For service launch you need to call BuildWebHost() and Run() methods.
