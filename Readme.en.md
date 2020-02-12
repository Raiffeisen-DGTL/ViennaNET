# Repo, containing .NET Standard based infrastructure libs

### Table of contents


#### WebApi
*  [**Company.WebApi.Core**](Company.WebApi.Core/Readme.en.md) - main service library. Contains WebApi-service builder with common AspNetCore & Swagger features enabled
*  [**Company.WebApi.Core.DefaultConfiguration**](Company.WebApi.Core.DefaultConfiguration/Readme.en.md) - all-in-one app configuration tools (Kestrel, SimpleInjector, log4net, JWT, Middleware, Handlers, etc ) and a pre-configured builder
*  [**Company.WebApi.Core.DefaultHttpSysRunner**](Company.WebApi.Core.DefaultHttpSysRunner/Readme.en.md) - Windows web-api configuration tools with integrated NTLM auth and run-as-windows-service hosting
*  **Company.HttpClient** - Http-client builder, integrated in standard AspNetCore DI-container

#### Security
* **Company.Security** - base security interface abstractions library
* **Company.Security.Jwt** - token factory 

#### Logging
* **Company.Logging** - a logging library based on log4net