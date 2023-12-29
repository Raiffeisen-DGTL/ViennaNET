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

Logging: see [Logging in .NET Core and ASP.NET Core][logging]

[logging]: <https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-8.0>