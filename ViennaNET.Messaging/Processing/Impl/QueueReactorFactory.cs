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
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public IMessageProcessorRegister Register<T>(string queueId) where T : class
    {
      return Register(typeof(T), queueId);
    }

    /// <inheritdoc />
    /// <exception cref="MessageProcessorAlreadyRegisterException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public IMessageProcessorRegister Register(Type type, string queueId)
    {
      type.ThrowIfNull(nameof(type));
      if (!type.IsClass)
      {
        throw new ArgumentException("Only class can be registered as message processor", nameof(type));
      }

      queueId.ThrowIfNullOrEmpty(nameof(queueId));

      Logger.LogDebug($"Try to register message processor with type: {type} for queue: {queueId}");
      if (_registrations.ContainsKey(type))
      {
        throw new
          MessageProcessorAlreadyRegisterException($"The message processor {type.Name} is already registered in the queue factory");
      }

      _registrations.Add(type, queueId);
      Logger.LogDebug($"Processor with type: {type} for queue: {queueId} has been registered");
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
          MessagingException($"Processing type: '{queueConfiguration.ProcessingType}' not available for queue: '{queueId}'");
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
      string queueId, out IReadOnlyCollection<IProcessor> processors, out IReadOnlyCollection<IProcessorAsync> asyncProcessors)
    {
      ValidateProcessorRegistrations();

      processors = _processors.Where(x => ConfiguredWithQueueName(x, queueId)).ToArray();
      asyncProcessors = _asyncProcessors.Where(x => ConfiguredWithQueueName(x, queueId)).ToArray();

      if (!processors.Any() && !asyncProcessors.Any())
      {
        throw new MessagingException($"There are no message processors registered for queue '{queueId}'");
      }
    }

    private void ValidateProcessorRegistrations()
    {
      var unregistered = new LinkedList<string>();

      foreach (var processor in _processors)
      {
        var type = processor.GetType();
        if (!_registrations.ContainsKey(type))
        {
          unregistered.AddLast(type.Name);
        }
      }
      foreach (var processor in _asyncProcessors)
      {
        var type = processor.GetType();
        if (!_registrations.ContainsKey(type))
        {
          unregistered.AddLast(type.Name);
        }
      }

      if (unregistered.Count > 0)
      {
        var allUnregisteredString = string.Join(", ", unregistered);
        throw new MessagingException($"The message processors {allUnregisteredString} are not registered in the queue factory");
      }
    }

    private bool ConfiguredWithQueueName(object messageProcessor, string queueId)
    {
      return _registrations.TryGetValue(messageProcessor.GetType(), out var processorQueue) && processorQueue == queueId;
    }
  }
}