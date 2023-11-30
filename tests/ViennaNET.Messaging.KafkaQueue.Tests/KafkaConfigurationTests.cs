﻿using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;

namespace ViennaNET.Messaging.KafkaQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(KafkaConfiguration))]
  public class KafkaConfigurationTests
  {
    [Test]
    public void GetConfiguration_HasNoQueueWithId_ReturnsNull()
    {
      var configuration = new KafkaConfiguration();

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result == null);
    }

    [Test]
    public void GetConfiguration_HasOneQueueWithId_ReturnsQueueConfiguration()
    {
      var configuration = new KafkaConfiguration();
      var queueConfiguration = new KafkaQueueConfiguration { Id = "id" };
      configuration.Queues.Add(queueConfiguration);

      var result = configuration.GetQueueConfiguration("id");

      Assert.That(result == queueConfiguration);
    }

    [Test]
    public void GetConfiguration_HasTwoQueueWithSameId_Exception()
    {
      var configuration = new KafkaConfiguration();
      var queueConfiguration = new KafkaQueueConfiguration { Id = "id" };
      configuration.Queues.Add(queueConfiguration);
      configuration.Queues.Add(queueConfiguration);

      Assert.Throws<MessagingConfigurationException>(() => configuration.GetQueueConfiguration("id"));
    }
  }
}