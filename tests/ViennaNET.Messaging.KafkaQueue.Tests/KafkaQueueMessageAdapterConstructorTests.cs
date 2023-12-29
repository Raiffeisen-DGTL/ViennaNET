using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.KafkaQueue.Tests.DSL;

namespace ViennaNET.Messaging.KafkaQueue.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(KafkaQueueMessageAdapterConstructor))]
    public class KafkaQueueMessageAdapterConstructorTests
    {
        [OneTimeSetUp]
        public void KafkaQueueMessageAdapterConstructorSetup()
        {
            var failedConf = new List<KeyValuePair<string, string?>>()
            {
                new("messaging:ApplicationName", "NameValueApplication"),
                new("kafka:queues:0:id", "noQueue"),
                new("kafka:queues:0:processingtype", "ThreadStrategy"),
                new("kafka:queues:0:intervalPollingQueue", "30000"),
                new("kafka:queues:0:consumerConfig:BootstrapServers", "some servers"),
                new("kafka:queues:1:id", "bothProducerAndConsumerConfig"),
                new("kafka:queues:1:processingtype", "ThreadStrategy"),
                new("kafka:queues:1:queuename", "ReQueue"),
                new("kafka:queues:1:intervalPollingQueue", "30000"),
                new("kafka:queues:1:producerConfig:BootstrapServers", "some servers"),
                new("kafka:queues:1:consumerConfig:BootstrapServers", "some servers"),
                new("kafka:queues:2:id", "noProducerAndConsumerConfig"),
                new("kafka:queues:2:processingtype", "ThreadStrategy"),
                new("kafka:queues:2:queuename", "ReQueue"),
                new("kafka:queues:2:intervalPollingQueue", "30000"),
            };

            var normalConf = new List<KeyValuePair<string, string?>>()
            {
                new("messaging:ApplicationName", "NameValueApplication"),
                new("kafka:queues:0:id", "testKafkaQueue"),
                new("kafka:queues:0:processingtype", "ThreadStrategy"),
                new("kafka:queues:0:queuename", "ReQueue"),
                new("kafka:queues:0:intervalPollingQueue", "30000"),
                new("kafka:queues:0:isHealthCheck", "true"),
                new("kafka:queues:0:serviceHealthDependent", "true"),
                new("kafka:queues:0:transactionEnabled", "false"),
                new("kafka:queues:0:producerConfig:BootstrapServers", "some servers"),
                new("kafka:queues:1:id", "testKafkaQueueWithTransactions"),
                new("kafka:queues:1:processingtype", "ThreadStrategy"),
                new("kafka:queues:1:queuename", "ReQueue"),
                new("kafka:queues:1:intervalPollingQueue", "30000"),
                new("kafka:queues:1:isHealthCheck", "true"),
                new("kafka:queues:1:serviceHealthDependent", "true"),
                new("kafka:queues:1:transactionEnabled", "true"),
                new("kafka:queues:1:producerConfig:BootstrapServers", "some servers")
            };

            _failedConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(failedConf)
                .Build();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(normalConf)
                .Build();

            _constructor = new KafkaQueueMessageAdapterConstructor(configuration, Given.FakeLoggerFactory);
        }
        
        private const string QueueId = "testKafkaQueue";
        private const string QueueWithTransactionsId = "testKafkaQueueWithTransactions";

        private IConfiguration _failedConfig = null!;
        private KafkaQueueMessageAdapterConstructor _constructor = null!;

        [Test]
        public void Create_HasNoQueueInConfig_Exception()
        {
            Assert.Throws<MessagingConfigurationException>(() => _constructor.Create(string.Empty));
        }

        [Test]
        public void Create_HasQueueInConfig_ReturnsAdapter()
        {
            var adapter = _constructor.Create(QueueId);

            Assert.Multiple(() =>
            {
                Assert.That(adapter, Is.InstanceOf<IMessageAdapter>());
                Assert.That(adapter.Configuration, Is.Not.Null);
                Assert.That(adapter.Configuration.Id, Is.EqualTo(QueueId));
            });
        }

        [Test]
        public void Create_HasQueueWithTransactionsInConfig_ReturnsTransactedAdapter()
        {
            var adapter = _constructor.Create(QueueWithTransactionsId);

            Assert.Multiple(() =>
            {
                Assert.That(adapter, Is.InstanceOf<IMessageAdapterWithTransactions>());
                Assert.That(adapter.Configuration, Is.Not.Null);
                Assert.That(adapter.Configuration.Id, Is.EqualTo(QueueWithTransactionsId));
            });
        }

        [Test]
        public void CreateAll_HasConfig_ReturnAdapters()
        {
            Assert.That(_constructor.CreateAll(), Is.Not.Empty);
        }

        [Test]
        public void Create_HasQueueInConfigWithoutParameter_Exception()
        {
            Assert.That(() => new KafkaQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory),
                Throws.InstanceOf<MessagingException>().And.Message.Contains("QueueName"));
        }

        [Test]
        public void Create_NoProducerAndConsumerConfigs_Exception()
        {
            Assert.That(() => new KafkaQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory),
                Throws.InstanceOf<MessagingException>().And.Message.Contains("ProducerConfig")
                    .And.Message.Contains("ConsumerConfig"));
        }

        [Test]
        public void Create_BothProducerAndConsumerConfigs_Exception()
        {
            Assert.That(() => new KafkaQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory),
                Throws.InstanceOf<MessagingException>().And.Message.Contains("ProducerConfig")
                    .And.Message.Contains("ConsumerConfig"));
        }

        [Test]
        public void HasQueue_QueueExistsInConfig_True()
        {
            Assert.That(_constructor.HasQueue(QueueId), Is.True);
        }

        [Test]
        public void HasQueue_QueueNotExistsInConfig_False()
        {
            Assert.That(_constructor.HasQueue(""), Is.False);
        }
    }
}