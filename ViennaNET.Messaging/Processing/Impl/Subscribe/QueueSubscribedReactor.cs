using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Processing.Impl.Subscribe
{
  /// <inheritdoc />
  public class QueueSubscribedReactor : QueueSubscribedReactorBase
  {
    private readonly IEnumerable<IMessageProcessorAsync> _asyncMessageProcessors;
    private readonly IEnumerable<IMessageProcessor> _messageProcessors;

    /// <inheritdoc />
    public QueueSubscribedReactor(
      IMessageAdapterWithSubscribing messageAdapter, IEnumerable<IMessageProcessor> messageProcessors,
      IEnumerable<IMessageProcessorAsync> asyncMessageProcessor, int reconnectTimeout, string pollingId, bool? serviceHealthDependent,
      IHealthCheckingService healthCheckingService, IMessagingCallContextAccessor messagingCallContextAccessor) : base(messageAdapter,
                                                                                                                      reconnectTimeout,
                                                                                                                      pollingId,
                                                                                                                      serviceHealthDependent,
                                                                                                                      healthCheckingService,
                                                                                                                      messagingCallContextAccessor)
    {
      _messageProcessors = messageProcessors.ThrowIfNull(nameof(messageProcessors));
      _asyncMessageProcessors = asyncMessageProcessor.ThrowIfNull(nameof(asyncMessageProcessor));
    }

    /// <inheritdoc />
    protected override bool GetProcessedMessage(BaseMessage message)
    {
      return _messageProcessors.Any(processor => processor.Process(message));
    }

    /// <inheritdoc />
    protected override async Task<bool> GetProcessedMessageAsync(BaseMessage message)
    {
      var processed = false;
      foreach (var processor in _asyncMessageProcessors)
      {
        processed = await processor.ProcessAsync(message);
        if (processed)
        {
          break;
        }
      }

      return processed;
    }
  }
}
