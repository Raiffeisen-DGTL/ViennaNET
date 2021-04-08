using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(ActiveMqConfiguration))]
  public class ActiveMqConfigurationTests
  {
    [Test]
    public void GetConfiguration_HasNoQueueWithId_ReturnsNull()
    {
      var configuration = new ActiveMqConfiguration();

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result == null);
    }

    [Test]
    public void GetConfiguration_HasOneQueueWithId_ReturnsQueueConfiguration()
    {
      var configuration = new ActiveMqConfiguration();
      var queueConfiguration = new ActiveMqQueueConfiguration { Id = "id" };
      configuration.Queues.Add(queueConfiguration);

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result == queueConfiguration);
    }

    [Test]
    public void GetConfiguration_HasSelectors_ReturnNotNull()
    {
      var configuration = new ActiveMqConfiguration();

      configuration.Queues.Add(new ActiveMqQueueConfiguration { Id = "id", Selector = "TEST = 'TEST'" });

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result.Selector == "TEST = 'TEST'");
    }

    [Test]
    public void GetConfiguration_HasSelectors_ReturnNull()
    {
      var configuration = new ActiveMqConfiguration();

      var result = configuration.GetQueueConfiguration("Selectors");

      Assert.That(result == null);
    }

    [Test]
    public void GetConfiguration_HasTwoQueueWithSameId_Exception()
    {
      var configuration = new ActiveMqConfiguration();
      var queueConfiguration = new ActiveMqQueueConfiguration { Id = "id" };
      configuration.Queues.Add(queueConfiguration);
      configuration.Queues.Add(queueConfiguration);

      Assert.Throws<MessagingConfigurationException>(() => configuration.GetQueueConfiguration("id"));
    }
  }
}