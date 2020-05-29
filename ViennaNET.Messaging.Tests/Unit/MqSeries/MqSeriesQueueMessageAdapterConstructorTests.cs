using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.MQSeriesQueue;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueMessageAdapterConstructor))]
  public class MqSeriesQueueMessageAdapterConstructorTests
  {
    private const string QueueId = "ReValue";
    private const string NoQueueStringNotUsingQueueId = "NoQueueStringNotUsing";
    private const string NoServerQueueId = "NoServer";

    private MqSeriesQueueMessageAdapterConstructor _constructor;

    [OneTimeSetUp]
    public void MqSeriesQueueMessageAdapterConstructorSetup()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true)
                                                 .Build();

      _constructor = new MqSeriesQueueMessageAdapterConstructor(configuration);
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
    public void Create_NoQueueStringAndUseQueueStringIsFalse_ReturnsAdapter()
    {
      var adapter = _constructor.Create(NoQueueStringNotUsingQueueId, false);

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
      Assert.Throws<MessagingConfigurationException>(() => _constructor.Create("", false), "There are no configuration with id '' in configuration file");
    }


    [Test]
    [TestCase("NoClientId", "ClientId")]
    [TestCase(NoServerQueueId, "Server")]
    [TestCase("NoQueueName", "QueueName")]
    [TestCase("NoQueueManager", "QueueManager")]
    [TestCase("NoQueueStringUsing", "QueueString")]
    public void Create_HasQueueInConfigWithoutParameter_Exception(string queueId, string paramName)
    {
      Assert.Throws<ArgumentNullException>(() => _constructor.Create(queueId, false), $"Value cannot be null. (Parameter '{paramName}')");
    }

    [Test]
    public void CreateAll_HasConfig_ReturnAdapters()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("appsettingsAll.json", true)
                                                    .Build();

      var constructor = new MqSeriesQueueMessageAdapterConstructor(configuration);

      var adapters = constructor.CreateAll(false);

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
      Assert.Throws<ArgumentNullException>(() => _constructor.CreateAll(false), "Value cannot be null. (Parameter 'ClientId')");
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