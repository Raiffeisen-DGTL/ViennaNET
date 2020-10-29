using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.Tests.Unit.DSL;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueMessageAdapterConstructor))]
  public class MqSeriesQueueMessageAdapterConstructorTests
  {
    private const string QueueId = "ReValue";
    private const string NoQueueStringNotUsingQueueId = "NoQueueStringNotUsing";
    private const string NoServerQueueId = "NoServer";
    private const string ThreadQueueId = "ReValue";
    private const string TransactedQueueId = "Transacted";
    private const string SubscribingQueueId = "Subscribing";

    private MqSeriesQueueMessageAdapterConstructor _constructor;

    [OneTimeSetUp]
    public void MqSeriesQueueMessageAdapterConstructorSetup()
    {
      var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", true)
        .Build();

      _constructor = new MqSeriesQueueMessageAdapterConstructor(configuration, Given.FakeLoggerFactory);
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
    public void Create_ThreadedNoTrans_ReturnsTransactedAdapter()
    {
      var adapter = _constructor.Create(ThreadQueueId);

      Assert.That(adapter, Is.InstanceOf<MqSeriesQueueTransactedMessageAdapter>());
    }

    [Test]
    public void Create_ThreadedWithTrans_ReturnsTransactedAdapter()
    {
      var adapter = _constructor.Create(TransactedQueueId);

      Assert.That(adapter, Is.InstanceOf<MqSeriesQueueTransactedMessageAdapter>());
    }

    [Test]
    public void Create_Subscribing_ReturnsSubscribingAdapter()
    {
      var adapter = _constructor.Create(SubscribingQueueId);

      Assert.That(adapter, Is.InstanceOf<MqSeriesQueueSubscribingMessageAdapter>());
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
    public void Create_HasNoQueueInConfig_Exception()
    {
      Assert.Throws<MessagingConfigurationException>(() => _constructor.Create(string.Empty), "There are no configuration with id '' in configuration file");
    }


    [Test]
    [TestCase("NoClientId", "ClientId")]
    [TestCase(NoServerQueueId, "Server")]
    [TestCase("NoQueueName", "QueueName")]
    [TestCase("NoQueueManager", "QueueManager")]
    [TestCase("NoQueueStringUsing", "QueueString")]
    public void Create_HasQueueInConfigWithoutParameter_Exception(string queueId, string paramName)
    {
      Assert.Throws<ArgumentNullException>(() => _constructor.Create(queueId), $"Value cannot be null. (Parameter '{paramName}')");
    }

    [Test]
    public void CreateAll_HasConfig_ReturnAdapters()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("appsettingsAll.json", true)
                                                    .Build();

      var constructor = new MqSeriesQueueMessageAdapterConstructor(configuration, Given.FakeLoggerFactory);

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
      Assert.Throws<ArgumentNullException>(() => _constructor.CreateAll(), "Value cannot be null. (Parameter 'ClientId')");
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
      var hasQueue = _constructor.HasQueue(NoServerQueueId);

      Assert.That(hasQueue);
    }
  }
}