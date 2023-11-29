﻿using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Mediator.Pipelines;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator.Pipeline
{
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  public interface IPreProcessorService
  {
    void RegisterBroadcastPreProcessor<TPipelineProcessor>(TPipelineProcessor registerPreProcessor, int order)
      where TPipelineProcessor : IBroadcastPreProcessor;

    void RegisterMessagePreProcessor<TMessage, TPipelineProcessor>(TPipelineProcessor registerPreProcessor, int order)
      where TMessage : class, IMessage
      where TPipelineProcessor : IMessagePreProcessor<TMessage>;

    Task ExecuteAllPreProcessorsAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
      where TMessage : class, IMessage;

    void ExecuteAllPreProcessors<TMessage>(TMessage message) where TMessage : class, IMessage;
  }
}