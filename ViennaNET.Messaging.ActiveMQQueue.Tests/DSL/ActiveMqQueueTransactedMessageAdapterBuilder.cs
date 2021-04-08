using System;
using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Messaging.Configuration;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
  internal class ActiveMqQueueTransactedMessageAdapterBuilder
  {
    private ActiveMqQueueConfiguration _configuration;
    private IActiveMqConnectionFactory _connectionFactoryProvider;
    private bool _transacted;

    public ActiveMqQueueTransactedMessageAdapterBuilder WithConfiguration(ActiveMqQueueConfiguration configuration)
    {
      _configuration = configuration;
      return this;
    }

    public ActiveMqQueueTransactedMessageAdapterBuilder Transacted()
    {
      _transacted = true;
      return this;
    }

    public ActiveMqQueueTransactedMessageAdapterBuilder WithConnectionFactoryProvider(
      IActiveMqConnectionFactory provider)
    {
      _connectionFactoryProvider = provider;
      return this;
    }

    public ActiveMqQueueTransactedMessageAdapterBuilder ConfigureConnectionFactoryProvider(
      Func<ActiveMqConnectionFactoryBuilder, IActiveMqConnectionFactory> configurator)
    {
      _connectionFactoryProvider = configurator(Given.ConnectionFactoryProvider);
      return this;
    }

    public ActiveMqQueueTransactedMessageAdapter Please()
    {
      return new ActiveMqQueueTransactedMessageAdapter(
        _connectionFactoryProvider ?? Given.ConnectionFactoryProvider.Please(),
        _configuration ?? new ActiveMqQueueConfiguration
        {
          ProcessingType = MessageProcessingType.ThreadStrategy,
          TransactionEnabled = _transacted
        },
        Mock.Of<ILogger<ActiveMqQueueTransactedMessageAdapter>>());
    }
  }
}