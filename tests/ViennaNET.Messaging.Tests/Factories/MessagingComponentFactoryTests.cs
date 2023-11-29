using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Messaging.Tests.DSL;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Factories
{
  [TestFixture(Category = "Unit", TestOf = typeof(MessagingComponentFactory))]
  public class MessagingComponentFactoryTests
  {
    [OneTimeSetUp]
    public void Setup()
    {
      using var configStream = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream("ViennaNET.Messaging.Tests.appsettings.json");
      _configuration = new ConfigurationBuilder()
        .AddJsonStream(configStream)
        .Build();
    }

    private IConfigurationRoot _configuration;

    [Test]
    public void CreateMessageReceiver_HasDeserializer_ReceiverCreated()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithDeserializer(new PlainTextSerializer())
        .Please();

      var result = messagingComponentFactory.CreateMessageReceiver<string>("ReValue");

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateMessageReceiver_NoDeserializer_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory.Please();

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageReceiver<string>("ReValue"),
        "There are no message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageReceiver_TwoDeserializers_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithDeserializer(new PlainTextSerializer())
        .WithDeserializer(new PlainTextSerializer())
        .Please();

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageReceiver<string>("ReValue"),
        "There are too many message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_HasSerializerAndDeserializer_SenderCreated()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithSerializer(new PlainTextSerializer())
        .WithDeserializer(new PlainTextSerializer())
        .Please();

      var result = messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue");

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateMessageRpcSender_NoDeserializer_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithSerializer(new PlainTextSerializer())
        .Please();

      Assert.Throws<MessagingException>(
        () => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
        "There are no message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_NoSerializer_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithDeserializer(new PlainTextSerializer())
        .Please();

      Assert.Throws<MessagingException>(
        () => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
        "There are no message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_NoSerializerAndDeserializer_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory.WithConfiguration(_configuration).Please();

      Assert.Throws<MessagingException>(
        () => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
        "There are no message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_TwoDeserializers_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithSerializer(new PlainTextSerializer())
        .WithDeserializer(new PlainTextSerializer())
        .WithDeserializer(new PlainTextSerializer())
        .Please();

      Assert.Throws<MessagingException>(
        () => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
        "There are too many message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_TwoSerializers_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithSerializer(new PlainTextSerializer())
        .WithSerializer(new PlainTextSerializer())
        .WithDeserializer(new PlainTextSerializer())
        .Please();

      Assert.Throws<MessagingException>(
        () => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
        "There are too many message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageRpcSender_TwoSerializersAndDeserializers_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithSerializer(new PlainTextSerializer())
        .WithSerializer(new PlainTextSerializer())
        .WithDeserializer(new PlainTextSerializer())
        .WithDeserializer(new PlainTextSerializer())
        .Please();

      Assert.Throws<MessagingException>(
        () => messagingComponentFactory.CreateMessageRpcSender<string, string>("ReValue"),
        "There are too many message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageSender_CorrectQueueId_SenderCreated()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory.WithConfiguration(_configuration).Please();

      var result = messagingComponentFactory.CreateMessageSender("ReValue");

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateMessageSender_HasSerializer_SenderCreated()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithSerializer(new PlainTextSerializer())
        .Please();

      var result = messagingComponentFactory.CreateMessageSender<string>("ReValue");

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateMessageSender_NoSerializer_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory.WithConfiguration(_configuration).Please();

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageSender<string>("ReValue"),
        "There are no message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateMessageSender_QueueIdIsNull_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory.WithConfiguration(_configuration).Please();

      Assert.Throws<ArgumentNullException>(() => messagingComponentFactory.CreateMessageSender(null));
    }

    [Test]
    public void CreateMessageSender_TwoSerializers_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithSerializer(new PlainTextSerializer())
        .WithSerializer(new PlainTextSerializer())
        .Please();

      Assert.Throws<MessagingException>(() => messagingComponentFactory.CreateMessageSender<string>("ReValue"),
        "There are too many message serializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateTransactedMessageReceiver_HasDeserializer_ReceiverCreated()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithDeserializer(new PlainTextSerializer())
        .ReturnsTransacted()
        .Please();

      var result = messagingComponentFactory.CreateTransactedMessageReceiver<object>("ReValue");

      Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateTransactedMessageReceiver_NoDeserializer_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory.WithConfiguration(_configuration).Please();

      Assert.Throws<MessagingException>(
        () => messagingComponentFactory.CreateTransactedMessageReceiver<string>("ReValue"),
        "There are no message deserializers with the message type System.String for the queue id: ReValue");
    }

    [Test]
    public void CreateTransactedMessageReceiver_TwoDeserializers_Exception()
    {
      var messagingComponentFactory = Given.MessagingComponentFactory
        .WithConfiguration(_configuration)
        .WithDeserializer(new PlainTextSerializer())
        .WithDeserializer(new PlainTextSerializer())
        .Please();

      Assert.Throws<MessagingException>(
        () => messagingComponentFactory.CreateTransactedMessageReceiver<string>("ReValue"),
        "There are too many message deserializers with the message type System.String for the queue id: ReValue");
    }
  }
}