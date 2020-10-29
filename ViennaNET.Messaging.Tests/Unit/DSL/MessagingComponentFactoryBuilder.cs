using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using ViennaNET.CallContext;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.Factories.Impl;
using ViennaNET.Messaging.Tools;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class MessagingComponentFactoryBuilder
  {
    private readonly LinkedList<IMessageSerializer> _serializers = new LinkedList<IMessageSerializer>();
    private readonly LinkedList<IMessageDeserializer> _deserializers = new LinkedList<IMessageDeserializer>();
    
    private IConfiguration _configuration;
    private bool _createTransacted;

    public MessagingComponentFactoryBuilder WithConfiguration(IConfiguration configuration)
    {
      _configuration = configuration;
      return this;
    }

    public MessagingComponentFactoryBuilder WithDeserializer(IMessageDeserializer deserializer)
    {
      _deserializers.AddLast(deserializer);
      return this;
    }
        
    public MessagingComponentFactoryBuilder WithSerializer(IMessageSerializer serializer)
    {
      _serializers.AddLast(serializer);
      return this;
    }

    public MessagingComponentFactoryBuilder ReturnsTransacted()
    {
      _createTransacted = true;
      return this;
    }

    public MessagingComponentFactory Please()
    {
      var messageAdapterFactoryMock = new Mock<IMessageAdapterFactory> {DefaultValue = DefaultValue.Mock};
      if (_createTransacted)
      {
        messageAdapterFactoryMock
          .Setup(x => x.Create(It.IsAny<string>()))
          .Returns(Mock.Of<IMessageAdapterWithTransactions>());
      }

      return new MessagingComponentFactory(
        _configuration ?? new ConfigurationBuilder().Build(),
        messageAdapterFactoryMock.Object,
        _serializers,
        _deserializers,
        Mock.Of<ICallContextFactory>()
        );
    }
  }
}
