using System;
using System.Threading.Tasks;
using Moq;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class MessageAdapterBuilder
  {
    private MessageProcessingType? _processingType;
    private QueueConfigurationBase _queueConfiguration;

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

    public T Please<T>(Action<Mock<T>> configure = null) where T : class, IMessageAdapter
    {
      return MockPlease(configure).Object;
    }

    public IMessageAdapter Please(Action<Mock<IMessageAdapter>> configure = null)
    {
      return Please<IMessageAdapter>(configure);
    }

    public Mock<T> MockPlease<T>(Action<Mock<T>> configure = null) where T : class, IMessageAdapter
    {
      var messageAdapter = new Mock<T>();
      messageAdapter
        .Setup(x => x.Configuration)
        .Returns(_queueConfiguration ??
                 new MqSeriesQueueConfiguration
                 {
                   IntervalPollingQueue = 1000,
                   ProcessingType = MessageProcessingType.ThreadStrategy
                 });
      messageAdapter
        .Setup(x => x.SupportProcessingType(
          It.Is<MessageProcessingType>(pt => _processingType == null || pt == _processingType)))
        .Returns(true);
      messageAdapter
        .Setup(x => x.Send(It.IsAny<BaseMessage>()))
        .Returns<BaseMessage>(m => m);

      if (typeof(T) == typeof(IMessageAdapterWithSubscribing))
      {
        messageAdapter.As<IMessageAdapterWithSubscribing>()
          .Setup(x => x.Subscribe(It.IsAny<Func<BaseMessage, Task>>()))
          .Callback<Func<BaseMessage, Task>>(cb => messageAdapter
            .Setup(x => x.Send(It.IsAny<BaseMessage>()))
            .Callback<BaseMessage>(msg => cb(msg).GetAwaiter().GetResult()));
      }

      configure?.Invoke(messageAdapter);

      return messageAdapter;
    }
  }
}