using System;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Receiving.Impl
{
  /// <inheritdoc />
  public class MessageReceiver<TMessage> : IMessageReceiver<TMessage>
  {
    private readonly IMessageAdapter _adapter;
    private readonly IMessageDeserializer<TMessage> _deserializer;

    private bool _disposed;

    /// <summary>
    ///   Конструктор, инициализирующий экземпляр зависимостями
    ///   <see cref="IMessageAdapter" /> и <see cref="IMessageDeserializer" />>
    /// </summary>
    /// <param name="adapter"></param>
    /// <param name="deserializer"></param>
    public MessageReceiver(IMessageAdapter adapter, IMessageDeserializer<TMessage> deserializer)
    {
      _adapter = adapter.ThrowIfNull(nameof(adapter));
      _deserializer = deserializer.ThrowIfNull(nameof(deserializer));
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

    /// <inheritdoc />
    public TMessage Receive(TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      CheckDisposed();
      return ReceiveMessageInternal(null, timeout, out _, additionalParameters);
    }

    /// <inheritdoc />
    public TMessage Receive(
      out BaseMessage receivedMessage, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      CheckDisposed();
      return ReceiveMessageInternal(null, timeout, out receivedMessage, additionalParameters);
    }

    /// <inheritdoc />
    public TMessage Receive(string correlationId, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      correlationId.ThrowIfNullOrEmpty(nameof(correlationId));

      CheckDisposed();

      return ReceiveMessageInternal(correlationId, timeout, out _, additionalParameters);
    }

    /// <inheritdoc />
    public TMessage Receive(
      string correlationId, out BaseMessage receivedMessage, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters)
    {
      correlationId.ThrowIfNullOrEmpty(nameof(correlationId));

      CheckDisposed();

      return ReceiveMessageInternal(correlationId, timeout, out receivedMessage, additionalParameters);
    }

    /// <inheritdoc />
    public bool TryReceive(out TMessage message, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      CheckDisposed();

      return TryReceiveMessageInternal(null, timeout, out message, out _, additionalParameters);
    }

    /// <inheritdoc />
    public bool TryReceive(
      out TMessage message, out BaseMessage receivedMessage, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters)
    {
      CheckDisposed();

      return TryReceiveMessageInternal(null, timeout, out message, out receivedMessage, additionalParameters);
    }

    /// <inheritdoc />
    public bool TryReceive(
      string correlationId, out TMessage message, TimeSpan? timeout = null, params (string Name, string Value)[] additionalParameters)
    {
      correlationId.ThrowIfNullOrEmpty(nameof(correlationId));

      CheckDisposed();

      return TryReceiveMessageInternal(correlationId, timeout, out message, out _, additionalParameters);
    }

    /// <inheritdoc />
    public bool TryReceive(
      string correlationId, out TMessage message, out BaseMessage receivedMessage, TimeSpan? timeout = null,
      params (string Name, string Value)[] additionalParameters)
    {
      correlationId.ThrowIfNullOrEmpty(nameof(correlationId));

      CheckDisposed();

      return TryReceiveMessageInternal(correlationId, timeout, out message, out receivedMessage, additionalParameters);
    }

    private TMessage ReceiveMessageInternal(
      string correlationId, TimeSpan? timeout, out BaseMessage receivedMessage, params (string Name, string Value)[] additionalParameters)
    {
      if (!_adapter.IsConnected)
      {
        _adapter.Connect();
      }

      var msg = _adapter.Receive(correlationId, timeout, additionalParameters);
      receivedMessage = msg;
      if (msg.IsEmpty())
      {
        throw new MessagingException("Can not deserialize message. Message body is empty");
      }

      try
      {
        var deserializedMessage = _deserializer.Deserialize(msg);
        return deserializedMessage;
      }
      catch (Exception ex)
      {
        throw new MessagingException(ex, "Can not deserialize message");
      }
    }

    private bool TryReceiveMessageInternal(
      string correlationId, TimeSpan? timeout, out TMessage message, out BaseMessage receivedMessage,
      params (string Name, string Value)[] additionalParameters)
    {
      if (!_adapter.IsConnected)
      {
        _adapter.Connect();
      }

      var isReceived = _adapter.TryReceive(out receivedMessage, correlationId, timeout, additionalParameters);

      if (isReceived && receivedMessage != null)
      {
        if (receivedMessage.IsEmpty())
        {
          throw new MessagingException("Can't deserialize message. Message body is empty");
        }

        try
        {
          var deserializedMessage = _deserializer.Deserialize(receivedMessage);
          message = deserializedMessage;
          return true;
        }
        catch (Exception ex)
        {
          throw new MessagingException(ex, "Can't deserialize message");
        }
      }

      message = default;
      receivedMessage = null;
      return false;
    }

    private void CheckDisposed()
    {
      if (_disposed)
      {
        throw new ObjectDisposedException("MessageSender", "Message sender is already disposed");
      }
    }
  }
}