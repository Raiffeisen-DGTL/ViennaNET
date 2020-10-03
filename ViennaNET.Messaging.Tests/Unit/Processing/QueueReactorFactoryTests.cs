using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.Processing.Impl;
using ViennaNET.Messaging.Tests.Unit.DSL;

namespace ViennaNET.Messaging.Tests.Unit.Processing
{
  [TestFixture(Category = "Unit", TestOf = typeof(QueueReactorFactory))]
  public class QueueReactorFactoryTests
  {
    [Test]
    public void CreateQueueReactorTest()
    {
      const string queueName = "ReName";
      var messageProcessor = new Mock<IMessageProcessor>().Object;

      var queueReactorFactory = Given.QueueReactorFactory.WithMessageProcessor(messageProcessor, queueName).Please();
      var result = queueReactorFactory.CreateQueueReactor(queueName);

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void RegisterTest()
    {
      const string queueName = "ReQueueName";
      var queueReactorFactory = Given.QueueReactorFactory.Please();

      var result = queueReactorFactory.Register<object>(queueName);

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void RegisterTwiceTest()
    {
      const string queueName = "ReQueueName";
      var queueReactorFactory = Given.QueueReactorFactory.Please();
      queueReactorFactory.Register<object>(queueName);

      Assert.Throws<MessageProcessorAlreadyRegisterException>(() => queueReactorFactory.Register<object>(queueName));
    }

    [Test]
    public void NoMessageProcessorsForQueueTest()
    {
      const string queueId = "ReName";
      var queueReactorFactory = Given.QueueReactorFactory.Please();

      var ex = Assert.Throws<MessagingException>(() => queueReactorFactory.CreateQueueReactor(queueId));
      Assert.IsTrue(ex.Message.Contains("There are no message processors registered for queue"));
    }

    [Test]
    public void UnregisteredMessageProcessorTest()
    {
      const string queueId = "ReName";
      var messageProcessor = new Mock<IMessageProcessor>().Object;
      var messageProcessorUnreg = new Mock<IMessageProcessorAsync>().Object;
      var queueReactorFactory = Given
        .QueueReactorFactory
        .WithMessageProcessor(messageProcessor, queueId)
        .WithMessageProcessorAsync(messageProcessorUnreg)
        .Please();

      var ex = Assert.Throws<MessagingException>(() => queueReactorFactory.CreateQueueReactor(queueId));
      Assert.IsTrue(ex.Message.Contains("are not registered in the queue factory"));
    }

    [Test]
    public void MessageProcessorNotSupportsProcessingTypeTest()
    {
      const string queueId = "ReName";
      var messageProcessor = new Mock<IMessageProcessor>().Object;
      var messageAdapter = Given
        .MessageAdapter
        .WithQueueConfiguration(new MqSeriesQueueConfiguration { ProcessingType = MessageProcessingType.ThreadStrategy, IntervalPollingQueue = 1000 })
        .SupportsProcessingType(MessageProcessingType.Subscribe)
        .Please();
      var queueReactorFactory = Given
        .QueueReactorFactory
        .WithMessageAdapter(messageAdapter)
        .WithMessageProcessor(messageProcessor, queueId)
        .Please();
      
      var ex = Assert.Throws<MessagingException>(() => queueReactorFactory.CreateQueueReactor(queueId));
      Assert.IsTrue(ex.Message.Contains("Processing type"));
    }
  }
}