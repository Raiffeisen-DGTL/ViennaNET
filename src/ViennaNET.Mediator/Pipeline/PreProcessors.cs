using System;
using System.Collections.Concurrent;
using ViennaNET.Mediator.Pipelines;

namespace ViennaNET.Mediator.Pipeline
{
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  internal class PreProcessors
  {
    public PreProcessors()
    {
      BroadcastPreProcessors = new ConcurrentQueue<IPipelineProcessor>();
      MessagePreProcessors = new ConcurrentDictionary<Type, ConcurrentQueue<IPipelineProcessor>>();
    }

    public ConcurrentQueue<IPipelineProcessor> BroadcastPreProcessors { get; set; }
    public ConcurrentDictionary<Type, ConcurrentQueue<IPipelineProcessor>> MessagePreProcessors { get; set; }

    public ConcurrentQueue<IPipelineProcessor> GetOrAddMessagePreProcessor(Type type)
    {
      return MessagePreProcessors.GetOrAdd(type, f => new ConcurrentQueue<IPipelineProcessor>());
    }
  }
}