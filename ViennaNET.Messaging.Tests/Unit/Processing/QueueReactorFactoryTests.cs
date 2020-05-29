using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.Processing.Impl;

namespace ViennaNET.Messaging.Tests.Unit.Processing
{
  [TestFixture(Category = "Unit", TestOf = typeof(QueueReactorFactory))]
  public class QueueReactorFactoryTests
  {
    private IEnumerable<IMessageProcessor> _messageProcessors;
    private IEnumerable<IMessageProcessorAsync> _asyncMessageProcessors;
    private Mock<IHealthCheckingService> _healthCheckingService;
    private Mock<IMessageAdapterFactory> _messageAdapterFactory;

    [OneTimeSetUp]
    public void Setup()
    {
      _messageProcessors = new List<IMessageProcessor>();
      _asyncMessageProcessors = new List<IMessageProcessorAsync>();
      _healthCheckingService = new Mock<IHealthCheckingService>();
      _messageAdapterFactory = new Mock<IMessageAdapterFactory>();
    }

    [Test]
    public void CreateQueueReactorTest()
    {
      const string queueName = "ReName";

      var configuration = new MqSeriesQueueConfiguration { IntervalPollingQueue = 1000 };
      var messageAdapter = new Mock<IMessageAdapter>();
      messageAdapter.Setup(x => x.Configuration)
                    .Returns(configuration);
      messageAdapter.Setup(x => x.SupportProcessingType(It.IsAny<MessageProcessingType>()))
                    .Returns(true);

      _messageAdapterFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<bool>()))
                            .Returns(messageAdapter.Object);
      var fakeContextAccessor = new Mock<MessagingCallContextAccessor>();

      var queueReactorFactory = new QueueReactorFactory(_messageAdapterFactory.Object, _messageProcessors,
                                                        _asyncMessageProcessors, _healthCheckingService.Object, fakeContextAccessor.Object);
      var result = queueReactorFactory.CreateQueueReactor(queueName);
      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void RegisterTest()
    {
      var fakeContextAccessor = new Mock<MessagingCallContextAccessor>();
      const string queueName = "ReQueueName";
      var queueReactorFactory = new QueueReactorFactory(_messageAdapterFactory.Object, _messageProcessors,
                                                        _asyncMessageProcessors, _healthCheckingService.Object, fakeContextAccessor.Object);

      var result = queueReactorFactory.Register<object>(queueName);
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.Throws<MessageProcessorAlreadyRegisterException>(() =>
        {
          queueReactorFactory.Register<object>(queueName);
        });
      });
    }
  }
}