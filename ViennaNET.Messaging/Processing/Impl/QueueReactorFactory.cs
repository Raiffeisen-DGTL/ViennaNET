using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViennaNET.Diagnostic;
using ViennaNET.Logging;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.Processing.Impl.Poll;
using ViennaNET.Messaging.Processing.Impl.Subscribe;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Processing.Impl
{
  /// <inheritdoc cref="IQueueReactorFactory" />
  public class QueueReactorFactory : IQueueReactorFactory
  {
    private readonly IEnumerable<IProcessorAsync> _asyncProcessors;
    private readonly IHealthCheckingService _healthCheckingService;
    private readonly IMessageAdapterFactory _messageAdapterFactory;
    private readonly IEnumerable<IProcessor> _processors;
    private readonly IMessagingCallContextAccessor _messagingCallContextAccessor;
    private readonly Dictionary<Type, string> _registrations = new Dictionary<Type, string>();

    /// <summary>
    ///   Инициализирует экземпляр класса ссылками на <see cref="IMessageAdapterFactory" />,
    ///   <see cref="IHealthCheckingService" />,
    ///   коллекции элементов <see cref="IProcessor" /> и <see cref="IProcessorAsync" />
    /// </summary>
    /// <param name="messageAdapterFactory"></param>
    /// <param name="messageProcessors"></param>
    /// <param name="asyncMessageProcessors"></param>
    /// <param name="healthCheckingService"></param>
    public QueueReactorFactory(
      IMessageAdapterFactory messageAdapterFactory, IEnumerable<IProcessor> messageProcessors,
      IEnumerable<IProcessorAsync> asyncMessageProcessors, IHealthCheckingService healthCheckingService,
      IMessagingCallContextAccessor messagingCallContextAccessor)
    {
      _messageAdapterFactory = messageAdapterFactory.ThrowIfNull(nameof(messageAdapterFactory));
      _processors = messageProcessors.ThrowIfNull(nameof(messageProcessors));
      _asyncProcessors = asyncMessageProcessors.ThrowIfNull(nameof(asyncMessageProcessors));
      _healthCheckingService = healthCheckingService.ThrowIfNull(nameof(healthCheckingService));
      _messagingCallContextAccessor = messagingCallContextAccessor.ThrowIfNull(nameof(messagingCallContextAccessor));

      LogProcessors(_processors.ToList(), _asyncProcessors.ToList());
    }

    /// <inheritdoc />
    /// <exception cref="MessageProcessorAlreadyRegisterException"></exception>
    public IMessageProcessorRegister Register<T>(string queueId) where T : class
    {
      queueId.ThrowIfNullOrEmpty(nameof(queueId));
      var type = typeof(T);
      Logger.LogDebug($"Try to register message processor with type: {type} for queue: {queueId}");
      if (_registrations.ContainsKey(type))
      {
        throw new
          MessageProcessorAlreadyRegisterException($"The message processor {type.Name} is already registered in the queue factory");
      }

      _registrations.Add(type, queueId);
      Logger.LogDebug($"Processor with type: {typeof(T)} for queue: {queueId} has been registered");
      return this;
    }

    /// <inheritdoc />
    public IQueueReactor CreateQueueReactor(string queueId)
    {
      var adapter = _messageAdapterFactory.Create(queueId, false);
      GetAllProcessors(queueId, out var processors, out var asyncProcessors);

      var queueConfiguration = adapter.Configuration;

      if (!adapter.SupportProcessingType(queueConfiguration.ProcessingType))
      {
        throw new
          MessagingException($"Processing type: '{queueConfiguration.ProcessingType}' not available for queue named: '{queueConfiguration.QueueName}'");
      }

      var messageProcessors = processors.OfType<IMessageProcessor>();
      var messageAsyncProcessors = asyncProcessors.OfType<IMessageProcessorAsync>();
      switch (adapter)
      {
        case IMessageAdapterWithTransactions transacted:
          return new QueueTransactedPollingReactor(transacted, messageProcessors, messageAsyncProcessors,
                                                   queueConfiguration.IntervalPollingQueue, queueConfiguration.PollingId,
                                                   queueConfiguration.ServiceHealthDependent, _healthCheckingService, _messagingCallContextAccessor);
        case IMessageAdapterWithSubscribing subscribing:
          switch (queueConfiguration.ProcessingType)
          {
            case MessageProcessingType.Subscribe:
              return new QueueSubscribedReactor(subscribing, messageProcessors, messageAsyncProcessors,
                                                queueConfiguration.IntervalPollingQueue, queueConfiguration.PollingId,
                                                queueConfiguration.ServiceHealthDependent, _healthCheckingService, _messagingCallContextAccessor);
            case MessageProcessingType.SubscribeAndReply:
              var replyProcessors = processors.OfType<IRepliableMessageProcessor>();
              var replyAsyncProcessors = asyncProcessors.OfType<IRepliableMessageProcessorAsync>();
              return new QueueSubscribeAndReplyReactor(subscribing, replyProcessors, replyAsyncProcessors,
                                                       queueConfiguration.IntervalPollingQueue, queueConfiguration.PollingId,
                                                       queueConfiguration.ServiceHealthDependent, _healthCheckingService, _messagingCallContextAccessor);
            default:
              throw new
                MessagingException($"There are found no QueueReactors with type '{nameof(queueConfiguration.ProcessingType)}' for queue with name '{queueId}'.");
          }
        default:
          return new QueuePollingReactor(adapter, messageProcessors, messageAsyncProcessors, queueConfiguration.IntervalPollingQueue,
                                         queueConfiguration.PollingId, queueConfiguration.ServiceHealthDependent, _healthCheckingService, _messagingCallContextAccessor);
      }
    }

    private static void LogProcessors(IReadOnlyCollection<IProcessor> processors, IReadOnlyCollection<IProcessorAsync> asyncProcessors)
    {
      if (!processors.Any() && !asyncProcessors.Any())
      {
        return;
      }

      var builder = new StringBuilder();
      builder.AppendLine("Processors have been registered in QueueReactorFactory:");
      foreach (var processor in processors)
      {
        builder.AppendLine($"- Sync processor {processor.GetType()}");
      }

      foreach (var asyncProcessor in asyncProcessors)
      {
        builder.AppendLine($"- Async processor {asyncProcessor.GetType()}");
      }

      Logger.LogDebug(builder.ToString());
    }

    private void GetAllProcessors(
      string queueName, out IReadOnlyCollection<IProcessor> processors, out IReadOnlyCollection<IProcessorAsync> asyncProcessors)
    {
      var unregistered = new List<IProcessor>();
      var asyncUnregistered = new List<IProcessorAsync>();
      processors = _processors.Where(x => ConfiguredWithQueueName(x, queueName, unregistered))
                              .ToArray();
      asyncProcessors = _asyncProcessors.Where(x => ConfiguredWithQueueName(x, queueName, asyncUnregistered))
                                        .ToArray();
      if (unregistered.Any() && asyncUnregistered.Any())
      {
        var unregisteredNames = unregistered.Select(x => x.GetType()
                                                          .Name);
        var asyncUnregisteredNames = asyncUnregistered.Select(x => x.GetType()
                                                                    .Name);
        var allUnregisteredNames = unregisteredNames.Union(asyncUnregisteredNames);
        var allUnregisteredString = string.Join(",", allUnregisteredNames);
        throw new MessagingException($"The message processors: {allUnregisteredString} not registered in the queue factory");
      }
    }

    private bool ConfiguredWithQueueName(IProcessor messageProcessor, string queueId, ICollection<IProcessor> unregistered)
    {
      if (_registrations.TryGetValue(messageProcessor.GetType(), out var processorQueue))
      {
        return processorQueue == queueId;
      }

      unregistered.Add(messageProcessor);
      return false;
    }

    private bool ConfiguredWithQueueName(IProcessorAsync messageProcessor, string queueId, ICollection<IProcessorAsync> unregistered)
    {
      if (_registrations.TryGetValue(messageProcessor.GetType(), out var processorQueue))
      {
        return processorQueue == queueId;
      }

      unregistered.Add(messageProcessor);
      return false;
    }
  }
}