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
  public class QueueSubscribeAndReplyReactor : QueueSubscribedReactorBase
  {
    private readonly IEnumerable<IRepliableMessageProcessorAsync> _asyncMessageProcessors;
    private readonly IEnumerable<IRepliableMessageProcessor> _messageProcessors;

    /// <inheritdoc />
    public QueueSubscribeAndReplyReactor(
      IMessageAdapterWithSubscribing messageAdapter, IEnumerable<IRepliableMessageProcessor> messageProcessors,
      IEnumerable<IRepliableMessageProcessorAsync> asyncMessageProcessor, int reconnectTimeout, string pollingId,
      bool? serviceHealthDependent, IHealthCheckingService healthCheckingService,
      IMessagingCallContextAccessor messagingCallContextAccessor) : base(messageAdapter, reconnectTimeout, pollingId,
                                                                        serviceHealthDependent, healthCheckingService,
                                                                        messagingCallContextAccessor)
    {
      _messageProcessors = messageProcessors.ThrowIfNull(nameof(messageProcessors));
      _asyncMessageProcessors = asyncMessageProcessor.ThrowIfNull(nameof(asyncMessageProcessor));
    }

    /// <inheritdoc />
    protected override bool GetProcessedMessage(BaseMessage message)
    {
      return _messageProcessors.Any(processor => processor.ProcessAndReply(message, x =>
      {
        x.ReplyQueue = message.ReplyQueue;
        adapter?.Reply(x);
      }));
    }

    /// <inheritdoc />
    protected override async Task<bool> GetProcessedMessageAsync(BaseMessage message)
    {
      var processed = false;
      foreach (var processor in _asyncMessageProcessors)
      {
        processed = await processor.ProcessAndReplyAsync(message, x =>
        {
          x.ReplyQueue = message.ReplyQueue;
          adapter?.Reply(x);
        });
        if (processed)
        {
          break;
        }
      }

      return processed;
    }
  }
}
