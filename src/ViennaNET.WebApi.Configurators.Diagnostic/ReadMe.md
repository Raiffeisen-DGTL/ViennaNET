# The assembly that provides the connection of diagnostics to the service

CompanyDiagnosticConfigurator - Registers the DiagnosticController in the application. For the controller to work in DI, IHealthCheckingService and its dependencies must be registered.

EmptyDiagnosticImplementor - an "empty" diagnostic service, necessary for building a dependency tree in DI if no other diagnostic services are found