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