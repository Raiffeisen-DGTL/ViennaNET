using Moq;
using ViennaNET.Messaging.Configuration;
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

    public IMessageAdapter Please()
    {
      var messageAdapter = new Mock<IMessageAdapter>();
      messageAdapter
        .Setup(x => x.Configuration)
        .Returns(_queueConfiguration ?? new MqSeriesQueueConfiguration { IntervalPollingQueue = 1000, ProcessingType = MessageProcessingType.ThreadStrategy });
      messageAdapter
        .Setup(x => x.SupportProcessingType(It.Is<MessageProcessingType>(pt => !_processingType.HasValue || pt == _processingType.Value)))
        .Returns(true);

      return messageAdapter.Object;
    }
  }
}
