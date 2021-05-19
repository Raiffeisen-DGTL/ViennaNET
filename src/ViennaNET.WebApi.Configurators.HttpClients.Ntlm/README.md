# Assembly with the settings of the HTTP client with authorization through NTLM

NtlmHttpClientsConfigurator - a configurator for registering Http clients with special settings to support NTLM

Section in the configuration file:

	"webApiEndpoints": [
		{
			"name": "<name>",
			"url": "<base_address>",
			"timeout": <timeout in seconds>, // optional parameter, 30 seconds by default
			"authType": "ntlm"
		}
	],