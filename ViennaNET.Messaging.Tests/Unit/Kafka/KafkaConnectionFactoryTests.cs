using NUnit.Framework;
using ViennaNET.Messaging.KafkaQueue;

namespace ViennaNET.Messaging.Tests.Unit.Kafka
{
  [TestFixture(Category = "Unit", TestOf = typeof(KafkaConnectionFactory))]
  internal class KafkaConnectionFactoryTests
  {
    [Test]
    public void CreateConsumer_NoArgs_ReturnsConsumer()
    {
      var factory = new KafkaConnectionFactory();

      var consumer = factory.CreateConsumer(new KafkaQueueConfiguration {GroupId = "groupId"});

      Assert.IsNotNull(consumer);
    }
    
    [Test]
    public void CreateProducer_NoArgs_ReturnsProducer()
    {
      var factory = new KafkaConnectionFactory();

      var producer = factory.CreateProducer(new KafkaQueueConfiguration());

      Assert.IsNotNull(producer);
    }
  }
}
