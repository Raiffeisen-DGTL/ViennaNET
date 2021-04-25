using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Mediator.Pipelines;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator.Pipeline
{
  public class PreProcessorService : IPreProcessorService
  {
    private readonly object _preProcessorsSync;
    private readonly SortedList<int, PreProcessors> _preProcessorsOrdered;
    private readonly ConcurrentQueue<PreProcessors> _preProcessorsQueue;
    private readonly ConcurrentBag<Type> _preProcessorsTypes;

    public PreProcessorService()
    {
      _preProcessorsSync = new object();
      _preProcessorsOrdered = new SortedList<int, PreProcessors>();
      _preProcessorsQueue = new ConcurrentQueue<PreProcessors>();
      _preProcessorsTypes = new ConcurrentBag<Type>();
    }

    private void RefreshPreProcessorsQueue()
    {
      while (!_preProcessorsQueue.IsEmpty)
      {
        _preProcessorsQueue.TryDequeue(out _);
      }
      lock (_preProcessorsSync)
      {
        foreach (var preProcessor in _preProcessorsOrdered)
        {
          _preProcessorsQueue.Enqueue(preProcessor.Value);
        }
      }
    }

    private PreProcessors GetPreProcessors(int order)
    {
      PreProcessors processors;
      lock (_preProcessorsSync)
      {
        if (!_preProcessorsOrdered.TryGetValue(order, out processors))
        {
          processors = new PreProcessors();
          _preProcessorsOrdered.Add(order, processors);
        }
      }
      return processors;
    }

    public void RegisterMessagePreProcessor<TMessage, TPipelineProcessor>(TPipelineProcessor registerPreProcessor, int order)
      where TMessage : class, IMessage
      where TPipelineProcessor : IMessagePreProcessor<TMessage>
    {
      if (registerPreProcessor == null)
      {
        throw new ArgumentNullException(nameof(registerPreProcessor));
      }

      if (_preProcessorsTypes.Contains(registerPreProcessor.GetType()))
      {
        throw new ArgumentException($"The pre-processor {typeof(TPipelineProcessor)} for message {typeof(TMessage).FullName} is already registered.", nameof(registerPreProcessor));
      }
      _preProcessorsTypes.Add(registerPreProcessor.GetType());

      var processors = GetPreProcessors(order);

      var messagePreProcessorsQueue = processors.GetOrAddMessagePreProcessor(typeof(TMessage));
      messagePreProcessorsQueue.Enqueue(registerPreProcessor);

      RefreshPreProcessorsQueue();
    }

    public void RegisterBroadcastPreProcessor<TPipelineProcessor>(TPipelineProcessor registerPreProcessor, int order)
      where TPipelineProcessor : IBroadcastPreProcessor
    {
      if (registerPreProcessor == null)
      {
        throw new ArgumentNullException(nameof(registerPreProcessor));
      }

      if (_preProcessorsTypes.Contains(registerPreProcessor.GetType()))
      {
        throw new ArgumentException($"The broadcast pre-processor {typeof(TPipelineProcessor)} is already registered.", nameof(registerPreProcessor));
      }
      _preProcessorsTypes.Add(registerPreProcessor.GetType());

      var processors = GetPreProcessors(order);

      var broadcastPreProcessorsQueue = processors.BroadcastPreProcessors;
      broadcastPreProcessorsQueue.Enqueue(registerPreProcessor);

      RefreshPreProcessorsQueue();
    }

    public async Task ExecuteAllPreProcessorsAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class, IMessage
    {
      foreach (var preProcessors in _preProcessorsQueue)
      {
        foreach (var broadcastPreProcessor in preProcessors.BroadcastPreProcessors)
        {
          await ((IBroadcastPreProcessor)broadcastPreProcessor).ProcessAsync(message, cancellationToken);
        }
        if (!preProcessors.MessagePreProcessors.TryGetValue(message.GetType(), out var processors))
        {
          continue;
        }
        foreach (var processor in processors)
        {
          await ((IMessagePreProcessor<TMessage>)processor).ProcessAsync(message, cancellationToken);
        }
      }
    }

    public void ExecuteAllPreProcessors<TMessage>(TMessage message) where TMessage : class, IMessage
    {
      foreach (var preProcessors in _preProcessorsQueue)
      {
        foreach (var broadcastPreProcessor in preProcessors.BroadcastPreProcessors)
        {
          ((IBroadcastPreProcessor)broadcastPreProcessor).Process(message);
        }
        if (!preProcessors.MessagePreProcessors.TryGetValue(message.GetType(), out var processors))
        {
          continue;
        }
        foreach (var processor in processors)
        {
          ((IMessagePreProcessor<TMessage>)processor).Process(message);
        }
      }
    }
  }
}
