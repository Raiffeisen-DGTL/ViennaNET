﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.MQSeriesQueue
{
  /// <summary>
  ///   Адаптер, реализующий взаимодействие с очередью IBM MQ в режиме подписки
  /// </summary>
  /// <remarks>Не поддерживает транзакции</remarks>
  internal sealed class MqSeriesQueueSubscribingMessageAdapter : MqSeriesQueueMessageAdapter,
    IMessageAdapterWithSubscribing
  {
    /// <inheritdoc />
    internal MqSeriesQueueSubscribingMessageAdapter(
      IMqSeriesQueueConnectionFactoryProvider connectionFactoryProvider,
      MqSeriesQueueConfiguration configuration,
      ILogger<MqSeriesQueueSubscribingMessageAdapter> logger)
      : base(connectionFactoryProvider, configuration, logger)
    {
    }

    /// <inheritdoc />
    [Obsolete("Метод должен быть реализован в MessageSender.", true)]
    [ExcludeFromCodeCoverage]
    public BaseMessage Reply(BaseMessage message)
    {
      // @Kesteem:
      // Есть план перенести эти методы из адаптеров. 
      // Считаю эту реализацию некорректной, она пришла из разработки адаптера Rabbit. 
      // Методы должны быть реализованы в MessageSender.
      return null;
    }

    /// <inheritdoc />
    [Obsolete("Метод должен быть реализован в MessageSender.", true)]
    [ExcludeFromCodeCoverage]
    public Task<BaseMessage> RequestAndWaitResponse(BaseMessage message)
    {
      // @Kesteem:
      // Есть план перенести эти методы из адаптеров. 
      // Считаю эту реализацию некорректной, она пришла из разработки адаптера Rabbit. 
      // Методы должны быть реализованы в MessageSender.
      return Task.FromResult<BaseMessage>(null);
    }

    /// <inheritdoc />
    public void Subscribe(Func<BaseMessage, Task> handler)
    {
      handler.ThrowIfNull(nameof(handler));

      GetConsumer().MessageListener = msg => handler.Invoke(msg.ConvertToBaseMessage());
    }

    /// <inheritdoc />
    public void Unsubscribe()
    {
      GetConsumer().MessageListener = null;
    }
  }
}