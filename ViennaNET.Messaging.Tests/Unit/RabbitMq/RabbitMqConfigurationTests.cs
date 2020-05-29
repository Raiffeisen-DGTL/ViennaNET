using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.RabbitMQQueue;

namespace ViennaNET.Messaging.Tests.Unit.RabbitMq
{
  [TestFixture(Category = "Unit", TestOf = typeof(RabbitMqConfiguration))]
  public class RabbitMqConfigurationTests
  {
    [Test]
    public void GetConfiguration_HasOneQueueWithId_ReturnsQueueConfiguration()
    {
      var configuration = new RabbitMqConfiguration();
      var queueConfiguration = new RabbitMqQueueConfiguration { Id = "id" };
      configuration.Queues.Add(queueConfiguration);

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result == queueConfiguration);
    }

    [Test]
    public void GetConfiguration_HasTwoQueueWithSameId_Exception()
    {
      var configuration = new RabbitMqConfiguration();
      var queueConfiguration = new RabbitMqQueueConfiguration { Id = "id" };
      configuration.Queues.Add(queueConfiguration);
      configuration.Queues.Add(queueConfiguration);

      Assert.Throws<MessagingConfigurationException>(() => configuration.GetQueueConfiguration("id"));
    }

    [Test]
    public void GetConfiguration_HasNoQueueWithId_ReturnsNull()
    {
      var configuration = new RabbitMqConfiguration();

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result == null);
    }
  }
}