using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using ViennaNET.Messaging.ActiveMQQueue;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Messaging.KafkaQueue;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.RabbitMQQueue;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class MessageAdapterFactoryBuilder
  {
    private readonly LinkedList<IMessageAdapterConstructor>
      _constructors = new LinkedList<IMessageAdapterConstructor>();

    public MessageAdapterFactoryBuilder WithActiveMq(IConfiguration configuration)
    {
      _constructors.AddLast(new ActiveMqQueueMessageAdapterConstructor(configuration,
        Given.FakeLoggerFactory,
        Mock.Of<IActiveMqConnectionFactory>()));
      return this;
    }

    public MessageAdapterFactoryBuilder WithKafka(IConfiguration configuration)
    {
      _constructors.AddLast(new KafkaQueueMessageAdapterConstructor(configuration, Given.FakeLoggerFactory));
      return this;
    }

    public MessageAdapterFactoryBuilder WithRabbit(IConfiguration configuration)
    {
      _constructors.AddLast(new RabbitMqQueueMessageAdapterConstructor(
        Mock.Of<IAdvancedBusFactory>(),
        configuration,
        Given.FakeLoggerFactory));
      return this;
    }

    public MessageAdapterFactoryBuilder WithMqSeries(IConfiguration configuration)
    {
      _constructors.AddLast(new MqSeriesQueueMessageAdapterConstructor(configuration, Given.FakeLoggerFactory));
      return this;
    }

    public IMessageAdapterFactory Please()
    {
      return new MessageAdapterFactory(_constructors);
    }
  }
}