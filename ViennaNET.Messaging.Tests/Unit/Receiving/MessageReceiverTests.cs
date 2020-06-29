using System;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Receiving.Impl;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Unit.Receiving
{
  [TestFixture, Category("Unit"), TestOf(typeof(MessageReceiver<>))]
  public class MessageReceiverTests
  {
    private const string MessageText =
      @"<?xml version=""1.0"" encoding=""utf-8""?><TextMessage xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><SendDateTime xsi:nil=""true"" /><ReceiveDate xsi:nil=""true"" /><LifeTime>PT0S</LifeTime><Body>testMessage</Body></TextMessage>";

    private const string TestMessageText = "testMessage";

    private static MessageReceiver<TextMessage> CreateMessageReceiver(out Mock<IMessageAdapter> adapter, bool adapterIsConnected = true)
    {
      BaseMessage message = new TextMessage { Body = MessageText };
      BaseMessage emptyMessage = new TextMessage();

      var deserializer = new XmlMessageSerializer<TextMessage>();
      adapter = new Mock<IMessageAdapter>();
      adapter.Setup(x => x.Receive(It.IsAny<string>(), It.IsAny<TimeSpan?>(), It.IsAny<(string, string)[]>()))
             .Returns((Func<string, TimeSpan?, (string, string)[], BaseMessage >)((x, y, z) => x == "corrId"
                        ? new TextMessage()
                        : new TextMessage { Body = MessageText }));
      adapter.SetupGet(x => x.IsConnected)
             .Returns(adapterIsConnected);

      adapter.Setup(x => x.TryReceive(out message, null, null))
             .Returns(true);
      adapter.Setup(x => x.TryReceive(out emptyMessage, "corrId", null))
             .Returns(true);
      adapter.Setup(x => x.TryReceive(out emptyMessage, "falseId", null))
             .Returns(false);

      var receiver = new MessageReceiver<TextMessage>(adapter.Object, deserializer);
      return receiver;
    }

    [Test]
    public void Dispose_AdapterExists_DisposeCalled()
    {
      var receiver = CreateMessageReceiver(out var adapter, false);

      receiver.Dispose();

      adapter.Verify(x => x.Dispose(), Times.Once);
    }

    [Test]
    public void Dispose_DisposeTwice_AdapterDisposeCalledOnce()
    {
      var receiver = CreateMessageReceiver(out var adapter, false);

      receiver.Dispose();
      receiver.Dispose();

      adapter.Verify(x => x.Dispose(), Times.Once);
    }

    [Test]
    public void Receive_AdapterNotConnected_Connect()
    {
      var receiver = CreateMessageReceiver(out var adapter, false);

      receiver.Receive();

      adapter.Verify(x => x.Connect(), Times.Once);
    }

    [Test]
    public void Receive_AdapterReturnsEmptyMessage_ExceptionThrown()
    {
      var receiver = CreateMessageReceiver(out _);

      Assert.Throws<MessagingException>(() => receiver.Receive("corrId"));
    }

    [Test]
    public void Receive_AdapterReturnsEmptyMessageAndOutMessage_ExceptionThrown()
    {
      var receiver = CreateMessageReceiver(out _);

      Assert.Throws<MessagingException>(() => receiver.Receive("corrId", out _));
    }

    [Test]
    public void Receive_AdapterReturnsMessage_CorrectBaseMessageReturned()
    {
      var receiver = CreateMessageReceiver(out _);

      var result = receiver.Receive(out var baseMessage);

      Assert.Multiple(() =>
      {
        Assert.That(baseMessage is TextMessage textMessage && textMessage.Body == MessageText);
        Assert.That(result.Body == TestMessageText);
      });
    }

    [Test]
    public void Receive_AdapterReturnsMessage_MessageDeserialized()
    {
      var receiver = CreateMessageReceiver(out _);

      var result = receiver.Receive();

      Assert.That(result.Body == TestMessageText);
    }

    [Test]
    public void Receive_Disposed_Throws()
    {
      var receiver = CreateMessageReceiver(out _, false);
      receiver.Dispose();

      Assert.Throws<ObjectDisposedException>(() => receiver.Receive(out _));
    }

    [Test]
    public void TryReceive_AdapterNotConnected_Connect()
    {
      var receiver = CreateMessageReceiver(out var adapter, false);

      receiver.TryReceive(out _);

      adapter.Verify(x => x.Connect(), Times.Once);
    }

    [Test]
    public void TryReceive_AdapterReturnsEmptyMessage_ExceptionThrown()
    {
      var receiver = CreateMessageReceiver(out _);

      Assert.Throws<MessagingException>(() => receiver.TryReceive("corrId", out _));
    }

    [Test]
    public void TryReceive_AdapterReturnsEmptyMessageAndOutMessage_ExceptionThrown()
    {
      var receiver = CreateMessageReceiver(out _);

      Assert.Throws<MessagingException>(() => receiver.TryReceive("corrId", out _, out _));
    }

    [Test]
    public void TryReceive_AdapterReturnsFalse_MessagesAreNull()
    {
      var receiver = CreateMessageReceiver(out _);

      var result = receiver.TryReceive("falseId", out var textMessage, out var baseMessage);

      Assert.Multiple(() =>
      {
        Assert.Null(textMessage);
        Assert.Null(baseMessage);
        Assert.False(result);
      });
    }

    [Test]
    public void TryReceive_AdapterReturnsMessage_CorrectBaseMessageReturned()
    {
      var receiver = CreateMessageReceiver(out _);

      var result = receiver.TryReceive(out var textMessage, out var baseMessage);

      Assert.Multiple(() =>
      {
        Assert.That(baseMessage is TextMessage txtMessage && txtMessage.Body == MessageText);
        Assert.That(textMessage.Body == TestMessageText);
        Assert.True(result);
      });
    }

    [Test]
    public void TryReceive_AdapterReturnsMessage_MessageDeserialized()
    {
      var receiver = CreateMessageReceiver(out _);

      var result = receiver.TryReceive(out var textMessage);

      Assert.Multiple(() =>
      {
        Assert.That(textMessage.Body == TestMessageText);
        Assert.True(result);
      });
    }
  }
}