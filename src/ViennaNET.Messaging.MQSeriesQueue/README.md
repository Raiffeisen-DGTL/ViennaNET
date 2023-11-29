# The assembly providing work with the IBM MQ Series queue
___
### Classes
* **MqSeriesQueueConfiguration** - Advanced configuration for the IBM MQSeries adapter.
* **MqSeriesQueueMessageAdapter** - Provides direct interaction with IBM MQSeries queues.
* **MqSeriesQueueMessageAdapterConstructor** - Creates instances _MqSeriesQueueMessageAdapter_.

 Section in the configuration file:

```JavaScript
        "mqseries": { 
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
              "queueManager": "<queue broker name>",
              "channel": "<connection channel name>",
              "queueString": "<queue connection string>",
              "transactionEnabled": "<transaction mode use flag true | false>",
              "useQueueString": "<use flag to connect the queueString field true | false>",
              "replyQueue": "<name of the queue for receiving messages>",
              "lifetime": "<lifetime of messages in TimeSpan format>",
              "timeout": "<timeout of operations in TimeSpan format>",
              "customHeaders": {"values": []} // additional headers for working with queues
            } 
          ] 
        }
```
_________________
#### Message header features
* Headings must not contain a character as a separator "-". 
https://stackoverflow.com/questions/50608415/cwsia0112e-the-property-name-keep-alive-is-not-a-valid-java-identifier https://www.ibm.com/support/knowledgecenter/SSFKSJ_7.5.0/com.ibm.mq.dev.doc/q022940_.htm

#### Listening mode (processingType)
* Transactions are only supported in ThreadStrategy mode.
* If transactions are disabled (transactionEnabled == false), then it can use both ThreadStrategy and the subscription mode (Subscribe).

#### How to make encrypted connection
Just set TlsEnabled to true. Client will try to connect to broker using TLS 1.2 protocol. Port should be usually the same as for unencrypted traffic. Revocation check is always enabled for 
server certificates.

If you wish to validate server certificate, set TlsServerCertSubject to subject (DN) of server certificate.

To specific client certificate to connect to server (in case broker validates it) you need to act depending on host OS:
* On Windows specify TlsClientCertStore (defaults to User) and TlsClientCertLabel which is Friendly Name of a certificate that should be set manually in certmgr.
* On Linux you just have to put cert into `~/.dotnet/corefx/cryptography/x509stores/`