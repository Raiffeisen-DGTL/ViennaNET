using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging.Processing;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Listening
{
  [ExcludeFromCodeCoverage]
  public class QueueListener<TProcessor> : IQueueListener where TProcessor : class
  {
    private readonly ILogger _logger;
    private readonly string _queueName;

    private readonly IQueueReactorFactory _queueReactorFactory;
    private IQueueReactor _queueReactor;

    public QueueListener(IQueueReactorFactory queueReactorFactory,
      ILogger<QueueListener<TProcessor>> logger,
      string queueName)
    {
      _queueReactorFactory = queueReactorFactory.ThrowIfNull(nameof(queueReactorFactory));
      _queueName = queueName.ThrowIfNull(nameof(queueName));
      _logger = logger.ThrowIfNull(nameof(logger));

      _queueReactorFactory.Register<TProcessor>(_queueName);
    }

    public void Start()
    {
      if (_queueReactor != null)
      {
        Stop();
      }

      _queueReactor = _queueReactorFactory.CreateQueueReactor(_queueName);
      _queueReactor.NeedReconnect += QueueReactorOnNeedReconnect;
      _queueReactor.StartProcessing();
    }

    public void Stop()
    {
      if (_queueReactor != null)
      {
        _queueReactor.Stop();
        _queueReactor.NeedReconnect -= QueueReactorOnNeedReconnect;
      }
    }

    private void QueueReactorOnNeedReconnect(object sender, EventArgs eventArgs)
    {
      _logger.LogError("Requests for check in queue adapter need to be reconnected, errorCount = {0}",
        ((IQueueReactor)sender).ErrorCount);
    }
  }
}