# Repo, containing .NET Standard based infrastructure libs


### Table of contents


#### WebApi
*  [**ViennaNET.WebApi**](ViennaNET.WebApi) - main service library. Contains WebApi-service builder with common AspNetCore & Swagger features enabled
*  [**ViennaNET.WebApi.Configurators.Common**](ViennaNET.WebApi.Configurators.Common) - all-in-one app configuration tools (Kestrel, SimpleInjector, log4net, JWT, Middleware, Handlers, etc ) and a pre-configured builder
*  [**ViennaNET.WebApi.Runners.BaseHttpSys**](ViennaNET.WebApi.Runners.BaseHttpSys) - Windows web-api configuration tools with integrated NTLM auth and run-as-windows-service hosting
*  **ViennaNET.HttpClient** - Http-client builder, integrated in standard AspNetCore DI-container

#### Security
* **ViennaNET.Security** - base security interface abstractions library
* **ViennaNET.Security.Jwt** - token factory 

#### Logging
* **ViennaNET.Logging** - a logging library based on log4net

#### Useful Utilities
* **ViennaNET.Utils** - contains useful extension methods and attributes
* [**ViennaNET.Validation**](ViennaNET.Validation) - implementation of validation services
