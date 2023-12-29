using System.Reflection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.ActiveMQQueue.Tests.DSL;
using ViennaNET.Messaging.Exceptions;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests;

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
                .AddJsonStream(configStream!)
                .Build();
        }

        using (var configStream = Assembly.GetExecutingAssembly()
                   .GetManifestResourceStream("ViennaNET.Messaging.ActiveMQQueue.Tests.appsettings.json"))
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(configStream!)
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
    private ActiveMqQueueMessageAdapterConstructor _constructor = null!;
    private IConfiguration _failedConfig = null!;

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
        Assert.That(() =>
                new ActiveMqQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory,
                    Given.FakeConnectionFactory),
            Throws.InstanceOf<MessagingException>()
                .And.Message.Contains("Server")
                .And.Message.Contains("QueueName")
                .And.Message.Contains("QueueString"));
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

        Assert.That(adapters, Is.Not.Empty);
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