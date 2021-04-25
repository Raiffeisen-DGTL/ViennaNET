using MessagingService.Features.Receive.Processors;
using ViennaNET.Messaging.Processing;

namespace MessagingService.Features.Receive
{
  public class Listener
  {
    private const string queueId = "exampleQueueId";

    private readonly IQueueReactor _queueReactor;

    public Listener(IQueueReactorFactory queueReactorFactory)
    {
      queueReactorFactory.Register<ExampleMessageProcessor>(queueId);
      _queueReactor = queueReactorFactory.CreateQueueReactor(queueId);
    }

    public void StartListening()
    {
      _queueReactor.StartProcessing();
    }

    public void StopListening()
    {
      _queueReactor.Stop();
    }
  }
}
