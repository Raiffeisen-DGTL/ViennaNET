using System;
using Moq;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class MessageAdapterBuilder
  {

    private QueueConfigurationBase _queueConfiguration;
    private MessageProcessingType? _processingType;

    public MessageAdapterBuilder WithQueueConfiguration(QueueConfigurationBase configuration)
    {
      _queueConfiguration = configuration;
      return this;
    }

    public MessageAdapterBuilder SupportsProcessingType(MessageProcessingType processingType)
    {
      _processingType = processingType;
      return this;
    }

    public T Please<T>(Action<Mock<T>> configure = null) where T: class, IMessageAdapter
    {
      var messageAdapter = new Mock<T>();
      messageAdapter
        .Setup(x => x.Configuration)
        .Returns(_queueConfiguration ?? new MqSeriesQueueConfiguration { IntervalPollingQueue = 1000, ProcessingType = MessageProcessingType.ThreadStrategy });
      messageAdapter
        .Setup(x => x.SupportProcessingType(It.Is<MessageProcessingType>(pt => !_processingType.HasValue || pt == _processingType.Value)))
        .Returns(true);
      messageAdapter
        .Setup(x => x.Send(It.IsAny<BaseMessage>()))
        .Returns<BaseMessage>(m => m);

      configure?.Invoke(messageAdapter);

      return messageAdapter.Object;
    }

    public IMessageAdapter Please(Action<Mock<IMessageAdapter>> configure = null)
    {
      return Please<IMessageAdapter>(configure);
    }
  }
}
