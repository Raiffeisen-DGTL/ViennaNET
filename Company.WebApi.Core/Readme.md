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
