# Build that provides Swagger connection with automatic receipt of JWT from NTLM authorization service

* SwaggerJwtAuthConfigurator - a configurator for embedding a JS file (swaggerAuth.js) into Swagger's interface, which automatically performs authorization through JWT
* SwaggerConfigurationSection - configuration section, extends the base section from ViennaNET.WebApi.Configurators.Swagger, adding an authorization service url for receiving JWT and http verb for request

> ** Attention! ** A token is requested when loading a SwaggerUi page, therefore, when it expires, you need to reload the page to update the token