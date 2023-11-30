using System;
using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests.DSL
{
  internal class MqSeriesQueueSubscribingMessageAdapterBuilder
  {
    private MqSeriesQueueConfiguration _configuration;
    private IMqSeriesQueueConnectionFactoryProvider _connectionFactoryProvider;

    public MqSeriesQueueSubscribingMessageAdapterBuilder WithConfiguration(MqSeriesQueueConfiguration configuration)
    {
      _configuration = configuration;
      return this;
    }

    public MqSeriesQueueSubscribingMessageAdapterBuilder WithConnectionFactoryProvider(
      IMqSeriesQueueConnectionFactoryProvider provider)
    {
      _connectionFactoryProvider = provider;
      return this;
    }

    public MqSeriesQueueSubscribingMessageAdapterBuilder ConfigureConnectionFactoryProvider(
      Func<MqSeriesConnectionFactoryProviderBuilder, IMqSeriesQueueConnectionFactoryProvider> configurator)
    {
      _connectionFactoryProvider = configurator(Given.ConnectionFactoryProvider);
      return this;
    }

    public MqSeriesQueueSubscribingMessageAdapter Please()
    {
      return new(
        _connectionFactoryProvider ?? Given.ConnectionFactoryProvider.Please(),
        _configuration ?? new MqSeriesQueueConfiguration { ProcessingType = MessageProcessingType.Subscribe },
        Mock.Of<ILogger<MqSeriesQueueSubscribingMessageAdapter>>());
    }
  }
}