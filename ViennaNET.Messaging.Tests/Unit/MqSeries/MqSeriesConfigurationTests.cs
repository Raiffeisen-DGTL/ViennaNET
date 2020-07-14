using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.MQSeriesQueue;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesConfiguration))]
  public class MqSeriesConfigurationTests
  {
    [Test]
    public void GetConfiguration_HasOneQueueWithId_ReturnsQueueConfiguration()
    {
      var configuration = new MqSeriesConfiguration();
      var queueConfiguration = new MqSeriesQueueConfiguration { Id = "id" };
      configuration.Queues.Add(queueConfiguration);

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result == queueConfiguration);
    }

    [Test]
    public void GetConfiguration_HasTwoQueueWithSameId_Exception()
    {
      var configuration = new MqSeriesConfiguration();
      var queueConfiguration = new MqSeriesQueueConfiguration { Id = "id" };
      configuration.Queues.Add(queueConfiguration);
      configuration.Queues.Add(queueConfiguration);

      Assert.Throws<MessagingConfigurationException>(() => configuration.GetQueueConfiguration("id"));
    }

    [Test]
    public void GetConfiguration_HasNoQueueWithId_ReturnsNull()
    {
      var configuration = new MqSeriesConfiguration();

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result == null);
    }

    [Test]
    public void GetConfiguration_HasSelectors_ReturnNull()
    {
      var configuration = new MqSeriesConfiguration();

      var result = configuration.GetQueueConfiguration("Selectors");

      Assert.That(result == null);
    }

    [Test]
    public void GetConfiguration_HasSelectors_ReturnNotNull()
    {
      var configuration = new MqSeriesConfiguration();

      configuration.Queues.Add(new MqSeriesQueueConfiguration{Id = "id", Selector = "TEST = 'TEST'" });

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result.Selector == "TEST = 'TEST'");
    }
  }
}