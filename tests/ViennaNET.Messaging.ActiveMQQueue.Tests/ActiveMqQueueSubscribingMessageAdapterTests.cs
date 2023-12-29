using Apache.NMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.ActiveMQQueue.Tests.DSL;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(ActiveMqQueueSubscribingMessageAdapter))]
  public class ActiveMqQueueSubscribingMessageAdapterTests
  {
    [Test]
    public void SubscribeTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.SubscribingMessageAdapter
        .WithConnectionFactoryProvider(provider.WithMessageProducer(Given.MessageProducer("1", "2")).Please()).Please();

      adapter.Connect();
      adapter.Subscribe(msg => Task.CompletedTask);

      provider.MessageConsumerMock.VerifyAdd(x => x.Listener += It.IsAny<MessageListener>());
    }

    [Test]
    public void UnsubscribeTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.SubscribingMessageAdapter
        .WithConnectionFactoryProvider(provider.WithMessageProducer(Given.MessageProducer("1", "2")).Please()).Please();

      adapter.Connect();
      adapter.Subscribe(msg => Task.CompletedTask);
      adapter.Unsubscribe();

      provider.MessageConsumerMock.VerifyRemove(x => x.Listener -= It.IsNotNull<MessageListener>());
    }
  }
}