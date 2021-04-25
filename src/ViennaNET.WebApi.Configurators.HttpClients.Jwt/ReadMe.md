# Assembly with the settings of the Http client with authorization through JWT

JwtHttpClientsConfigurator - configurator for registering Http clients using JWT authorization by forwarding Authorization header to outgoing Http requests

Section in the configuration file:

	"webApiEndpoints": [
		{
			"name": "<name>",
			"url": "<base_address>",
			"timeout": <timeout in seconds>, // optional parameter, 30 seconds by default
			"authType": "jwt"
		}
	],