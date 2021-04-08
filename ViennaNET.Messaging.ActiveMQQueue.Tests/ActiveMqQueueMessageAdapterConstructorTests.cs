using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.ActiveMQQueue.Tests.DSL;
using ViennaNET.Messaging.Exceptions;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(ActiveMqQueueMessageAdapterConstructor))]
  public class ActiveMqQueueMessageAdapterConstructorTests
  {
    private const string QueueId = "ActiveMQ";
    private const string NoQueueStringNotUsingQueueId = "NoQueueStringNotUsing";
    private const string NoServerQueueId = "NoServer";
    private const string ThreadQueueId = "ActiveMQ";
    private const string TransactedQueueId = "Transacted";
    private const string SubscribingQueueId = "Subscribing";

    private ActiveMqQueueMessageAdapterConstructor _constructor;

    [OneTimeSetUp]
    public void MqSeriesQueueMessageAdapterConstructorSetup()
    {
      var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", true)
        .Build();

      _constructor = new ActiveMqQueueMessageAdapterConstructor(configuration,
        Given.FakeLoggerFactory, Given.FakeConnectionFactory);
    }

    [Test]
    public void Create_HasNoQueueInConfig_Exception()
    {
      Assert.Throws<MessagingConfigurationException>(() => _constructor.Create(string.Empty),
        "There are no configuration with id '' in configuration file");
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
        Assert.That(adapter.Configuration.Lifetime, Is.EqualTo(new TimeSpan(0, 0, 0, 0, 1)));
      });
    }


    [Test]
    [TestCase(NoServerQueueId, "Server")]
    [TestCase("NoQueueName", "QueueName")]
    [TestCase("NoQueueStringUsing", "QueueString")]
    public void Create_HasQueueInConfigWithoutParameter_Exception(string queueId, string paramName)
    {
      Assert.Throws<ArgumentNullException>(() => _constructor.Create(queueId),
        $"Value cannot be null. (Parameter '{paramName}')");
    }

    [Test]
    public void Create_NoQueueStringAndUseQueueStringIsFalse_ReturnsAdapter()
    {
      var adapter = _constructor.Create(NoQueueStringNotUsingQueueId);

      Assert.Multiple(() =>
      {
        Assert.That(adapter, Is.Not.Null);
        Assert.That(adapter.Configuration, Is.Not.Null);
        Assert.That(adapter.Configuration.Id, Is.EqualTo(NoQueueStringNotUsingQueueId));
      });
    }

    [Test]
    public void Create_Subscribing_ReturnsSubscribingAdapter()
    {
      var adapter = _constructor.Create(SubscribingQueueId);

      Assert.That(adapter, Is.InstanceOf<ActiveMqQueueSubscribingMessageAdapter>());
    }

    [Test]
    public void Create_ThreadedNoTrans_ReturnsMessageAdapter()
    {
      var adapter = _constructor.Create(ThreadQueueId);

      Assert.That(adapter, Is.InstanceOf<ActiveMqQueueMessageAdapter>());
    }

    [Test]
    public void Create_ThreadedWithTrans_ReturnsTransactedAdapter()
    {
      var adapter = _constructor.Create(TransactedQueueId);

      Assert.That(adapter, Is.InstanceOf<ActiveMqQueueTransactedMessageAdapter>());
    }

    [Test]
    public void CreateAll_HasConfig_ReturnAdapters()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("appsettingsAll.json", true)
        .Build();

      var constructor =
        new ActiveMqQueueMessageAdapterConstructor(configuration, Given.FakeLoggerFactory, Given.FakeConnectionFactory);

      var adapters = constructor.CreateAll();

      Assert.Multiple(() =>
      {
        Assert.That(adapters, Is.Not.Null);
        Assert.That(adapters.Count, Is.EqualTo(2));
        Assert.That(adapters.First().Configuration, Is.Not.Null);
        Assert.That(adapters.First().Configuration.Id, Is.EqualTo(QueueId));
        Assert.That(adapters.Last().Configuration, Is.Not.Null);
        Assert.That(adapters.Last().Configuration.Id, Is.EqualTo("NextValue"));
      });
    }

    [Test]
    public void CreateAll_HasNoClientIdQueueInConfig_Exception()
    {
      Assert.Throws<ArgumentNullException>(() => _constructor.CreateAll(),
        "Value cannot be null. (Parameter 'ClientId')");
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
      var hasQueue = _constructor.HasQueue(NoServerQueueId);

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