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
- Interfaces:
	- **IMessagingConfigurationFactory** - Creates a configuration for working with queues.
- Classes:
	- **MessagingConfigurationFactory** - Creates a configuration for working with queues.
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
- **MessageProcessingType** - type of processing for listening to the queue. Values:
	- _ThreadStrategy_ - based on work in an infinite loop in one thread.
	- _Subscribe_ - based on a subscription to queue events.
	- _SubscribeAndReply_ - based on subscribing to queue events and the ability to respond.
_________________
##### Messages
* **BaseMessage** - The main class of messages to send to the queue. Any messages containing data must be serialized in the Body field and deserialized from it.
 Properties:
 
	* _MessageId_ - message identifier.
	* _CorrelationId_ - correlation identifier.
	* _ApplicationTitle_ - the name of the application that sent the message.
	* _SendDate_ - date and time of sending the message.
	* _ReceiveDate_ - date and time of receipt of the message.
	* _LifeTime_ - message lifetime, by default - indefinitely (TimeSpan.Zero).
	* _Body_ - message body.
	* _Properties_ - dictionary of additional message properties, by default - an empty dictionary.
_________________
_________________
#### Message serialization / deserialization
* Interfaces:
	* **IMessageSerializer / IMessageSerializer<in TMessage>** - Serializes the original message in the Body field of the _BaseMessage_ instance and returns the _BaseMessage_ instance.
	* **IMessageDeserializer / IMessageDeserializer<out TMessage>** - Deserializes from the Body field of the _BaseMessage_ instance to the message type intended for further use.
* Classes:
	* **PlainTextSerializer** - serializer/deserializer for plain text
	* **XmlMessageSerializer<T>** - xml-serializer/deserializer. It supports validation based on a data scheme - for this you need to set the value of the xsd field using the _SetStream (Stream xsd)_ or _AddStream (Stream xsd)_ methods.
	* **XmlResourcesMessageSerializer<T>** - xml-serializer/deserealizer. It supports validation based on a data scheme - for this, you need to set the value of the xsd field using an implementation of a descendant of the ResourceStorage class.
_________________   
_________________
#### Queue connection classes their classes generating them
* Interfaces:
	* **IMessageAdapter** - Proxy to the queue.
		> **IMessageAdapter** is implemented in separate assemblies for each individual queue type, since its specific implementations depend on libraries that provide queue APIs.

		* **IMessageAdapterWithSubscribing** - Proxy to the queue, in addition to the _IMessageAdapter_ functionality, which provides the ability to subscribe client classes to queue events.
			> **IMessageAdapterWithSubscribing** is implemented in separate assemblies for each individual queue type, since its specific implementations depend on the libraries that provide the queue API.
		* **IMessageAdapterWithTransactions** - Proxy to the queue, in addition to the _IMessageAdapter_ functionality, which provides the possibility of transactional work with the queue.
			> **IMessageAdapterWithTransactions** is implemented in separate assemblies for each individual queue type, since its specific implementations depend on the libraries that provide the queue API.

		* **IMessageAdapterConstructor** - Creates _IMessageAdapter_ instances for this queue type.
			> **IMessageAdapterConstructor** is implemented in separate assemblies for each individual queue type, since its specific implementations depend on the libraries that provide the queue API.

		* **IMessageAdapterFactory** - Creates instances **IMessageAdapter** by the queue name defined in the configuration file.
	* **IMessageReceiver<TMessage>** - Receive messages from the queue.
	* **IMessageSender** - Sends messages to the queue.
	* **ISerializedMessageSender<in TMessage>** - Sends messages to the queue with pre-serialization.
	* **IMessagingComponentFactory** - Creates instances of senders and recipients of messages by the queue name defined in the configuration file.

* Classes:
	* **MessageAdapterFactory** - Instance Factory _IMessageAdapter_. Creates them by the queue name defined in the configuration file. Contains a collection of _IMessageAdapterConstructor_ instances that are referenced when the adapter is instantiated.
			> For one type of queue, only one IMessageAdapterConstructor implementation can be defined. Otherwise, a CantFindAdapterConstructorException will be thrown.

	* **MessagingComponentFactory** - Instance factory _IMessageReceiver_ and _IMessageSender_. Creates them by the queue name defined in the configuration file. Contains collections of _IMessageSerializer_ and _IMessageDeserializer_ instances that are referenced when the component is instantiated.
			> Only one implementation of IMessageReceiver<TMessage> / ISerializedMessageSender<TMessage> messages can be defined for a single queue name. Otherwise, a CantFindSerializerOrDeserializerException will be thrown.

	* **MessageReceiver<TMessage>** - the class of the message recipient.
	* **MessageSender** - class of message sender.
	* **SerializedMessageSender<TMessage>** - the class of the sender of messages with serialization.
_________________
_________________
#### Listening to Queues
* Interfaces:
	* **IQueueReactor** - A process in memory listening on a queue.
	* **IQueueReactorFactory** - Creates instances of _IQueueReactor_.
	* **IPolling** - Provides _IQueueReactor_ operation due to the flow.
	* **IMessageProcessorRegister** - Registers the message handlers _IMessageProcessor_.
	* **IMessageProcessor** - Processes messages when they are received.
		> IMessageProccessor implementations are defined in the assemblies where the message is directly processed.

* Classes:
	* **QueueReactorFactory** - Creates _IQueueReactor_ instances by the name of the queue based on the collections of _IMessageConstructor_ and _IMessageProcessor_ instances, and also registers _IMessageProcessor_ handlers.
		> Queues and types implementing IMessageProcessor are registered in the class. If you try to register a processor type already registered in the factory, a MessageProcessorAlreadyRegisterException will be thrown.

* Features:
	* When the serviceHealthDependent configuration flag is set to true, the current IQueueReactor subscribes to events that are thrown by the root IHealthCheckingService diagnostic service. This allows you to quickly disconnect from the queue in case the service diagnostics fail.

_________________   
_________________
