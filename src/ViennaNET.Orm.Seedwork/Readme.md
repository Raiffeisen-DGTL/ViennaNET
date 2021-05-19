# Build with basic domain model interfaces for creating domain entities.

### Interface Description

* **IEntityKey** - entity marker interface. Without it, it will not be possible to register the entity in a bounded context.
* **IEntityRepository** - interface for the repository working with an abstract collection of objects.
* **IBoundedContext** - interface for registering entities and value objects. Required to register the domain part of the limited application context.
* **IEntityRepositoryFactory** - factory interface for creating **IEntityRepository**. Implementation is necessary if the repository is supposed to be made a separate instance at the user's request.

### Description of base classes

* **BoundedContext** is a base class that represents an abstraction of a limited context. A container into which you can add entities that implement the **IEntityKey** interface and value objects.