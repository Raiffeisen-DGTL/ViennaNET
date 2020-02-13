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
