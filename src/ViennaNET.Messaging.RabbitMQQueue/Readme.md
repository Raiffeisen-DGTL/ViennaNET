# Assembly providing work with the RabbitMQ queue
___
### Classes
* **RabbitMqQueueConfiguration** - Advanced configuration for the RabbitMQ adapter.
* **RabbitMqQueueMessageAdapter** - Provides direct interaction with RabbitMQ queues.
* **RabbitMqQueueMessageAdapterConstructor** - Creates instances of _RabbitMqQueueMessageAdapter_.

### Configuration
 Section in the configuration file:

```javascript
        "rabbitmq": {
          "queues": [
            {
              "id": "<queue identifier>", // as a rule, the name of the queue is indicate
              "addresses": "<list hostname or ip. example: 127.0.0.1:5672,127.0.0.1:5672>", // when filling in this field, the priority of this field will be higher than that of server and port,                
              "server": "<queue server host>",
              "port": "<queue server port>",
              "intervalPollingQueue": 
              "processingtype": "<processing type to listen on the queue>", // one of the values of MessageProcessingType
              "queuename": "<queue name>",
              "user": "<login to connect to the queue>",
              "password": "password to connect to the queue",
              "replyQueue": "<name of the queue for receiving messages>",
              "lifetime": "<lifetime of messages in TimeSpan format>",
              "autoAck": "<automatically ack received messages - true | false>",
              "requeue": "<requeue messages that was handled with error - true | false>"
              "customHeaders": {"values": []} // additional headers for working with queues
              "serviceHealthDependent": <flag of the dependence of the subscription to the queue on the state of the service 
                                         obtained as a result of the diagnostic call true | false>
              // the parameters below apply only to the RabbitMQ queue
              "exchangeType": "<type of exchange point>",
              "exchangeName": "<exchange point name>",
              "replyTimeout": "<timeout of operations in TimeSpan format>",
              "connectionTimeout": "<timeout of connection to Rabbit in TimeSpan. Default value: 30 seconds. Example: \"0.00:00:30\">",
              "virtualHost": "<RabbitMQ virtual host>",
              "routings": [] // RabbitMQ routing keys
            }
          ]
        }
```

> Required fields
> * id
> * server
> * exchangeName
> * intervalPollingQueue
> * user
> * password
>
> RabbitMQ configuration features
> * processingtype - can take the value Subscribe or SubscribeAndReply to create a subscriber that reads messages as they arrive (by event), or for the same subscriber, but with the ability to send a response message, respectively
> * exchangeType - can take one of the values ​​direct | topic | fanout | headers, if not specified, then by default fanout
> * exchangeName - if not specified, then the exchange point is not automatically created
> * queuename - if not specified, then the queue is not automatically created
> * if both queuename and exchangeName are specified, then the queue is bound to the exchange point
> * autoAck - automatically ack messages received with Subscribe. Incompatible with requeue.
> * requeue - requeue message that was handled with error in Subscribe. Incompatible with autoAck.