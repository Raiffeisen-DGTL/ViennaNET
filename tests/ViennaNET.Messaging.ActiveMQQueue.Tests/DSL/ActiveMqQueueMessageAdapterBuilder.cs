using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
    internal class ActiveMqQueueMessageAdapterBuilder
  {
    private ActiveMqQueueConfiguration _configuration;
    private IActiveMqConnectionFactory _connectionFactoryProvider;

    public ActiveMqQueueMessageAdapterBuilder WithConfiguration(ActiveMqQueueConfiguration configuration)
    {
      _configuration = configuration;
      return this;
    }

    public ActiveMqQueueMessageAdapterBuilder WithConnectionFactoryProvider(IActiveMqConnectionFactory provider)
    {
      _connectionFactoryProvider = provider;
      return this;
    }

    public ActiveMqQueueMessageAdapterBuilder ConfigureConnectionFactoryProvider(
      Func<ActiveMqConnectionFactoryBuilder, IActiveMqConnectionFactory> configurator)
    {
      _connectionFactoryProvider = configurator(Given.ConnectionFactoryProvider);
      return this;
    }

    public ActiveMqQueueMessageAdapter Please()
    {
      return new(
        _connectionFactoryProvider ?? Given.ConnectionFactoryProvider.Please(),
        _configuration ?? new ActiveMqQueueConfiguration { ProcessingType = MessageProcessingType.ThreadStrategy },
        Mock.Of<ILogger<ActiveMqQueueMessageAdapter>>());
    }
  }
}