using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.KafkaQueue
{
    internal class KafkaQueueTransactedMessageAdapter : KafkaQueueMessageAdapter, IMessageAdapterWithTransactions
    {
        private readonly ILogger<KafkaQueueTransactedMessageAdapter> _logger;
        private readonly KafkaQueueConfiguration _queueConfiguration;

        public KafkaQueueTransactedMessageAdapter(
          KafkaQueueConfiguration queueConfiguration,
          IKafkaConnectionFactory connectionFactory,
          ILogger<KafkaQueueTransactedMessageAdapter> logger) : base(queueConfiguration, connectionFactory, logger)
        {
            _queueConfiguration = queueConfiguration;
            _logger = logger;
        }

        public override void Connect()
        {
            base.Connect();

            if (_producer != null)
            {
                _producer.InitTransactions(
                  TimeSpan.FromMilliseconds(_queueConfiguration.InitTransactionsTimeout));
                _producer.BeginTransaction();

                _logger.LogDebug("Transaction for queue {queueId} started", _queueConfiguration.Id);
            }
        }

        public override void Disconnect()
        {
            if (_producer != null)
            {
                _producer.CommitTransaction();

                _logger.LogDebug("Transaction for queue {queueId} committed", _queueConfiguration.Id);
            }

            base.Disconnect();
        }

        public void CommitIfTransacted(BaseMessage message)
        {
            _consumer.Commit();
        }

        public void RollbackIfTransacted()
        {
            // Just don't commit
        }
    }
}