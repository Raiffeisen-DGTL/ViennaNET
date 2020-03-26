using ViennaNET.Mediator.Pipelines;
using System;
using System.Collections.Concurrent;

namespace ViennaNET.Mediator.Pipeline
{
  internal class PreProcessors
  {
    public ConcurrentQueue<IPipelineProcessor> BroadcastPreProcessors { get; set; }
    public ConcurrentDictionary<Type, ConcurrentQueue<IPipelineProcessor>> MessagePreProcessors { get; set; }

    public PreProcessors()
    {
      BroadcastPreProcessors = new ConcurrentQueue<IPipelineProcessor>();
      MessagePreProcessors = new ConcurrentDictionary<Type, ConcurrentQueue<IPipelineProcessor>>();
    }

    public ConcurrentQueue<IPipelineProcessor> GetOrAddMessagePreProcessor(Type type)
    {
      return MessagePreProcessors.GetOrAdd(type, f => new ConcurrentQueue<IPipelineProcessor>());
    }
  }
}
