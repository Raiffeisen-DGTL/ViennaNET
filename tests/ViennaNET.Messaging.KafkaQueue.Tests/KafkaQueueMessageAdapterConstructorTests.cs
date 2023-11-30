using System.Reflection;
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
      using (var configStream = Assembly
               .GetExecutingAssembly()
               .GetManifestResourceStream("ViennaNET.Messaging.KafkaQueue.Tests.appsettingsFailed.json"))
      {
        _failedConfig = new ConfigurationBuilder()
          .AddJsonStream(configStream)
          .Build();
      }

      using (var configStream = Assembly
               .GetExecutingAssembly()
               .GetManifestResourceStream("ViennaNET.Messaging.KafkaQueue.Tests.appsettings.json"))
      {
        var configuration = new ConfigurationBuilder()
          .AddJsonStream(configStream)
          .Build();

        _constructor = new KafkaQueueMessageAdapterConstructor(configuration, Given.FakeLoggerFactory);
      }
    }

    private const string QueueId = "testKafkaQueue";
    private const string QueueWithTransactionsId = "testKafkaQueueWithTransactions";

    private IConfiguration _failedConfig;
    private KafkaQueueMessageAdapterConstructor _constructor;

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
        Assert.IsInstanceOf<IMessageAdapter>(adapter);
        Assert.IsNotNull(adapter.Configuration);
        Assert.AreEqual(QueueId, adapter.Configuration.Id);
      });
    }

    [Test]
    public void Create_HasQueueWithTransactionsInConfig_ReturnsTransactedAdapter()
    {
      var adapter = _constructor.Create(QueueWithTransactionsId);

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<IMessageAdapterWithTransactions>(adapter);
        Assert.IsNotNull(adapter.Configuration);
        Assert.AreEqual(QueueWithTransactionsId, adapter.Configuration.Id);
      });
    }

    [Test]
    public void CreateAll_HasConfig_ReturnAdapters()
    {
      var adapters = _constructor.CreateAll();

      CollectionAssert.IsNotEmpty(adapters);
    }

    [Test]
    public void Create_HasQueueInConfigWithoutParameter_Exception()
    {
      var exception = Assert.Throws<MessagingException>(() =>
        new KafkaQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory));
      Assert.Multiple(() =>
      {
        StringAssert.Contains("QueueName", exception.Message);
      });
    }
    
    [Test]
    public void Create_NoProducerAndConsumerConfigs_Exception()
    {
      var exception = Assert.Throws<MessagingException>(() =>
        new KafkaQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory));
      Assert.Multiple(() =>
      {
        StringAssert.Contains("ProducerConfig", exception.Message);
        StringAssert.Contains("ConsumerConfig", exception.Message);
      });
    }
    
    [Test]
    public void Create_BothProducerAndConsumerConfigs_Exception()
    {
      var exception = Assert.Throws<MessagingException>(() =>
        new KafkaQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory));
      Assert.Multiple(() =>
      {
        StringAssert.Contains("ProducerConfig", exception.Message);
        StringAssert.Contains("ConsumerConfig", exception.Message);
      });
    }

    [Test]
    public void HasQueue_QueueExistsInConfig_True()
    {
      var hasQueue = _constructor.HasQueue(QueueId);

      Assert.IsTrue(hasQueue);
    }

    [Test]
    public void HasQueue_QueueNotExistsInConfig_False()
    {
      var hasQueue = _constructor.HasQueue("");

      Assert.IsFalse(hasQueue);
    }
  }
}