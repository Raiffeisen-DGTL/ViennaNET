using System;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.MQSeriesQueue;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries.DSL
{
  internal class MqSeriesQueueTransactedMessageAdapterBuilder
  {
    private MqSeriesQueueConfiguration _configuration;
    private IMqSeriesQueueConnectionFactoryProvider _connectionFactoryProvider;
    private bool _transacted = false;

    public MqSeriesQueueTransactedMessageAdapterBuilder WithConfiguration(MqSeriesQueueConfiguration configuration)
    {
      _configuration = configuration;
      return this;
    }

    public MqSeriesQueueTransactedMessageAdapterBuilder Transacted()
    {
      _transacted = true;
      return this;
    }

    public MqSeriesQueueTransactedMessageAdapterBuilder WithConnectionFactoryProvider(IMqSeriesQueueConnectionFactoryProvider provider)
    {
      _connectionFactoryProvider = provider;
      return this;
    }

    public MqSeriesQueueTransactedMessageAdapterBuilder ConfigureConnectionFactoryProvider(
      Func<MqSeriesConnectionFactoryProviderBuilder, IMqSeriesQueueConnectionFactoryProvider> configurator)
    {
      _connectionFactoryProvider = configurator(Given.ConnectionFactoryProvider);
      return this;
    }

    public MqSeriesQueueTransactedMessageAdapter Please()
    {
      return new MqSeriesQueueTransactedMessageAdapter(
        _connectionFactoryProvider ?? Given.ConnectionFactoryProvider.Please(),
        _configuration ?? new MqSeriesQueueConfiguration { ProcessingType = MessageProcessingType.ThreadStrategy, TransactionEnabled = _transacted });
    }
  }
}
