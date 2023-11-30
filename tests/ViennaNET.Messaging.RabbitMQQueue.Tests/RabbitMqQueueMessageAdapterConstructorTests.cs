using System.Reflection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.RabbitMQQueue.Tests.DSL;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(RabbitMqQueueMessageAdapterConstructor))]
  public class RabbitMqQueueMessageAdapterConstructorTests
  {
    [OneTimeSetUp]
    public void RabbitMqQueueMessageAdapterConstructorSetup()
    {
      using (var configStream = Assembly.GetExecutingAssembly()
               .GetManifestResourceStream("ViennaNET.Messaging.RabbitMQQueue.Tests.appsettingsFailed.json"))
      {
        _failedConfig = new ConfigurationBuilder()
          .AddJsonStream(configStream)
          .Build();
      }

      using (var configStream = Assembly.GetExecutingAssembly()
               .GetManifestResourceStream("ViennaNET.Messaging.RabbitMQQueue.Tests.appsettings.json"))
      {
        var configuration = new ConfigurationBuilder()
          .AddJsonStream(configStream)
          .Build();

        _constructor = new RabbitMqQueueMessageAdapterConstructor(
          configuration,
          Given.FakeLoggerFactory);
      }
    }

    private const string QueueId = "Rabbit";
    private const string QueueWithoutServerId = "NextRabbit";

    private IConfiguration _failedConfig;
    private RabbitMqQueueMessageAdapterConstructor _constructor;

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
        Assert.IsNotNull(adapter);
        Assert.IsNotNull(adapter.Configuration);
        Assert.AreEqual(QueueId, adapter.Configuration.Id);
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
        new RabbitMqQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory));
      Assert.Multiple(() =>
      {
        StringAssert.Contains("AutoAck", exception.Message);
        StringAssert.Contains("Requeue", exception.Message);
      });
    }

    [Test]
    public void HasQueue_QueueExistsInConfig_True()
    {
      var hasQueue = _constructor.HasQueue(QueueId);

      Assert.That(hasQueue);
    }

    [Test]
    public void HasQueue_QueueExistsInConfigButBroken_True()
    {
      var hasQueue = _constructor.HasQueue(QueueWithoutServerId);

      Assert.That(hasQueue);
    }

    [Test]
    public void HasQueue_QueueNotExistsInConfig_False()
    {
      var hasQueue = _constructor.HasQueue("");

      Assert.That(!hasQueue);
    }
  }
}