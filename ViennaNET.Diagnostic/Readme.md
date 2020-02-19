# Build with classes and interfaces to provide centralized application diagnostics

### Build entities:
* HealthCheckingService - Root service for centralized application diagnostics call
* DiagnosticFailedDelegate - Delegate for the diagnostic failure event

If at least one diagnostic check fails, the DiagnosticFailedEvent event, declared in the HealthCheckingService, will be triggered. If necessary, you can subscribe to it, and, for example, stop listening to the queue.