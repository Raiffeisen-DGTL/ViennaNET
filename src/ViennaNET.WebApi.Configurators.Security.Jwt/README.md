# Build with JWT-based authorization services

Contains:
* JwtSecurityConfigurator - configurator for registering JWT verification logic
* JwtSecurityContextFactory - implementation of ISecurityContextFactory based on receiving data from a token
* SecurityContext - implementation of ISecurityContext. Filled with all the necessary data through the constructor
* JwtGenerationConfigurator - configurator for registering the JWT token generation service (for use in authorization services)

The keys and parameters for encrypting and decrypting the token can be redefined in the optional section in the configuration file:

	"jwtSecuritySettings": {
		"issuer": "<publisher>",
		"audience": "<audience>",
		"tokenKeyEnvVariable": "<environment variable_name>"
	}

All parameters are optional.
* issuer - who issues the token, the default value is "Raiffeisenbank"
* audience - for whom the token is issued, the default value is "RaiffeisenbankEmployee"
* tokenKeyEnvVariable - the name of the environment variable that stores the code word for encryption, the default value is "TOKEN_SECRET_KEY"

> **Attention!** If any of the parameters were redefined in the service generating the tokens, then they also need to be redefined in other services


# Build with JWT-based authorization by ASP.NET Core default mechanism services 

If you need use this authorization, then call ICompanyHostBuilder extension method with name 'UseDefaultJwtBearer'

In order to use this authorization, you must add to service configuration new section with name 'Bearer' whose structure is identical [JwtBearerOptions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.jwtbeareroptions?view=aspnetcore-1.1&viewFallbackFrom=aspnetcore-6.0)
