﻿using System;
using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests.DSL
{
  internal class MqSeriesQueueTransactedMessageAdapterBuilder
  {
    private MqSeriesQueueConfiguration _configuration;
    private IMqSeriesQueueConnectionFactoryProvider _connectionFactoryProvider;
    private bool _transacted;

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

    public MqSeriesQueueTransactedMessageAdapterBuilder WithConnectionFactoryProvider(
      IMqSeriesQueueConnectionFactoryProvider provider)
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
      return new(
        _connectionFactoryProvider ?? Given.ConnectionFactoryProvider.Please(),
        _configuration ?? new MqSeriesQueueConfiguration
        {
          ProcessingType = MessageProcessingType.ThreadStrategy, TransactionEnabled = _transacted
        },
        Mock.Of<ILogger<MqSeriesQueueTransactedMessageAdapter>>());
    }
  }
}