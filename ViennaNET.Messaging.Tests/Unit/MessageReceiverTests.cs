using System;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Receiving.Impl;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Unit
{
  [TestFixture(Category = "Unit", TestOf = typeof(MessageReceiver<>))]
  public class MessageReceiverTests
  {
    private Mock<IMessageAdapter> _adapter;
    private Mock<IMessageDeserializer<object>> _deserializer;

    [OneTimeSetUp]
    public void Setup()
    {
      _adapter = new Mock<IMessageAdapter>();
      _adapter.Setup(x => x.Receive(It.IsAny<string>(), It.IsAny<TimeSpan?>()))
              .Returns(new TextMessage { Body = "E" });
      _deserializer = new Mock<IMessageDeserializer<object>>();
    }

    [Test]
    [TestCase(null)]
    [TestCase("ReString")]
    public void ReceiveNotArgumentTest(object data)
    {
      InitReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.Receive();
      Assert.That(result, Is.EqualTo(data));
    }

    [Test]
    [TestCase(null)]
    [TestCase("ReString")]
    public void ReceiveMessageArgumentTest(object data)
    {
      InitReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.Receive(out var _);

      Assert.That(result, Is.EqualTo(data));
    }

    [Test]
    [TestCase(null)]
    [TestCase("ReString")]
    public void ReceiveStringArgumentTest(object data)
    {
      InitReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.Receive("ReCorrelationId");

      Assert.That(result, Is.EqualTo(data));
    }

    [Test]
    [TestCase(null)]
    [TestCase("ReString")]
    public void ReceiveStringMessageArgumentsTest(object data)
    {
      InitReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.Receive("ReCorrelationId", out _);
      Assert.That(result, Is.EqualTo(data));
    }

    [Test]
    [TestCase(null)]
    [TestCase("ReString")]
    public void ReceiveTimeSpanArgumentTest(object data)
    {
      InitReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.Receive();
      Assert.That(result, Is.EqualTo(data));
    }

    [Test]
    [TestCase(null)]
    [TestCase("ReString")]
    public void ReceiveStringTimeSpanArgumentsTest(object data)
    {
      InitReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.Receive("ReCorrelationId");
      Assert.That(result, Is.EqualTo(data));
    }

    [Test]
    [TestCase(null)]
    [TestCase("ReString")]
    public void ReceiveStringTimeSpanMessageArgumentsTest(object data)
    {
      InitReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.Receive("ReCorrelationId", out _);
      Assert.That(result, Is.EqualTo(data));
    }

    [Test]
    public void TryReceiveMessageArgumentTest()
    {
      var data = "ReTryReceive";
      InitTryReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.TryReceive(out var t);
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.True);
        Assert.That(t, Is.EqualTo(data));
      });
    }

    [Test]
    public void TryReceiveMessageReciveMessageArgumentsTest()
    {
      var data = "ReTryReceive";
      InitTryReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.TryReceive(out var dataMessage, out var dataReceivedMessage);
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.True);
        Assert.That(dataMessage, Is.EqualTo(data));
        Assert.That(dataReceivedMessage.LogBody(), Is.EqualTo("ReBody"));
      });
    }

    [Test]
    public void TryReceiveCorrelationIdMessageArgumentsTest()
    {
      var data = "ReTryReceive";
      InitTryReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.TryReceive("ReCorrelationId", out var dataMessage);
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.True);
        Assert.That(dataMessage, Is.EqualTo(data));
      });
    }

    [Test]
    public void TryReceiveCorrelationIdMessageReciveMessageArgumentsTest()
    {
      var data = "ReTryReceive";
      InitTryReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.TryReceive("ReCorrelationId", out var dataMessage, out _);
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.True);
        Assert.That(dataMessage, Is.EqualTo(data));
      });
    }

    [Test]
    public void TryReceiveTimeoutMessageArgumentsTest()
    {
      var data = "ReTryReceive";
      InitTryReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.TryReceive(out var dataMessage);
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.True);
        Assert.That(dataMessage, Is.EqualTo(data));
      });
    }

    [Test]
    public void TryReceiveTimeoutMessageReceivedMessageArgumentsTest()
    {
      var data = "ReTryReceive";
      InitTryReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.TryReceive(out var dataMessage, out _);

      Assert.Multiple(() =>
      {
        Assert.That(result, Is.True);
        Assert.That(dataMessage, Is.EqualTo(data));
      });
    }

    [Test]
    public void TryReceiveCorrelationIdTimeoutMessageArgumentsTest()
    {
      var data = "ReTryReceive";
      InitTryReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.TryReceive("ReCorrelationId", out var dataMessage);

      Assert.Multiple(() =>
      {
        Assert.That(result, Is.True);
        Assert.That(dataMessage, Is.EqualTo(data));
      });
    }

    [Test]
    public void TryReceiveCorrelationIdTimeoutMessageReceivedMessageArgumentsTest()
    {
      var data = "ReTryReceive";
      InitTryReceive(data);
      var messageReceiver = new MessageReceiver<object>(_adapter.Object, _deserializer.Object);
      var result = messageReceiver.TryReceive("ReCorrelationId", out var dataMessage, out _);

      Assert.Multiple(() =>
      {
        Assert.That(result, Is.True);
        Assert.That(dataMessage, Is.EqualTo(data));
      });
    }

    private void InitReceive(object data)
    {
      _deserializer.Setup(x => x.Deserialize(It.IsAny<BaseMessage>()))
                   .Returns(data);
      _adapter.Setup(x => x.Receive(It.IsAny<string>(), It.IsAny<TimeSpan?>()))
              .Returns(new TextMessage { Body = "ReBody" });
    }

    private void InitTryReceive(string data)
    {
      _deserializer.Setup(x => x.Deserialize(It.IsAny<BaseMessage>()))
                   .Returns(data);

      BaseMessage dataMessage = new TextMessage { Body = "ReBody" };
      _adapter.Setup(x => x.TryReceive(out dataMessage, It.IsAny<string>(), It.IsAny<TimeSpan?>()))
              .Returns(true);
    }
  }
}