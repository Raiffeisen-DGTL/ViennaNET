﻿using System;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Receiving.Impl;
using ViennaNET.Messaging.Tests.DSL;

namespace ViennaNET.Messaging.Tests.Receiving
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(MessageReceiver<>))]
  public class MessageReceiverTests
  {
    [Test]
    public void Dispose_AdapterExists_DisposeCalled()
    {
      var receiverBuilder = Given.MessageReceiver;
      var receiver = receiverBuilder.Please<TextMessage>();

      receiver.Dispose();

      receiverBuilder.MessageAdapter.Verify(x => x.Dispose(), Times.Once);
    }

    [Test]
    public void Dispose_DisposeTwice_AdapterDisposeCalledOnce()
    {
      var receiverBuilder = Given.MessageReceiver;
      var receiver = receiverBuilder.Please<TextMessage>();

      receiver.Dispose();
      receiver.Dispose();

      receiverBuilder.MessageAdapter.Verify(x => x.Dispose(), Times.Once);
    }

    [Test]
    public void Receive_AdapterNotConnected_Connect()
    {
      var receiverBuilder = Given.MessageReceiver;
      var receiver = receiverBuilder.Please<TextMessage>();

      receiver.Receive();

      receiverBuilder.MessageAdapter.Verify(x => x.Connect(), Times.Once);
    }

    [Test]
    public void Receive_AdapterReturnsEmptyMessage_ExceptionThrown()
    {
      var receiver = Given.MessageReceiver.Please<TextMessage>();

      Assert.Throws<MessagingException>(() => receiver.Receive("corrId"));
    }

    [Test]
    public void Receive_AdapterReturnsEmptyMessageAndOutMessage_ExceptionThrown()
    {
      var receiver = Given.MessageReceiver.Please<TextMessage>();

      Assert.Throws<MessagingException>(() => receiver.Receive("corrId", out _));
    }

    [Test]
    public void Receive_AdapterReturnsMessage_CorrectBaseMessageReturned()
    {
      var messageBody = "message text";
      var receiver = Given
        .MessageReceiver
        .WithMessageBody(messageBody)
        .Please<TextMessage>();

      var result = receiver.Receive(out var baseMessage);

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<TextMessage>(baseMessage);
        Assert.AreEqual(messageBody, result.Body);
      });
    }

    [Test]
    public void Receive_AdapterReturnsMessage_MessageDeserialized()
    {
      var messageBody = "message text";
      var receiver = Given
        .MessageReceiver
        .WithMessageBody(messageBody)
        .Please<TextMessage>();

      var result = receiver.Receive();

      Assert.AreEqual(messageBody, result.Body);
    }

    [Test]
    public void Receive_Disposed_Throws()
    {
      var receiver = Given.MessageReceiver.Please<TextMessage>();
      receiver.Dispose();

      Assert.Throws<ObjectDisposedException>(() => receiver.Receive(out _));
    }

    [Test]
    public void TryReceive_AdapterNotConnected_Connect()
    {
      var receiverBuilder = Given.MessageReceiver;
      var receiver = receiverBuilder.Please<TextMessage>();

      receiver.TryReceive(out _);

      receiverBuilder.MessageAdapter.Verify(x => x.Connect(), Times.Once);
    }

    [Test]
    public void TryReceive_AdapterReturnsEmptyMessage_ExceptionThrown()
    {
      var receiver = Given.MessageReceiver.Please<TextMessage>();

      Assert.Throws<MessagingException>(() => receiver.TryReceive("corrId", out _));
    }

    [Test]
    public void TryReceive_AdapterReturnsEmptyMessageAndOutMessage_ExceptionThrown()
    {
      var receiver = Given.MessageReceiver.Please<TextMessage>();

      Assert.Throws<MessagingException>(() => receiver.TryReceive("corrId", out _, out _));
    }

    [Test]
    public void TryReceive_AdapterReturnsFalse_MessagesAreNull()
    {
      var receiver = Given.MessageReceiver.Please<TextMessage>();

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
      var messageBody = "message text";
      var receiver = Given
        .MessageReceiver
        .WithMessageBody(messageBody)
        .Please<TextMessage>();

      var result = receiver.TryReceive(out var textMessage, out var baseMessage);

      Assert.Multiple(() =>
      {
        Assert.IsInstanceOf<TextMessage>(baseMessage);
        Assert.AreEqual(messageBody, textMessage.Body);
        Assert.True(result);
      });
    }

    [Test]
    public void TryReceive_AdapterReturnsMessage_MessageDeserialized()
    {
      var messageBody = "message text";
      var receiver = Given
        .MessageReceiver
        .WithMessageBody(messageBody)
        .Please<TextMessage>();

      var result = receiver.TryReceive(out var textMessage);

      Assert.Multiple(() =>
      {
        Assert.AreEqual(messageBody, textMessage.Body);
        Assert.True(result);
      });
    }
  }
}