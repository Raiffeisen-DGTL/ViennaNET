# Assembly for centralized app diagnostics

### Сущности сборки:
*  HealthCheckingService - Root service for centralized app diagnostics features
*  DiagnosticFailedDelegate - A delegate for health check failure logic

In case at least one diagnostics check fails, a DiagnosticFailedEvent is fired (this event is declared in HealthCheckingService). You could subscribe to this event, and, for example, stop a queue listener.