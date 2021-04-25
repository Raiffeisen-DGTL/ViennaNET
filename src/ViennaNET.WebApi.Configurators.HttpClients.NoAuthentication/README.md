# Build with HTTP client settings without authorization

NoAuthenticationHttpClientsConfigurator - configurator for registration Http clients do not use authorization through forwarding

Section in the configuration database:

```
		"webApiEndpoints": [
			{
				"name": "<name>",
				"url": "<base address>",
				"timeout": <timeout in seconds>, // optional parameter, 30 seconds by default
				"authType": "noauth"
			}
		],
```