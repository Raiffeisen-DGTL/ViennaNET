# Build with basic domain model interfaces for creating domain events.

### Interface Description

* **IEntityEventPublisher** is an entity interface that allows it to send domain events.
* **IEvent** is a marker interface, all domain events must implement it.
* **IEventCollector** - message collector interface. Its implementation is passed to the entity for deferred publication of events.
* **IEventCollectorFactory** - factory interface for creating a message collector.
* **IMessage** - the basic message interface. Specific entity interfaces that can be processed by the mediator are inherited from it. For example **IEvent**