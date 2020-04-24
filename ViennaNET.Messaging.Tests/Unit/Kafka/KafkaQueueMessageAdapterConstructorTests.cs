using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.KafkaQueue;

namespace ViennaNET.Messaging.Tests.Unit.Kafka
{
  [TestFixture(Category = "Unit", TestOf = typeof(KafkaQueueMessageAdapterConstructor))]
  public class KafkaQueueMessageAdapterConstructorTests
  {
    private const string QueueId = "testKafkaQueue";
    private const string QueueWithoutServerId = "noServer";

    private KafkaQueueMessageAdapterConstructor _constructor;

    [OneTimeSetUp]
    public void KafkaQueueMessageAdapterConstructorSetup()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true)
                                                 .Build();

      _constructor = new KafkaQueueMessageAdapterConstructor(configuration);
    }

    [Test]
    public void Create_HasQueueInConfig_ReturnsAdapter()
    {
      var adapter = _constructor.Create(QueueId, false);
      
      Assert.Multiple(() =>
      {
        Assert.That(adapter, Is.Not.Null);
        Assert.That(adapter.Configuration, Is.Not.Null);
        Assert.That(adapter.Configuration.Id, Is.EqualTo(QueueId));
      });
    }

    [Test]
    public void Create_HasNoQueueInConfig_Exception()
    {
      Assert.Throws<MessagingConfigurationException>(() => _constructor.Create("", false), "There are no configuration with id '' in configuration file");
    }

    [Test]
    public void Create_HasQueueInConfigWithoutServer_Exception()
    {
      Assert.Throws<ArgumentNullException>(() => _constructor.Create(QueueWithoutServerId, false), "Value cannot be null. (Parameter 'Server')");
    }

    [Test]
    public void CreateAll_HasConfig_ReturnAdapters()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("appsettingsAll.json", true)
                                                    .Build();

      var constructor = new KafkaQueueMessageAdapterConstructor(configuration);

      var adapters = constructor.CreateAll(false);

      Assert.Multiple(() =>
      {
        Assert.That(adapters, Is.Not.Null);
        Assert.That(adapters.Count, Is.EqualTo(2));
        Assert.That(adapters.First().Configuration, Is.Not.Null);
        Assert.That(adapters.First().Configuration.Id, Is.EqualTo(QueueId));
        Assert.That(adapters.Last().Configuration, Is.Not.Null);
        Assert.That(adapters.Last().Configuration.Id, Is.EqualTo("nextKafkaQueue"));
      });
    }

    [Test]
    public void CreateAll_HasNoServerQueueInConfig_Exception()
    {
      Assert.Throws<ArgumentNullException>(() => _constructor.CreateAll(false), "Value cannot be null. (Parameter 'Server')");
    }

    [Test]
    public void HasQueue_QueueExistsInConfig_True()
    {
      var hasQueue = _constructor.HasQueue(QueueId);

      Assert.That(hasQueue);
    }

    [Test]
    public void HasQueue_QueueNotExistsInConfig_False()
    {
      var hasQueue = _constructor.HasQueue("");

      Assert.That(!hasQueue);
    }

    [Test]
    public void HasQueue_QueueExistsInConfigButBroken_True()
    {
      var hasQueue = _constructor.HasQueue(QueueWithoutServerId);

      Assert.That(hasQueue);
    }
  }
}