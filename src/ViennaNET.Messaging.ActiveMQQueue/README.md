# The assembly providing work with the ActiveMQ (Artemis) queue
___
### Classes
* **ActiveMqQueueConfiguration** - Advanced configuration for the ActiveMQ adapter.
* **ActiveMqQueueMessageAdapter** - Provides direct interaction with ActiveMQ queues.
* **ActiveMqQueueMessageAdapterConstructor** - Creates instances _ActiveMqQueueMessageAdapter_.

 Section in the configuration file:

```JavaScript
        "activemq": { 
          "queues": [ 
            {
              "id": "<queue identifier>", // usually the queue name is specified
              "processingtype": "<processing type to listen on the queue>", // one of the values ​​of MessageProcessingType
              "server": "<broker server>",
              "port": "<broker connection port>",
              "connectionString": "<broker connection string>",
              "queuename": "<queue name>",
              "user": "<login to connect to the queue>",
              "password": "password to connect to the queue",
              "clientId": "<client id to queue>",
              "queue": "<queue name in the queue broker>",
              "transactionEnabled": "<transaction mode use flag true | false>",
              "useQueueString": "<use flag to connect the queueString field true | false>",
              "queueString": "<queue connection string>",
              "replyQueue": "<name of the queue for send reply messages>",
              "lifetime": "<lifetime of messages in TimeSpan format>",
              "timeout": "<timeout of operations in TimeSpan format>",
              "Selector": "<selector for reading filtered messages from the queue>",
              "intervalPollingQueue": 
            } 
          ] 
        }
```
_________________

#### Listening mode (processingType)
* Transactions are only supported in ThreadStrategy mode.
* If transactions are disabled (transactionEnabled == false), then it can use both ThreadStrategy and the subscription mode (Subscribe/SubscribeAndReply).
* connectionString if specified has higher priority than server and port. Example: "connectionString": "activemq:failover:(tcp://remotehost1:61616,tcp://remotehost2:61616,...,tcp://remotehostN:61616)?initialReconnectDelay=100&maxReconnectAttempts=-1"
* AMQP protocol can be enabled with corresponding connection string examples:
    * amqp://remotehost1:61616
    * failover:(amqp://remotehost1:61616,amqp://remotehost2:61616)

```JavaScript
        "activemq": { 
          "queues": [ 
            {
              "id": "id",
              "processingtype": "SubscribeAndReply",
              "server": "localhost",
              "port": "61616",
              "queueName": "name",
              "replyQueue": "replyName",
              "lifetime": "00:01:00",
              "user": "admin",
              "password": "admin",
              "isHealthCheck": "true",
              "serviceHealthDependent": true
            },
            {
              "id": "id",
              "processingtype": "Subscribe",
              "connectionString": "activemq:failover:(tcp://remotehost1:61616,tcp://remotehost2:61616,...,tcp://remotehostN:61616)?initialReconnectDelay=100&maxReconnectAttempts=-1"
              "port": "61616",
              "queueName": "name",
              "intervalPollingQueue": "10000",
              "user": "admin",
              "password": "admin",
              "isHealthCheck": "true",
              "serviceHealthDependent": true
            }
          ] 
        }
```
