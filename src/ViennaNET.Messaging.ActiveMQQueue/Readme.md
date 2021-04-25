# The assembly providing work with the ActiveMQ queue
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
              "server": "<queue server>",
              "port": "<queue connection port>",
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
              "Selector": "<selector for reading filtered messages from the queue>"
            } 
          ] 
        }
```
_________________

#### Listening mode (processingType)
* Transactions are only supported in ThreadStrategy mode.
* If transactions are disabled (transactionEnabled == false), then it can use both ThreadStrategy and the subscription mode (Subscribe/SubscribeAndReply).
