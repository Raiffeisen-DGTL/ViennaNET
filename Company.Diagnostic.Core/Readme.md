# Сборка с классами и интерфейсами для обеспечения централизованной диагностики приложения

### Сущности сборки:
*  HealthCheckingService - Корневой сервис для централизованного вызова диагностики приложения
*  DiagnosticFailedDelegate - Делегат для события о непрохождении диагностики

В случае, если хотя бы одна диагностическая проверка не прошла, то произойдет срабатывание события DiagnosticFailedEvent, объявленного в HealthCheckingService. При необходимости на него можно подписаться, и, например, прекращать слушать очередь.

___

# Assembly for centralized app diagnostics

### Сущности сборки:
*  HealthCheckingService - Root service for centralized app diagnostics features
*  DiagnosticFailedDelegate - A delegate for health check failure logic

In case, when at least one diagnostics check failed, a DiagnosticFailedEvent is fired (this event is being declared in HealthCheckingService). You could subscribe to this event, and, for example, stop a queue listener.