# Build with classes and interfaces providing a common functionality for working with queues
___
### The principle of operation and use of the assembly
The assembly contains basic interfaces and implementations for working with abstract queues. Expands by adapters to secret queues in the form of separate assemblies. The assembly does not contain information about the specific queues used.
___
#### Sending and receiving messages
> To use the library in your assembly, implementations of the _IMessageProcessor_ interface must be defined in case of constant listening to the queue, as well as the implementation of the message classes necessary for your assemblies to work.
To send messages and receive them in a targeted manner, you must implement the IMessagingComponentFactory dependency.
This dependency is a component factory and returns objects that implement the IDisposable interface.

> Example of receiving a message:

```csharp
    using (var receiver = _messagingComponentFactory.CreateMessageReceiver<TestMessage>("testQueue"))
    {
        var message = receiver.Receive();
    }
```

> Example of sending a message:

```csharp
    using (var sender = _messagingComponentFactory.CreateMessageSender<TestMessage>("testQueue"))
    {
        sender.SendMessage(new TestMessage {Value = message});
    }
```

> To implement listening on the queue, you must implement the IQueueReactorFactory dependency. In a class with an embedded dependency, you need to create an IQueueReactor instance using this factory and start listening, after registering IMessageProcessor message handlers in it:

```csharp
    private readonly IQueueReactor _queueReactor;
    public TestClass (IQueueReactorFactory queueReactorFactory)
    {
      _queueReactorFactory.Register<TestMessageProcessor>("testQueue");
      _queueReactor = _queueReactorFactory.CreateQueueReactor("testQueue");
      _queueReactor.StartProcessing();
    }
```

> Example of a message handler class and message class:

```csharp
        public class TestMessageProcessor: IMessageProcessor
        {
            public bool Process(BaseMessage message)
            {
                var deserializer = new XmlMessageSerializer<TestMessage>();
                var msg = deserializer.Deserialize(message);
                Logger.LogDebug(msg.Value);
                return true;
            }
        }
        [Serializable]
        public class TestMessage
        {
            public string Value {get; set; }
        }
```

> In your assembly that uses libraries for working with queues, you need to register the XmlMessageSerializer<T> implementation of the IMessageSerializer and IMessageDeserializer interfaces for messages used in your business logic and implemented in your assemblies in the installer for the IoC container.
> Registration example:

```csharp
public void RegisterServices(Container container)
{
  container.Collection.Append<IMessageSerializer, XmlMessageSerializer<TestMessage>>(Lifestyle.Singleton);
  container.Collection.Append<IMessageDeserializer, XmlMessageSerializer<TestMessage>>(Lifestyle.Singleton);
}
```
___
___
___
___
### Assembly composition
___
#### Configuration
___
##### Factory
* Interfaces:
    * **IMessagingConfigurationFactory** - Creates a configuration for working with queues.
* Classes:
    * **MessagingConfigurationFactory** - Creates a configuration for working with queues.
___
##### Configuration sections
* **MessagingConfiguration** - Configuration section. It contains the necessary information for the library.
Section in the configuration file:

```javascript
"messaging": { "ApplicationName": <name of the application performing work with queues> }
```

* **QueueConfigurationBase** - The base class of the configuration section. It contains the necessary information to work with all types of queues.

* **CustomHeader** - Configuration section. Contains information on additional headers for working with queues in key-value format.
 Section in the configuration file:

```javascript
        "<queue type name>": [{
        ...
            "customHeaders": {
                "values": [
                    {"key": "<title name>", "value": "<title value>"}]
            }
        }
```

_________________
##### Enumerations Used in the Configuration
* **MessageProcessingType** - type of processing for listening to the queue. Values:
    * _ThreadStrategy_ - based on work in an infinite loop in one thread.
    * _Subscribe_ - based on a subscription to queue events.
    * _SubscribeAndReply_ - based on subscribing to queue events and the ability to respond.
_________________
##### Messages
* **BaseMessage** - The main class of messages to send to the queue. Any messages containing data must be serialized in the Body field and deserialized from it.
 Properties:
    * _MessageId_ - message identifier.
    * _CorrelationId_ - correlation identifier.
    * _ApplicationTitle_ - the name of the application that sent the message.
    * _SendDate_ - date and time of sending the message.
    * _ReceiveDate_ - date and time of receipt of the message.
    * _LifeTime_ - life time
