# Assembly providing work with the Kafka queue
___
### Classes
* **KafkaQueueConfiguration** - Advanced configuration for the Apache Kafka adapter.
* **KafkaQueueMessageAdapter** -Provides direct interaction with Apache Kafka queues.
* **KafkaQueueMessageAdapterConstructor** - Creates instances of _KafkaQueueMessageAdapter_.
* **AvroMessageSerializer** - Serializer messages in the Apache Avro format.

 Section in the configuration file:

```javascript
       "kafka": {
         "queues": [
           {
             "id": "<queue identifier>", // as a rule, the name of the queue is indicated
             "processingtype": "<processing type to listen on the queue>", // one of the values ​​of MessageProcessingType
             "server": "<queue server>",
             "queuename": "<queue name>",
             "user": "<login to connect to the queue>",
             "password": "password to connect to the queue",
             "replyQueue": "<name of the queue for receiving messages>",
             "lifetime": "<lifetime of messages in TimeSpan format>",
             "customHeaders": {"values": []} // additional headers for working with queues
             // the parameters below apply only to the Kafka queue
             "exchangeType": "<type of exchange point>",
             "exchangeName": "<exchange point name>",
             "serviceHealthDependent": "<flag of the dependence of the subscription to the queue on the state of the service obtained as a result of the diagnostic call true | false>",
             "isConsumer": "<flag that determines whether the adapter accepts or sends true | false>",
             "serviceName": "<service name>",
             "keyTab": "<path to the *.keytab file, required for SASL authentication on unix-based servers>",
             "securityProtocol": "<authentication protocol>",
             "saslMechanism": "<encryption mechanism for authentication protocol>",
             "groupId": "<group name of message recipients>",
             "debug": "<line with setting the logging mode>",
             "autoOffsetReset": "<action to be taken if there is no offset in the store or it is out of range>"
           }
         ]
       }
```       
