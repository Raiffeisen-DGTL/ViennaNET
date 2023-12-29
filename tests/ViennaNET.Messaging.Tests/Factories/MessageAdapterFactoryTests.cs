using System.Reflection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.ActiveMQQueue;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Messaging.KafkaQueue;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.RabbitMQQueue;
using ViennaNET.Messaging.Tests.DSL;

namespace ViennaNET.Messaging.Tests.Factories;

[TestFixture(Category = "Unit", TestOf = typeof(MessageAdapterFactory))]
public class MessageAdapterFactoryTests
{
    [OneTimeSetUp]
    public void Setup()
    {
        using var configStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("ViennaNET.Messaging.Tests.appsettings.json");
        _configuration = new ConfigurationBuilder()
            .AddJsonStream(configStream!)
            .Build();
    }

    private const string ActiveMqQueueId = "ActiveMQ";
    private const string MqSeriesQueueId = "ReValue";
    private const string RabbitQueueId = "Rabbit";
    private const string KafkaQueueId = "testKafkaQueue";
    private const string DuplicateQueueId = "T2";

    private IConfigurationRoot _configuration = null!;

    [Test]
    public void Create_ActiveMq_Success()
    {
        var factory = Given
            .MessageAdapterFactory
            .WithActiveMq(_configuration)
            .WithMqSeries(_configuration)
            .WithKafka(_configuration)
            .WithRabbit(_configuration)
            .Please();

        var adapter = factory.Create(ActiveMqQueueId);

        Assert.That(adapter, Is.InstanceOf<ActiveMqQueueMessageAdapter>());
    }

    [Test]
    public void Create_DuplicateConstructor_Exception()
    {
        var factory = Given
            .MessageAdapterFactory
            .WithActiveMq(_configuration)
            .WithMqSeries(_configuration)
            .WithKafka(_configuration)
            .WithRabbit(_configuration)
            .WithRabbit(_configuration)
            .Please();

        Assert.Throws<MessagingConfigurationException>(() => factory.Create(RabbitQueueId));
    }

    [Test]
    public void Create_DuplicateId_Exception()
    {
        var factory = Given
            .MessageAdapterFactory
            .WithActiveMq(_configuration)
            .WithMqSeries(_configuration)
            .WithKafka(_configuration)
            .WithRabbit(_configuration)
            .Please();

        Assert.Throws<MessagingConfigurationException>(() => factory.Create(DuplicateQueueId));
    }

    [Test]
    public void Create_Kafka_Success()
    {
        var factory = Given
            .MessageAdapterFactory
            .WithActiveMq(_configuration)
            .WithMqSeries(_configuration)
            .WithKafka(_configuration)
            .WithRabbit(_configuration)
            .Please();

        var adapter = factory.Create(KafkaQueueId);

        Assert.That(adapter, Is.InstanceOf<KafkaQueueMessageAdapter>());
    }

    [Test]
    public void Create_MqSeries_Success()
    {
        var factory = Given
            .MessageAdapterFactory
            .WithActiveMq(_configuration)
            .WithMqSeries(_configuration)
            .WithKafka(_configuration)
            .WithRabbit(_configuration)
            .Please();

        var adapter = factory.Create(MqSeriesQueueId);

        Assert.That(adapter, Is.InstanceOf<MqSeriesQueueMessageAdapter>());
    }

    [Test]
    public void Create_NoConstructors_Exception()
    {
        var factory = Given
            .MessageAdapterFactory
            .Please();

        Assert.Throws<MessagingConfigurationException>(() => factory.Create(KafkaQueueId));
    }

    [Test]
    public void Create_NonExistingId_Exception()
    {
        var factory = Given
            .MessageAdapterFactory
            .WithActiveMq(_configuration)
            .WithMqSeries(_configuration)
            .WithKafka(_configuration)
            .WithRabbit(_configuration)
            .Please();

        Assert.Throws<MessagingConfigurationException>(() => factory.Create("SomeOther"));
    }

    [Test]
    public void Create_Rabbit_Success()
    {
        var factory = Given
            .MessageAdapterFactory
            .WithActiveMq(_configuration)
            .WithMqSeries(_configuration)
            .WithKafka(_configuration)
            .WithRabbit(_configuration)
            .Please();

        var adapter = factory.Create(RabbitQueueId);

        Assert.That(adapter, Is.InstanceOf<RabbitMqQueueMessageAdapter>());
    }
}