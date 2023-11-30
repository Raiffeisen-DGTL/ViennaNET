using Confluent.Kafka;
using NUnit.Framework;

namespace ViennaNET.Messaging.KafkaQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(KafkaConnectionFactory))]
  internal class KafkaConnectionFactoryTests
  {
    [Test]
    public void CreateConsumer_NoArgs_ReturnsConsumer()
    {
      var factory = new KafkaConnectionFactory();

      var consumer = factory.CreateConsumer(new KafkaQueueConfiguration()
      {
        ConsumerConfig = new ConsumerConfig() { GroupId = "groupId" }
      });

      Assert.IsNotNull(consumer);
    }

    [Test]
    public void CreateProducer_NoArgs_ReturnsProducer()
    {
      var factory = new KafkaConnectionFactory();

      var producer = factory.CreateProducer(new KafkaQueueConfiguration()
      {
        ProducerConfig = new ProducerConfig()
      });

      Assert.IsNotNull(producer);
    }
  }
}