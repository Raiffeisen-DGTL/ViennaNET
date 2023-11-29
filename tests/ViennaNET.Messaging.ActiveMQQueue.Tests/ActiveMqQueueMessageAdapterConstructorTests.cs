using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.ActiveMQQueue.Tests.DSL;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(ActiveMqQueueMessageAdapterConstructor))]
  public class ActiveMqQueueMessageAdapterConstructorTests
  {
    [OneTimeSetUp]
    public void MqSeriesQueueMessageAdapterConstructorSetup()
    {
      using (var configStream = Assembly.GetExecutingAssembly()
               .GetManifestResourceStream("ViennaNET.Messaging.ActiveMQQueue.Tests.appsettingsFailed.json"))
      {
        _failedConfig = new ConfigurationBuilder()
          .AddJsonStream(configStream)
          .Build();
      }

      using (var configStream = Assembly.GetExecutingAssembly()
               .GetManifestResourceStream("ViennaNET.Messaging.ActiveMQQueue.Tests.appsettings.json"))
      {
        var configuration = new ConfigurationBuilder()
          .AddJsonStream(configStream)
          .Build();

        _constructor = new ActiveMqQueueMessageAdapterConstructor(configuration,
          Given.FakeLoggerFactory, Given.FakeConnectionFactory);
      }
    }

    private const string QueueId = "ActiveMQ";
    private const string NoQueueStringNotUsingQueueId = "NoQueueStringNotUsing";
    private const string ThreadQueueId = "ActiveMQ";
    private const string TransactedQueueId = "Transacted";
    private const string SubscribingQueueId = "Subscribing";

    private IMessageAdapterConstructor _constructor;
    private IConfiguration _failedConfig;

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
    public void Create_HasQueueInConfigWithoutParameter_Exception()
    {
      var exception = Assert.Throws<MessagingException>(() =>
        new ActiveMqQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory,
          Given.FakeConnectionFactory));
      Assert.Multiple(() =>
      {
        StringAssert.Contains("Server", exception.Message);
        StringAssert.Contains("QueueName", exception.Message);
        StringAssert.Contains("QueueString", exception.Message);
      });
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
      var adapters = _constructor.CreateAll();

      CollectionAssert.IsNotEmpty(adapters);
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
  }
}