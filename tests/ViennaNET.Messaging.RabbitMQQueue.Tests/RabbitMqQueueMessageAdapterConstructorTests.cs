using System.Reflection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.RabbitMQQueue.Tests.DSL;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests;

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
                .AddJsonStream(configStream!)
                .Build();
        }

        using (var configStream = Assembly.GetExecutingAssembly()
                   .GetManifestResourceStream("ViennaNET.Messaging.RabbitMQQueue.Tests.appsettings.json"))
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(configStream!)
                .Build();

            _constructor = new RabbitMqQueueMessageAdapterConstructor(
                configuration,
                Given.FakeLoggerFactory);
        }
    }

    private const string QueueId = "Rabbit";
    private const string QueueWithoutServerId = "NextRabbit";

    private IConfiguration _failedConfig = null!;
    private RabbitMqQueueMessageAdapterConstructor _constructor = null!;

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
            Assert.That(adapter, Is.Not.Null);
            Assert.That(adapter.Configuration, Is.Not.Null);
            Assert.That(adapter.Configuration.Id, Is.EqualTo(QueueId));
        });
    }

    [Test]
    public void CreateAll_HasConfig_ReturnAdapters()
    {
        Assert.That(_constructor.CreateAll(), Is.Not.Empty);
    }

    [Test]
    public void Create_HasQueueInConfigWithoutParameter_Exception()
    {
        Assert.That(() => new RabbitMqQueueMessageAdapterConstructor(_failedConfig, Given.FakeLoggerFactory),
            Throws.InstanceOf<MessagingException>()
                .And.Message.Contains("AutoAck")
                .And.Message.Contains("Requeue"));
    }

    [Test]
    public void HasQueue_QueueExistsInConfig_True()
    {
        Assert.That(_constructor.HasQueue(QueueId), Is.True);
    }

    [Test]
    public void HasQueue_QueueExistsInConfigButBroken_True()
    {
        Assert.That(_constructor.HasQueue(QueueWithoutServerId), Is.True);
    }

    [Test]
    public void HasQueue_QueueNotExistsInConfig_False()
    {
        Assert.That(_constructor.HasQueue(""), Is.False);
    }
}