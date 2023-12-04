using ViennaNET.Mediator.Pipelines;
using ViennaNET.Mediator.Seedwork;

namespace ViennaNET.Mediator.DefaultConfiguration
{
  internal class PipelineMessagePreProcessorRegistrar<TMessage, TMessagePreProcessor> where TMessage : class, IMessage
    where TMessagePreProcessor : IMessagePreProcessor<TMessage>
  {
    public PipelineMessagePreProcessorRegistrar(
      IPipelineProcessorsRegistrar registrar, TMessagePreProcessor preProcessor,
      PreProcessorOrder<TMessagePreProcessor> order)
    {
      registrar.RegisterMessagePreProcessor<TMessage, TMessagePreProcessor>(preProcessor, order.Order);
    }
  }
}