using Moq;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Processing.Impl.Subscribe;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class QueueSubscribedReactorBuilder
  {
    public QueueSubscribedReactor Please()
    {
      var messageAdapter = new Mock<IMessageAdapterWithSubscribing>();

      var healthCheckingService = new Mock<IHealthCheckingService>();

      var messagingCca = new Mock<MessagingCallContextAccessor>();

      return new QueueSubscribedReactor(
        messageAdapter.Object,
        new IMessageProcessor[0],
        new IMessageProcessorAsync[0],
        100,
        null,
        null,
        healthCheckingService.Object,
        messagingCca.Object);
    }
  }
}
