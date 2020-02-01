# Сборка с набором конфигураторов для настройки WebApi-приложения под Windows с использованием NTLM

### Набор конфигураторов

#### Сервера
*  HttpSysConfigurator - Конфигуратор для регистрации HttpSys в качестве сервера. Для хостинга под Windows и поддержки NTLM-аутентификации

Секция в файле конфигурации:

		"webApiConfiguration": {
			"portNumber": 9050,
			"corsOrigins": ["http://...", ...] // необязательный параметр, позволяет ограничить кроссдоменные запросы. Если не указан, то ограничений нет (*)
			...
		},

> **Внимание!** Имеется некоторая особенность: Если к сервису будет обращаться клиент из **браузера** по NTLM, то обязательно нужно прописать его (клиента) адрес хоста в параметр **additionalCorsOrigins**, а также добавить **"*"**, чтобы данный сервис мог ходить к другим сервисам. Например:

		"corsOrigins": ["http://s-msk-t-icdb999", "*"]

_________________

#### Http-клиенты

##### Конфигураторы
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

#### Security

##### NTLM
*  NtlmSecurityConfigurator - Регистрирует во встроенном DI схему аутентификации для NTLM, ISecurityContextFactory и WithPermissionsAttribute
*  NtlmSecurityContextFactory - Реализация ISecurityContextFactory на основе получения данных из заголовков, учетной записи Windows из запроса, либо текущей сервисной учетной записи
*  NtlmSecurityContext - Реализация ISecurityContext с учетом запроса полномочий пользователя из security-сервиса
*  WithPermissionsAttribute - Атрибут авторизации для контроллеров и действий, проверяющий полномочия пользователя

_________________

#### DefaultHttpSysRunner
Создает и запускает **HostBuilder** с конфигураторами из сборки Company.WebApi.Core.DefaultConfiguration. Применена NTLM-авторизация и сервер HttpSys. Для запуска сервиса необходимо вызвать BuildAndRun(). Если в данный метод пробросить параметр --install, то приложение запустится в режиме Windows-сервиса, однако предварительно его нужно зарегистрировать в системе
> **Внимание!** Имеется некоторая особенность: при запуске в окружении .Net Core NTLM не работает (нужно что-то дополнительно делать с заголовками Http-клиентов), в окружении .Net Framework - все отлично

___

# A setup assembly for WebApi-app configuration with Windows and NTLM auth

### Available confugurators

#### Web servers
*  HttpSysConfigurator - a configurator for registering HttpSys as a server. Used for Windows hosting and NTLM-authentication support

Configuration:

		"webApiConfiguration": {
			"portNumber": 9050,
			"corsOrigins": ["http://...", ...] // optional, this parameter allowes to restrict cross-domain requests. If not supplied, no restrictions apply (*)
			...
		},

> **NOTE!** If a **browser** client calls the service with NTLM auth enabled, you should add its (client) host address in **additionalCorsOrigins** param, and add **"*"**, so that the service could call other services. Example:

		"corsOrigins": ["http://s-msk-t-icdb999", "*"]

_________________

#### Http-clients

##### Configurators
*  NtlmHttpClientsConfigurator - a configurator for Http-clients with integrated NTLM auth setting

Config section:

		"webApiEndpoints": [
			{
				"name": "<name>",
				"url": "<base address>",
				"timeout": <timeout_in_seconds> // optional, 30 seconds by default
			}
		],
_________________

#### Security

##### NTLM
*  NtlmSecurityConfigurator - configurator, that registers in integrated DI a NTLM-based authentication schema, ISecurityContextFactory and WithPermissionsAttribute
*  NtlmSecurityContextFactory - ISecurityContextFactory implementation based on request headers data, Windows account from the request, or current service account
*  NtlmSecurityContext - ISecurityContext implementation with user rights check from security service
*  WithPermissionsAttribute - an authorization attribute for controllers & actions, use it to check user rights

_________________

#### DefaultHttpSysRunner
A **HostBuilder** with Company.WebApi.Core.DefaultConfiguration assembly configurators. NTLM-authorization and HttpSys are applied by default. To run the app you need to call BuildAndRun() method. If an --install flag is added, your app will run as Windows service, but you need to install it beforehand
> **NOTE!** There is a catch: when being launched in .Net Core environment NTLM does not work (something needs to be done with Http-client headers), but in .Net Framework env - it works just fine
