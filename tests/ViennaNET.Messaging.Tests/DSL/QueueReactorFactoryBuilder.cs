using System.Collections.Generic;
using System.Linq;
using Moq;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.Processing.Impl;

namespace ViennaNET.Messaging.Tests.DSL
{
  internal class QueueReactorFactoryBuilder
  {
    private readonly LinkedList<MessageProcessorData> _messageProcessors = new();

    private readonly LinkedList<MessageProcessorAsyncData> _messageProcessorsAsync =
      new();

    private IMessageAdapter _messageAdapter;

    public QueueReactorFactoryBuilder WithMessageProcessor(IMessageProcessor messageProcessor,
      string queueNameToRegister = null)
    {
      _messageProcessors.AddLast(new MessageProcessorData
      {
        messageProcessor = messageProcessor, queueName = queueNameToRegister
      });
      return this;
    }

    public QueueReactorFactoryBuilder WithMessageProcessorAsync(IMessageProcessorAsync messageProcessor,
      string queueNameToRegister = null)
    {
      _messageProcessorsAsync.AddLast(new MessageProcessorAsyncData
      {
        messageProcessor = messageProcessor, queueName = queueNameToRegister
      });
      return this;
    }

    public QueueReactorFactoryBuilder WithMessageAdapter(IMessageAdapter messageAdapter)
    {
      _messageAdapter = messageAdapter;
      return this;
    }

    public QueueReactorFactory Please()
    {
      var messageAdapterFactory = new Mock<IMessageAdapterFactory>();
      messageAdapterFactory
        .Setup(x => x.Create(It.IsAny<string>()))
        .Returns(_messageAdapter ?? Given.MessageAdapter.Please());

      var healthCheckingService = new Mock<IHealthCheckingService>();

      var fakeContextAccessor = new Mock<MessagingCallContextAccessor>();

      var factory = new QueueReactorFactory(
        messageAdapterFactory.Object,
        _messageProcessors.Select(d => d.messageProcessor),
        _messageProcessorsAsync.Select(d => d.messageProcessor),
        healthCheckingService.Object,
        fakeContextAccessor.Object,
        Given.FakeLoggerFactory
      );

      foreach (var processorData in _messageProcessors.Where(d => !string.IsNullOrEmpty(d.queueName)))
      {
        factory.Register(processorData.messageProcessor.GetType(), processorData.queueName);
      }

      foreach (var processorData in _messageProcessorsAsync.Where(d => !string.IsNullOrEmpty(d.queueName)))
      {
        factory.Register(processorData.messageProcessor.GetType(), processorData.queueName);
      }

      return factory;
    }

    private struct MessageProcessorData
    {
      public IMessageProcessor messageProcessor;
      public string queueName;
    }

    private struct MessageProcessorAsyncData
    {
      public IMessageProcessorAsync messageProcessor;
      public string queueName;
    }
  }
}