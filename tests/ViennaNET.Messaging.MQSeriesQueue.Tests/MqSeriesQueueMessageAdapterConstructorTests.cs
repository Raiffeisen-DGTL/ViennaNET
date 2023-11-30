using System.Reflection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.MQSeriesQueue.Tests.DSL;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueMessageAdapterConstructor))]
  public class MqSeriesQueueMessageAdapterConstructorTests
  {
    [OneTimeSetUp]
    public void MqSeriesQueueMessageAdapterConstructorSetup()
    {
      using (var configStream = Assembly.GetExecutingAssembly()
               .GetManifestResourceStream("ViennaNET.Messaging.MQSeriesQueue.Tests.appsettingsFailed.json"))
      {
        _failedConfig = new ConfigurationBuilder()
          .AddJsonStream(configStream)
          .Build();
      }

      using (var configStream = Assembly.GetExecutingAssembly()
               .GetManifestResourceStream("ViennaNET.Messaging.MQSeriesQueue.Tests.appsettings.json"))
      {
        var configuration = new ConfigurationBuilder()
          .AddJsonStream(configStream)
          .Build();

        _constructor = new MqSeriesQueueMessageAdapterConstructor(configuration, Given.FakeLoggerFactory);
      }
    }

    private const string QueueId = "ReValue";
    private const string NoQueueStringNotUsingQueueId = "NoQueueStringNotUsing";
    private const string ThreadQueueId = "ReValue";
    private const string TransactedQueueId = "Transacted";
    private const string SubscribingQueueId = "Subscribing";

    private IConfiguration _failedConfig;
    private MqSeriesQueueMessageAdapterConstructor _constructor;

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
      });
    }

    [Test]
    public void Create_HasQueueInConfigWithoutParameter_Exception()
    {
      var exception = Assert.Throws<MessagingException>(() =>
        new MqSeriesQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory));
      Assert.Multiple(() =>
      {
        StringAssert.Contains("ClientId", exception.Message);
        StringAssert.Contains("Server", exception.Message);
        StringAssert.Contains("QueueName", exception.Message);
        StringAssert.Contains("QueueManager", exception.Message);
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

      Assert.That(adapter, Is.InstanceOf<MqSeriesQueueSubscribingMessageAdapter>());
    }

    [Test]
    public void Create_ThreadedNoTrans_ReturnsTransactedAdapter()
    {
      var adapter = _constructor.Create(ThreadQueueId);

      Assert.That(adapter, Is.InstanceOf<MqSeriesQueueMessageAdapter>());
    }

    [Test]
    public void Create_ThreadedWithTrans_ReturnsTransactedAdapter()
    {
      var adapter = _constructor.Create(TransactedQueueId);

      Assert.That(adapter, Is.InstanceOf<MqSeriesQueueTransactedMessageAdapter>());
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