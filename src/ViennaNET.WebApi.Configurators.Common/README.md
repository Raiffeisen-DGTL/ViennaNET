# Build with a basic configurator to configure a WebApi application

## CommonConfigurator
Configurator for registering and configuring services and middleware in the application:

Services from the ViennaNET.WebApi.Net library:
* LocalIpProvider - a service for obtaining a local IPv4 address
* LoopbackIpFilter - a service for converting a local IP address (loop) into a real one

Middleware software:
* RequestRegistrationMiddleware - Adds an X-Request-Id header to an incoming request if it is missing. Called First
* SetUpLoggerMiddleware (LoggingMiddlewareBase) - Fills the RequestId and User fields from the incoming request in the logger
* LogRequestAndResponseMiddleware - Logs an incoming request and an outgoing service response

Logging:
For the logger from the ViennaNET.Logging library to work in the configuration, you must specify the "logger" section with the necessary parameters, for example:

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
