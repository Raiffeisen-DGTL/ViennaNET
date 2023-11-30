using System;
using Confluent.Kafka;
using NUnit.Framework;
using ViennaNET.Messaging.KafkaQueue.Tests.DSL;

namespace ViennaNET.Messaging.KafkaQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(KafkaQueueTransactedMessageAdapter))]
  internal class KafkaQueueTransactedMessageAdapterTests
  {
    [Test]
    public void CommitIfTransacted_ShouldCommit()
    {
      var config = new KafkaQueueConfiguration { ConsumerConfig = new ConsumerConfig() };
      var factory = Given.KafkaConnectionFactory;
      var adapter =
        new KafkaQueueTransactedMessageAdapter(config, factory, Given.GetLogger<KafkaQueueTransactedMessageAdapter>());

      adapter.Connect();
      adapter.CommitIfTransacted(null);

      factory.ConsumerMock.Verify(
        m => m.Commit());
    }

    [Test]
    public void Connect_ShouldStartTransactions()
    {
      var config = new KafkaQueueConfiguration { ProducerConfig = new ProducerConfig(), IntervalPollingQueue = 10000 };
      var factory = Given.KafkaConnectionFactory;
      var adapter =
        new KafkaQueueTransactedMessageAdapter(config, factory, Given.GetLogger<KafkaQueueTransactedMessageAdapter>());

      adapter.Connect();

      Assert.Multiple(() =>
      {
        factory.ProducerMock.Verify(
          m => m.InitTransactions(TimeSpan.FromMilliseconds(config.IntervalPollingQueue)));
        factory.ProducerMock.Verify(
          m => m.BeginTransaction());
      });
    }

    [Test]
    public void Disconnect_ShouldCommitTransaction()
    {
      var config = new KafkaQueueConfiguration { ProducerConfig = new ProducerConfig() };
      var factory = Given.KafkaConnectionFactory;
      var adapter =
        new KafkaQueueTransactedMessageAdapter(config, factory, Given.GetLogger<KafkaQueueTransactedMessageAdapter>());

      adapter.Connect();
      adapter.Disconnect();

      factory.ProducerMock.Verify(
        m => m.CommitTransaction());
    }
  }
}