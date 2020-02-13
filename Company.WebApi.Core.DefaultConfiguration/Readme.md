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
