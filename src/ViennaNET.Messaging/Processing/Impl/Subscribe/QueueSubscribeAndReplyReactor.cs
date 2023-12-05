using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Processing.Impl.Subscribe
{
  /// <inheritdoc />
  public class QueueSubscribeAndReplyReactor : QueueSubscribedReactorBase
  {
    private readonly IEnumerable<IRepliableMessageProcessorAsync> _asyncMessageProcessors;
    private readonly IEnumerable<IRepliableMessageProcessor> _messageProcessors;

    /// <inheritdoc />
    public QueueSubscribeAndReplyReactor(
      IMessageAdapterWithSubscribing messageAdapter,
      IEnumerable<IRepliableMessageProcessor> messageProcessors,
      IEnumerable<IRepliableMessageProcessorAsync> asyncMessageProcessor,
      int reconnectTimeout,
      bool? serviceHealthDependent,
      IHealthCheckingService healthCheckingService,
      IMessagingCallContextAccessor messagingCallContextAccessor,
      ILogger<QueueSubscribeAndReplyReactor> logger) : base(messageAdapter,
      reconnectTimeout,
      serviceHealthDependent,
      healthCheckingService,
      messagingCallContextAccessor,
      logger)
    {
      _messageProcessors = messageProcessors;
      _asyncMessageProcessors = asyncMessageProcessor;
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