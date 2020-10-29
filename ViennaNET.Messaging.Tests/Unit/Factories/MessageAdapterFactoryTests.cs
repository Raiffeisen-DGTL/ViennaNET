using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Messaging.KafkaQueue;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.RabbitMQQueue;
using ViennaNET.Messaging.Tests.Unit.DSL;

namespace ViennaNET.Messaging.Tests.Unit.Factories
{
  [TestFixture(Category = "Unit", TestOf = typeof(MessageAdapterFactory))]
  public class MessageAdapterFactoryTests
  {
    private const string MqSeriesQueueId = "ReValue";
    private const string RabbitQueueId = "Rabbit";
    private const string KafkaQueueId = "testKafkaQueue";
    private const string DuplicateQueueId = "T2";

    private IConfigurationRoot _configuration;

    [OneTimeSetUp]
    public void Setup()
    {
      _configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .Build();
    }

    [Test]
    public void Create_MqSeries_Success()
    {
      var factory = Given
        .MessageAdapterFactory
        .WithMqSeries(_configuration)
        .WithKafka(_configuration)
        .WithRabbit(_configuration)
        .Please();

      var adapter = factory.Create(MqSeriesQueueId);

      Assert.IsInstanceOf<MqSeriesQueueMessageAdapterBase>(adapter);
    }  
    
    [Test]
    public void Create_Rabbit_Success()
    {
      var factory = Given
        .MessageAdapterFactory
        .WithMqSeries(_configuration)
        .WithKafka(_configuration)
        .WithRabbit(_configuration)
        .Please();

      var adapter = factory.Create(RabbitQueueId);

      Assert.IsInstanceOf<RabbitMqQueueMessageAdapter>(adapter);
    } 
    
    [Test]
    public void Create_Kafka_Success()
    {
      var factory = Given
        .MessageAdapterFactory
        .WithMqSeries(_configuration)
        .WithKafka(_configuration)
        .WithRabbit(_configuration)
        .Please();

      var adapter = factory.Create(KafkaQueueId);

      Assert.IsInstanceOf<KafkaQueueMessageAdapter>(adapter);
    }

    [Test]
    public void Create_NonExistingId_Exception()
    {
      var factory = Given
        .MessageAdapterFactory
        .WithMqSeries(_configuration)
        .WithKafka(_configuration)
        .WithRabbit(_configuration)
        .Please();

      Assert.Throws<MessagingConfigurationException>(() => factory.Create("SomeOther"));
    }

    [Test]
    public void Create_DuplicateId_Exception()
    {
      var factory = Given
        .MessageAdapterFactory
        .WithMqSeries(_configuration)
        .WithKafka(_configuration)
        .WithRabbit(_configuration)
        .Please();

      Assert.Throws<MessagingConfigurationException>(() => factory.Create(DuplicateQueueId));
    }

    [Test]
    public void Create_DuplicateConstructor_Exception()
    {
      var factory = Given
        .MessageAdapterFactory
        .WithMqSeries(_configuration)
        .WithKafka(_configuration)
        .WithRabbit(_configuration)
        .WithRabbit(_configuration)
        .Please();

      Assert.Throws<MessagingConfigurationException>(() => factory.Create(RabbitQueueId));
    }

    [Test]
    public void Create_NoConstructors_Exception()
    {
      var factory = Given
        .MessageAdapterFactory
        .Please();

      Assert.Throws<MessagingConfigurationException>(() => factory.Create(KafkaQueueId));
    }
  }
}