using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.RabbitMQQueue;
using ViennaNET.Messaging.Tests.Unit.DSL;

namespace ViennaNET.Messaging.Tests.Unit.RabbitMq
{
  [TestFixture(Category = "Unit", TestOf = typeof(RabbitMqQueueMessageAdapterConstructorTests))]
  public class RabbitMqQueueMessageAdapterConstructorTests
  {
    private const string QueueId = "Rabbit";
    private const string QueueWithoutServerId = "NextRabbit";

    private RabbitMqQueueMessageAdapterConstructor _constructor;

    [OneTimeSetUp]
    public void RabbitMqQueueMessageAdapterConstructorSetup()
    {
      var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .Build();

      _constructor = new RabbitMqQueueMessageAdapterConstructor(
        Mock.Of<IAdvancedBusFactory>(),
        configuration, 
        Given.FakeLoggerFactory);
    }

    [Test]
    public void Create_HasQueueInConfig_ReturnsAdapter()
    {
      var adapter = _constructor.Create(QueueId);
      
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
      Assert.Throws<MessagingConfigurationException>(() => _constructor.Create(string.Empty), "There are no configuration with id '' in configuration file");
    }

    [Test]
    public void Create_HasQueueInConfigWithoutServer_Exception()
    {
      Assert.Throws<ArgumentNullException>(() => _constructor.Create(QueueWithoutServerId), "Value cannot be null. (Parameter 'Server')");
    }

    [Test]
    public void CreateAll_HasConfig_ReturnAdapters()
    {
      var fakeAdvancedBusFactory = new Mock<IAdvancedBusFactory>();
      var configuration = new ConfigurationBuilder().AddJsonFile("appsettingsAll.json", true)
                                                    .Build();

      var constructor = new RabbitMqQueueMessageAdapterConstructor(fakeAdvancedBusFactory.Object, configuration, Given.FakeLoggerFactory);

      var adapters = constructor.CreateAll();

      Assert.Multiple(() =>
      {
        Assert.That(adapters, Is.Not.Null);
        Assert.That(adapters.Count, Is.EqualTo(2));
        Assert.That(adapters.First().Configuration, Is.Not.Null);
        Assert.That(adapters.First().Configuration.Id, Is.EqualTo(QueueId));
        Assert.That(adapters.Last().Configuration, Is.Not.Null);
        Assert.That(adapters.Last().Configuration.Id, Is.EqualTo("NextRabbit"));
      });
    }

    [Test]
    public void CreateAll_HasNoServerQueueInConfig_Exception()
    {
      Assert.Throws<ArgumentNullException>(() => _constructor.CreateAll(), "Value cannot be null. (Parameter 'Server')");
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