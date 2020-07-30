using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Messaging.KafkaQueue;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.RabbitMQQueue;

namespace ViennaNET.Messaging.Tests.Unit.Factories
{
  [TestFixture(Category = "Unit", TestOf = typeof(MessageAdapterFactory))]
  public class MessageAdapterFactoryTests
  {
    private IConfigurationRoot _configuration;

    [OneTimeSetUp]
    public void Setup()
    {
      _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true)
                                                 .Build();
    }

    [Test]
    public void CreateTest()
    {
      var fakeAdvancedBusFactory = new Mock<IAdvancedBusFactory>();
      var adapterConstructors = new List<IMessageAdapterConstructor>
      {
        new MqSeriesQueueMessageAdapterConstructor(_configuration), new RabbitMqQueueMessageAdapterConstructor(fakeAdvancedBusFactory.Object, _configuration), new KafkaQueueMessageAdapterConstructor(_configuration)
      };
      var messageAdapterFactory = new MessageAdapterFactory(adapterConstructors);
      Assert.Multiple(() =>
      {
        Assert.That(messageAdapterFactory.Create("ReValue", false), Is.Not.Null);
        Assert.That(messageAdapterFactory.Create("ReValue", false) is MqSeriesQueueMessageAdapter, Is.True);
        Assert.That(messageAdapterFactory.Create("Rabbit", false), Is.Not.Null);
        Assert.That(messageAdapterFactory.Create("Rabbit", false) is RabbitMqQueueMessageAdapter, Is.True);
        Assert.That(messageAdapterFactory.Create("testKafkaQueue", false), Is.Not.Null);
        Assert.That(messageAdapterFactory.Create("testKafkaQueue", false) is KafkaQueueMessageAdapter, Is.True);
        Assert.Throws<MessagingConfigurationException>(() => messageAdapterFactory.Create("SomeOther", false));
        Assert.Throws<MessagingConfigurationException>(() => messageAdapterFactory.Create("T2", false));

        adapterConstructors.Add(new RabbitMqQueueMessageAdapterConstructor(fakeAdvancedBusFactory.Object, _configuration));
        Assert.Throws<MessagingConfigurationException>(() => messageAdapterFactory.Create("T1", false));

        adapterConstructors.Clear();
        Assert.Throws<MessagingConfigurationException>(() => messageAdapterFactory.Create("T1", false));
      });
    }
  }
}