using ViennaNET.Mediator.Pipelines;

namespace ViennaNET.Mediator.DefaultConfiguration
{
  internal class PipelineBroadcastPreProcessorRegistrar<TMessagePreProcessor> where TMessagePreProcessor : IBroadcastPreProcessor
  {
    public PipelineBroadcastPreProcessorRegistrar(
      IPipelineProcessorsRegistrar registrar, TMessagePreProcessor preProcessor, PreProcessorOrder<TMessagePreProcessor> order)
    {
      registrar.RegisterBroadcastPreProcessor(preProcessor, order.Order);
    }
  }
}
