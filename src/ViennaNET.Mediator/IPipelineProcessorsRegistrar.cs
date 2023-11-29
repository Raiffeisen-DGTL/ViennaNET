using ViennaNET.Mediator.Pipelines;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator
{ 
  [Obsolete(
      "Данный пакет устарел и будет удален в ноябре 2023. Пожалуйста используйте ViennaNET.Extensions.Mediator")]
  public interface IPipelineProcessorsRegistrar
  {
    /// <summary>
    ///   Performs registration for message pre-processor.
    /// </summary>
    /// <typeparam name="TMessage">The type of message <see cref="IMessage" />.</typeparam>
    /// <typeparam name="TPipelineProcessor">The type of pre-processor.</typeparam>
    /// <param name="registerPreProcessor">Instance of pre-processor.</param>
    /// <param name="order">Order of pre-processor for ordered execution.</param>
    void RegisterMessagePreProcessor<TMessage, TPipelineProcessor>(TPipelineProcessor registerPreProcessor, int order)
      where TMessage : class, IMessage
      where TPipelineProcessor : IMessagePreProcessor<TMessage>;

    /// <summary>
    ///   Performs registration broadcast pre-processor.
    /// </summary>
    /// <typeparam name="TPipelineProcessor">Type of pre-processor.</typeparam>
    /// <param name="registerPreProcessor">Instance of pre-processor.</param>
    /// <param name="order">Order of pre-processor for ordered execution.</param>
    void RegisterBroadcastPreProcessor<TPipelineProcessor>(TPipelineProcessor registerPreProcessor, int order)
      where TPipelineProcessor : IBroadcastPreProcessor;
  }
}