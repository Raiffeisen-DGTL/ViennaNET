using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ViennaNET.CallContext;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Unit.Factories
{
  [TestFixture(Category = "Unit", TestOf = typeof(MessagingComponentFactory))]
  public class MessagingComponentFactoryTests
  {
    private Mock<IMessageAdapterFactory> _adapterFactory;
    private Mock<IMessageAdapter> _messageAdapter;
    private Mock<ICallContextFactory> _callContextFactory;
    private IConfigurationRoot _configuration;

    [OneTimeSetUp]
    public void Setup()
    {
      _adapterFactory = new Mock<IMessageAdapterFactory>();
      _messageAdapter = new Mock<IMessageAdapter>();
      _callContextFactory = new Mock<ICallContextFactory>();

      _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true)
                                                 .Build();

      _adapterFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<bool>()))
                     .Returns(_messageAdapter.Object);
    }

    [Test]
    public void CreateMessageSender_CorrectQueueId_SenderCreated()
    {
      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    new IMessageDeserializer[0], _callContextFactory.Object);
      var result = messagingComponentFactory.CreateMessageSender("ReValue");

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateMessageSender_QueueIdIsNull_Exception()
    {
      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    new IMessageDeserializer[0], _callContextFactory.Object);

      Assert.Throws<ArgumentNullException>(() => messagingComponentFactory.CreateMessageSender(null));
    }

    [Test]
    public void CreateMessageReceiver_HasDeserializer_ReceiverCreated()
    {
      var deserializers = new List<IMessageDeserializer> { new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    deserializers, _callContextFactory.Object);
      var result = messagingComponentFactory.CreateMessageReceiver<string>("ReValue");
      
      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateMessageReceiver_NoDeserializer_Exception()
    {
      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    new IMessageDeserializer[0], _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageReceiver<string>("ReValue"),
                                        "There are no message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageReceiver_TwoDeserializers_Exception()
    {
      var deserializers = new List<IMessageDeserializer> { new PlainTextSerializer(), new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    deserializers, _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageReceiver<string>("ReValue"),
                                        "There are too many message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateTransactedMessageReceiver_HasDeserializer_ReceiverCreated()
    {
      var adapterFactory = new Mock<IMessageAdapterFactory>();
      var messageAdapter = new Mock<IMessageAdapterWithTransactions>();

      adapterFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<bool>()))
                     .Returns(messageAdapter.Object);

      var deserializers = new List<IMessageDeserializer> { new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, adapterFactory.Object, new IMessageSerializer[0],
                                                                    deserializers, _callContextFactory.Object);

      var result = messagingComponentFactory.CreateTransactedMessageReceiver<object>("ReValue");

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateTransactedMessageReceiver_NoDeserializer_Exception()
    {
      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    new IMessageDeserializer[0], _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateTransactedMessageReceiver<string>("ReValue"),
                                        "There are no message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateTransactedMessageReceiver_TwoDeserializers_Exception()
    {
      var deserializers = new List<IMessageDeserializer> { new PlainTextSerializer(), new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    deserializers, _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateTransactedMessageReceiver<string>("ReValue"),
                                        "There are too many message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageSender_HasSerializer_SenderCreated()
    {
      var serializers = new List<IMessageSerializer> { new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, serializers,
                                                                    new IMessageDeserializer[0], _callContextFactory.Object);
      var result = messagingComponentFactory.CreateMessageSender<string>("ReValue");

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateMessageSender_NoSerializer_Exception()
    {
      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    new IMessageDeserializer[0], _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageSender<string>("ReValue"),
                                        "There are no message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageSender_TwoSerializers_Exception()
    {
      var serializers = new List<IMessageSerializer> { new PlainTextSerializer(), new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, serializers,
                                                                    new IMessageDeserializer[0], _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageSender<string>("ReValue"),
                                        "There are too many message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_HasSerializerAndDeserializer_SenderCreated()
    {
      var serializers = new List<IMessageSerializer> { new PlainTextSerializer() };
      var deserializers = new List<IMessageDeserializer> { new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object,
                                                                    serializers, deserializers, _callContextFactory.Object);
      var result = messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue");

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateMessageRpcSender_NoSerializerAndDeserializer_Exception()
    {
      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    new IMessageDeserializer[0], _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
                                        "There are no message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_NoSerializer_Exception()
    {
      var deserializers = new List<IMessageDeserializer> { new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, new IMessageSerializer[0],
                                                                    deserializers, _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
                                        "There are no message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_TwoSerializers_Exception()
    {
      var serializers = new List<IMessageSerializer> { new PlainTextSerializer(), new PlainTextSerializer() };
      var deserializers = new List<IMessageDeserializer> { new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object,
                                                                    serializers, deserializers, _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
                                        "There are too many message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_NoDeserializer_Exception()
    {
      var serializers = new List<IMessageSerializer> { new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object, serializers,
                                                                    new IMessageDeserializer[0], _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
                                        "There are no message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_TwoDeserializers_Exception()
    {
      var serializers = new List<IMessageSerializer> { new PlainTextSerializer() };
      var deserializers = new List<IMessageDeserializer> { new PlainTextSerializer(), new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object,
                                                                    serializers, deserializers, _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
                                        "There are too many message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_TwoSerializersAndDeserializers_Exception()
    {
      var serializers = new List<IMessageSerializer> { new PlainTextSerializer(), new PlainTextSerializer() };
      var deserializers = new List<IMessageDeserializer> { new PlainTextSerializer(), new PlainTextSerializer() };

      var messagingComponentFactory = new MessagingComponentFactory(_configuration, _adapterFactory.Object,
                                                                    serializers, deserializers, _callContextFactory.Object);

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
                                        "There are too many message serializers with the message type System.String for the queue id: ReValue");
    }
  }
}