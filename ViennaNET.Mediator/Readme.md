# An assembly containing an intermediary implementation for performing service operations

### Key Entities

The main class is **Mediator**. It is an intermediary that allows you to bind different types of messages and their handlers.

**EventCollector** allows for delayed sending through an intermediary.

#### Instructions for use for processing requests and commands:

1. Add dependency on **IMediator** to the class.
2. We create or register in the DI class **Mediator**, parameterizing it with collections of synchronous and asynchronous message handlers. The list of handlers should correspond to the expected message types in the service.
3. In the service methods, call the methods of the SendMessage or SendMessageAsync interface. The broker will automatically forward the message to the appropriate handler.

### Usage example

```csharp
  public IHttpActionResult Get ()
  {
    IEnumerable<ProductModel>products = _mediator.SendMessage<GetProductsRequest, IEnumerable <ProductModel>>(new GetProductsRequest());

    if (products is null ||! products.Any())
    {
      return NotFound();
    }
    else
    {
      return Ok(products);
    }
  }
```

#### Application instruction for processing domain events:

1. Implement in essence the interface **IEntityEventPublisher**. The standard implementation stores an instance of **IEventCollector** in an internal variable.

```csharp
  public class Payroll: IEntityKey<int>, IEntityEventPublisher
  {
    private IEventCollector _eventCollector;

    public virtual void SetCollector(IEventCollector eventCollector);
    {
      _eventCollector = eventCollector;
    }
  }
```

2. If necessary, publish the event using **IEventCollector**.

```csharp
  _eventCollector.Enqueue (new PayrollReachesFinalStatusEvent (this));
```

3. In the application service managing domain entities, implement a unit of work for sending domain events.

```csharp
   using (var collector = _eventCollectorFactory.Create())
   {
     ...
     collector.Publish();
   }
```

For correct event handling, Handlers must be registered in the **Mediator** class.
