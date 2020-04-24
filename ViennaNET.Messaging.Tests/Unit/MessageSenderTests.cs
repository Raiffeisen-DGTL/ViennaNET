using Moq;
using NUnit.Framework;
using ViennaNET.CallContext;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Sending.Impl;

namespace ViennaNET.Messaging.Tests.Unit
{
  [TestFixture(Category = "Unit", TestOf = typeof(MessageSender))]
  public class MessageSenderTests
  {
    private Mock<IMessageAdapter> _adapter;
    private Mock<ICallContextFactory> _callContextFactory;
    private Mock<QueueConfigurationBase> _queueConfiguration;
    private MessagingConfiguration _messagingConfig;

    [OneTimeSetUp]
    public void Setup()
    {
      _messagingConfig = new MessagingConfiguration();
      _adapter = new Mock<IMessageAdapter>();
      _callContextFactory = new Mock<ICallContextFactory>();
      _callContextFactory.Setup(x => x.Create())
                         .Returns(new EmptyCallContext());

      _queueConfiguration = new Mock<QueueConfigurationBase>();
    }

    [Test]
    public void DisposeTest()
    {
      var messageSender = new MessageSender(_adapter.Object, _callContextFactory.Object, "ReApplication");
      messageSender.Dispose();
      _adapter.Verify(x => x.Dispose(), Times.AtLeastOnce);
    }

    [Test]
    public void SendMessageTest()
    {
      var messageId = "ReMessageId";

      _messagingConfig.ApplicationName = "ReApplication";
      _adapter.Setup(x => x.Send(It.IsAny<BaseMessage>()))
              .Returns(new TextMessage { MessageId = messageId });
      _adapter.Setup(x => x.Configuration)
              .Returns(_queueConfiguration.Object);

      var messageSender = new MessageSender(_adapter.Object, _callContextFactory.Object, "ReApplication");
      var result = messageSender.SendMessage(new TextMessage());

      Assert.Multiple(() =>
      {
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(messageId));
      });
    }
  }
}
