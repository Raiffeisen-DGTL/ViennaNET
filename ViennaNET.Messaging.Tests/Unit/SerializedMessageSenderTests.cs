using Moq;
using NUnit.Framework;
using ViennaNET.CallContext;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.Sending.Impl;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Unit
{
  [TestFixture(Category = "Unit", TestOf = typeof(SerializedMessageSender<>))]
  public class SerializedMessageSenderTests
  {
    [SetUp]
    public void Setup()
    {
      _messageSerializer = new Mock<IMessageSerializer<object>>();
      _messageAdapter = new Mock<IMessageAdapter>();
      _callContextFactory = new Mock<ICallContextFactory>();
      _callContextFactory.Setup(x => x.Create())
                         .Returns(new EmptyCallContext());
    }

    private Mock<IMessageSerializer<object>> _messageSerializer;
    private Mock<IMessageAdapter> _messageAdapter;
    private Mock<ICallContextFactory> _callContextFactory;

    [Test]
    public void SendMessageTest()
    {
      var messageId = "MessageId";

      var configuration = new MqSeriesQueueConfiguration { IntervalPollingQueue = 1000 };
      _messageSerializer.Setup(x => x.Serialize(It.IsAny<object>()))
                        .Returns(new TextMessage());
      _messageAdapter.Setup(x => x.Configuration)
                     .Returns(configuration);

      _messageAdapter.Setup(x => x.Connect());
      _messageAdapter.Setup(x => x.Send(It.IsAny<BaseMessage>()))
                     .Returns(new TextMessage { MessageId = messageId });

      var serializedMessageSender =
        new SerializedMessageSender<object>(_messageSerializer.Object, _messageAdapter.Object, _callContextFactory.Object, "");
      var result = serializedMessageSender.SendMessage(new TextMessage());

      Assert.That(result, Is.EqualTo(messageId));
    }
  }
}