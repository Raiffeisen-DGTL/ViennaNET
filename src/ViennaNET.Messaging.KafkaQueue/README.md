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
            "queuename": "<queue name>", // name of the queue/topic
            "transactionEnabled": <flag enables transaction true | false>,
            "replyQueue": "<name of the queue for receiving messages>",
            "customHeaders": { "values": [] }, // additional headers for working with queues
                                               // the parameters below apply only to the Kafka queue
            "serviceHealthDependent": <flag of the dependence of the subscription to the queue on the state of the service 
                                       obtained as a result of the diagnostic call true | false>,
            "consumerConfig": { },
            "producerConfig": { }
        }
    ]
}
```

> Features of configuring Kafka
> * processingtype - can only take the value of ThreadStrategy to create a subscriber that reads messages at certain intervals (Polling)
> * consumerConfig - consumer configuration properties description can be found in [Confluent.Kafka consumer documentation](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ConsumerConfig.html)
> * producerConfig - producer configuration properties description can be found in [Confluent.Kafka producer documentation](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerConfig.html)
> * consumerConfig or producerConfig must be set, but not both
> * transactionEnabled - the table below describes automatically calculated properties 
>
>   | **transactionEnabled** | consumerConfig.EnableAutoCommit | consumerConfig.IsolationLevel | producerConfig.TransactionalId |
>   |------------------------|---------------------------------|-------------------------------|--------------------------------|
>   | **true**               | false                           | ReadCommitted                 | id (queue identifier)          |
>   | **false**              | true                            | ReadUncommitted               | null                           |


Adapter recognizes `kafka_correlationId` header. If it exists in incoming message it will be put into MessageId and CorrelationId fields.
For outgoig message content of CorrelationId (if not empty) or MessageId fields will be pu into that header.