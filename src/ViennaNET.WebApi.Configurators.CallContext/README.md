# Project with call context integration configurator

## CallContextConfigurator
Configurator for register and configure call context using in WebApi application.

*  CallContextMiddleware - middleware to control lifecycle of HttpCallContext based on incomming Http-request

*  HttpCallContext - implementation of ICallContext, which built from incomming Http-request
*  HttpCallContextAccessor - implementation of ICallContextAccessor, that holds HttpCallContext
