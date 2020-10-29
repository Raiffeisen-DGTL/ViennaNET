using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Processing.Impl.Subscribe;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class QueueSubscribedReactorBuilder
  {
    private readonly LinkedList<IMessageProcessor> _messageProcessors = new LinkedList<IMessageProcessor>();

    private IMessageAdapterWithSubscribing _messageAdapter;

    public QueueSubscribedReactorBuilder WithMessageAdapter(IMessageAdapterWithSubscribing adapter)
    {
      _messageAdapter = adapter;
      return this;
    }

    public QueueSubscribedReactorBuilder WithMessageAdapter(Func<MessageAdapterBuilder, IMessageAdapterWithSubscribing> builder)
    {
      _messageAdapter = builder(new MessageAdapterBuilder());
      return this;
    }

    public QueueSubscribedReactorBuilder WithMessageProcessor(IMessageProcessor processor)
    {
      _messageProcessors.AddLast(processor);
      return this;
    }

    public QueueSubscribedReactor Please()
    {
      var loggerMock = new Mock<ILogger<QueueSubscribedReactor>>() {DefaultValue = DefaultValue.Mock};

      return new QueueSubscribedReactor(
        _messageAdapter ?? Mock.Of<IMessageAdapterWithSubscribing>(),
        _messageProcessors,
        new IMessageProcessorAsync[0],
        100,
        null,
        Mock.Of<IHealthCheckingService>(),
        Mock.Of<IMessagingCallContextAccessor>(),
        loggerMock.Object);
    }
  }
}
