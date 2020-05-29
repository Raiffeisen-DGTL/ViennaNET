using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.CallContext;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Sending.Impl
{
  /// <inheritdoc cref="IMessageSender"/>
  public class MessageSender : IMessageSender
  {
    private readonly IMessageAdapter _adapter;
    private readonly string _applicationName;
    private readonly ICallContextFactory _callContextFactory;
    private bool _disposed;

    /// <summary>
    /// Инициализирует отправителя сообщений с определенным адаптером очереди <see cref="IMessageAdapter"/>
    /// </summary>
    /// <param name="adapter">Адаптер сообщений <see cref="IMessageAdapter"/></param>
    /// <param name="applicationName">Имя приложения</param>
    public MessageSender(IMessageAdapter adapter, ICallContextFactory callContextFactory, string applicationName)
    {
      _adapter = adapter.ThrowIfNull(nameof(adapter));
      _callContextFactory = callContextFactory.ThrowIfNull(nameof(callContextFactory));

      _applicationName = applicationName;
    }

    /// <inheritdoc />
    public string SendMessage(BaseMessage message, IReadOnlyDictionary<string, object> additionalProperties = null)
    {
      ThrowIfDisposed();

      message.ThrowIfNull(nameof(message));

      if (!string.IsNullOrWhiteSpace(_applicationName))
      {
        message.ApplicationTitle = _applicationName;
      }

      FillPropertiesFromCallContext(message);
      FillPropertiesFromQueueConfiguration(message);
      FillAdditionalProperties(message, additionalProperties);

      if (!_adapter.IsConnected)
      {
        _adapter.Connect();
      }

      var sendedMessage = _adapter.Send(message);
      return sendedMessage.MessageId;
    }

    /// <inheritdoc />
    public async Task<string> SendAsync(BaseMessage message, IReadOnlyDictionary<string, object> additionalProperties = null, CancellationToken cancellationToken = default)
    {
      return await Task.Factory.StartNew(() => SendMessage(message, additionalProperties), cancellationToken);
    }

    /// <inheritdoc />
    public BaseMessage SendAndWaitReplyMessage(BaseMessage message, IReadOnlyDictionary<string, object> additionalProperties = null)
    {
      return SendAndWaitReplyMessageAsync(message, additionalProperties).Result;
    }

    /// <inheritdoc />
    public async Task<BaseMessage> SendAndWaitReplyMessageAsync(BaseMessage message, IReadOnlyDictionary<string, object> additionalProperties = null, CancellationToken cancellationToken = default)
    {
      if (!(_adapter is IMessageAdapterWithSubscribing adapter))
      {
        throw new MessagingException("Adapter is not supports send and wait for reply interaction");
      }

      ThrowIfDisposed();

      message.ThrowIfNull(nameof(message));

      if (!string.IsNullOrWhiteSpace(_applicationName))
      {
        message.ApplicationTitle = _applicationName;
      }

      FillPropertiesFromCallContext(message);
      FillPropertiesFromQueueConfiguration(message);
      FillAdditionalProperties(message, additionalProperties);

      if (!adapter.IsConnected)
      {
        adapter.Connect();
      }

      var response = await adapter.RequestAndWaitResponse(message);
      return response;
    }

    /// <inheritdoc />
    public void Dispose()
    {
      if (_disposed)
      {
        return;
      }

      _disposed = true;

      _adapter?.Dispose();
    }

    protected void FillPropertiesFromCallContext(BaseMessage message)
    {
      var context = _callContextFactory.Create();

      message.Properties.Add(MessagingContextHeaders.UserId, context.UserId);
      message.Properties.Add(MessagingContextHeaders.UserDomain, context.UserDomain);
      message.Properties.Add(MessagingContextHeaders.RequestId, context.RequestId);
      message.Properties.Add(MessagingContextHeaders.RequestCallerIp, context.RequestCallerIp);
      message.Properties.Add(MessagingContextHeaders.AuthorizeInfo, context.AuthorizeInfo);
    }

    protected void FillPropertiesFromQueueConfiguration(BaseMessage message)
    {
      var queueConfiguration = _adapter.Configuration;

      if (queueConfiguration.Lifetime.HasValue)
      {
        message.LifeTime = queueConfiguration.Lifetime.Value;
      }

      if (!string.IsNullOrWhiteSpace(queueConfiguration.ReplyQueue))
      {
        message.ReplyQueue = queueConfiguration.ReplyQueue;
      }

      if (queueConfiguration.CustomHeaders == null)
      {
        return;
      }

      foreach (var kv in queueConfiguration.CustomHeaders.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)))
      {
        message.Properties.Add(kv.Key, kv.Value);
      }
    }

    protected void FillAdditionalProperties(BaseMessage mes, IReadOnlyDictionary<string, object> properties = null)
    {
      if (properties?.Any() != true)
      {
        return;
      }

      foreach (var property in properties)
      {
        mes.Properties.Add(property.Key, property.Value);
      }
    }

    private void ThrowIfDisposed()
    {
      if (_disposed)
      {
        throw new MessagingException("Message sender is already disposed");
      }
    }
  }
}
