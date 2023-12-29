using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
    internal class ActiveMqQueueSubscribingMessageAdapterBuilder
  {
    private ActiveMqQueueConfiguration _configuration;
    private IActiveMqConnectionFactory _connectionFactoryProvider;

    public ActiveMqQueueSubscribingMessageAdapterBuilder WithConfiguration(ActiveMqQueueConfiguration configuration)
    {
      _configuration = configuration;
      return this;
    }

    public ActiveMqQueueSubscribingMessageAdapterBuilder WithConnectionFactoryProvider(
      IActiveMqConnectionFactory provider)
    {
      _connectionFactoryProvider = provider;
      return this;
    }

    public ActiveMqQueueSubscribingMessageAdapterBuilder ConfigureConnectionFactoryProvider(
      Func<ActiveMqConnectionFactoryBuilder, IActiveMqConnectionFactory> configurator)
    {
      _connectionFactoryProvider = configurator(Given.ConnectionFactoryProvider);
      return this;
    }

    public ActiveMqQueueSubscribingMessageAdapter Please()
    {
      return new(
        _connectionFactoryProvider ?? Given.ConnectionFactoryProvider.Please(),
        _configuration ?? new ActiveMqQueueConfiguration { ProcessingType = MessageProcessingType.Subscribe },
        Mock.Of<ILogger<ActiveMqQueueSubscribingMessageAdapter>>());
    }
  }
}