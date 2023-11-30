using System;
using Moq;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Receiving.Impl;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.DSL
{
  internal class MessageReceiverBuilder
  {
    private const string CorrectMessage =
      @"<?xml version=""1.0""?>
                                  <TextMessage xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                                    <SendDateTime xsi:nil=""true"" />
                                    <ReceiveDate xsi:nil=""true"" />
                                    <LifeTime>PT0S</LifeTime>
                                    <Body>{0}</Body>
                                  </TextMessage>";

    private bool _isConnected;
    private string _messageText = string.Format(CorrectMessage, string.Empty);

    public Mock<IMessageAdapter>? MessageAdapter { get; private set; }

    public MessageReceiverBuilder Connected()
    {
      _isConnected = true;
      return this;
    }

    public MessageReceiverBuilder WithMessageBody(string text)
    {
      _messageText = string.Format(CorrectMessage, text);
      return this;
    }

    public MessageReceiver<T> Please<T>() where T : BaseMessage
    {
      BaseMessage message = new TextMessage { Body = _messageText };
      BaseMessage emptyMessage = new TextMessage();

      var configuration = new Mock<QueueConfigurationBase>();
      configuration.SetupAllProperties();

      MessageAdapter = new Mock<IMessageAdapter>();
      MessageAdapter.Setup(x => x.Receive(It.IsAny<string>(), It.IsAny<TimeSpan?>(), It.IsAny<(string, string)[]>()))
        .Returns((Func<string, TimeSpan?, (string, string)[], BaseMessage>)((x, y, z) => x == "corrId"
          ? emptyMessage
          : message));
      MessageAdapter.SetupGet(x => x.IsConnected)
        .Returns(_isConnected);
      MessageAdapter.Setup(x => x.TryReceive(out message, null, null))
        .Returns(true);
      MessageAdapter.Setup(x => x.TryReceive(out emptyMessage, "corrId", null))
        .Returns(true);
      MessageAdapter.Setup(x => x.TryReceive(out emptyMessage, "falseId", null))
        .Returns(false);
      MessageAdapter.SetupGet(x => x.Configuration)
        .Returns(configuration.Object);

      var deserializer = new XmlMessageSerializer<T>();
      return new MessageReceiver<T>(MessageAdapter.Object, deserializer);
    }
  }
}