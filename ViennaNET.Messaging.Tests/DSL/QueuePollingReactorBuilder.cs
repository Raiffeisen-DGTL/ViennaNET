using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Processing.Impl.Poll;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class QueuePollingReactorBuilder
  {
    public QueuePollingReactor Please()
    {
      var messageAdapter = new Mock<IMessageAdapter>();

      var healthCheckingService = new Mock<IHealthCheckingService>();

      var messagingCca = new Mock<MessagingCallContextAccessor>();

      return new QueuePollingReactor(
        messageAdapter.Object,
        new IMessageProcessor[0],
        new IMessageProcessorAsync[0],
        100,
        null,
        healthCheckingService.Object,
        messagingCca.Object,
        Mock.Of<ILogger<QueuePollingReactor>>());
    }
  }
}