# Assembly providing work with the RabbitMQ queue
___
### Classes
* **RabbitMqQueueConfiguration** - Advanced configuration for the RabbitMQ adapter.
* **RabbitMqQueueMessageAdapter** - Provides direct interaction with RabbitMQ queues.
* **RabbitMqQueueMessageAdapterConstructor** - Creates instances of _RabbitMqQueueMessageAdapter_.

 Section in the configuration file:

```javascript
        "rabbitmq": {
          "queues": [
            {
              "id": "<queue identifier>", // as a rule, the name of the queue is indicated
              "processingtype": "<processing type to listen on the queue>", // one of the values of MessageProcessingType
              "server": "<queue server>",
              "port": "<queue connection port>",
              "queuename": "<queue name>",
              "user": "<login to connect to the queue>",
              "password": "password to connect to the queue",
              "replyQueue": "<name of the queue for receiving messages>",
              "lifetime": "<lifetime of messages in TimeSpan format>",
              "customHeaders": {"values": []} // additional headers for working with queues
              "serviceHealthDependent": <flag of the dependence of the subscription to the queue on the state of the service obtained as a result of the diagnostic call true | false>
              // the parameters below apply only to the RabbitMQ queue
              "exchangeType": "<type of exchange point>",
              "exchangeName": "<exchange point name>",
              "replyTimeout": "<timeout of operations in TimeSpan format>"
            }
          ]
        }
```

> RabbitMQ configuration features
> * exchangeType - can take one of the values ​​direct | topic | fanout | headers, if not specified, then by default fanout
> * exchangeName - if not specified, then the exchange point is not automatically created
> * queuename - if not specified, then the queue is not automatically created
> * if both queuename and exchangeName are specified, then the queue is bound to the exchange point
