# Assembly with general parameters of Http clients

Contains:
* CompanyHttpClientsTypes - dictionary with authorization types
* WebapiEndpointsSection - description of the Http client section in the configuration file
* RestEndpointsChecker - diagnoses all Http connections to other services
* BaseCompanyRequestHeadersHandler - a handler for adding / forwarding the following headers to outgoing Http requests:

	X-Request-Id
	X-user-id
	X-User-Domain
	X-caller-ip