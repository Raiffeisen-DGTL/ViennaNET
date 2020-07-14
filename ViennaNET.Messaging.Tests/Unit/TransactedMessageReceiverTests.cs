using System;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Receiving.Impl;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Unit
{
  [TestFixture(Category = "Unit", TestOf = typeof(MessageReceiver<>))]
  public class TransactedMessageReceiverTests
  {
    private Mock<IMessageAdapterWithTransactions> _adapter;
    private Mock<IMessageDeserializer<object>> _deserializer;

    [OneTimeSetUp]
    public void Setup()
    {
      _adapter = new Mock<IMessageAdapterWithTransactions>();
      _adapter.Setup(x => x.Receive(It.IsAny<string>(), It.IsAny<TimeSpan?>()))
              .Returns(new TextMessage { Body = "E" });
      _deserializer = new Mock<IMessageDeserializer<object>>();
    }

    [Test]
    public void CommitIfTransactedTest()
    {
      var messageReceiver = new TransactedMessageReceiver<object>(_adapter.Object, _deserializer.Object);
      messageReceiver.CommitIfTransacted();
      _adapter.Verify(x => x.CommitIfTransacted(null));
    }

    [Test]
    public void RollbackIfTransactedTest()
    {
      var messageReceiver = new TransactedMessageReceiver<object>(_adapter.Object, _deserializer.Object);
      messageReceiver.RollbackIfTransacted();
      _adapter.Verify(x => x.RollbackIfTransacted());
    }
  }
}
