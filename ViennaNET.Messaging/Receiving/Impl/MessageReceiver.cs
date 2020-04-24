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

    /// <summary>
    ///  Конструктор, инициализирующий экземпляр зависимостями
    ///  <see cref="IMessageAdapter"/> и <see cref="IMessageDeserializer"/>>
    /// </summary>
    /// <param name="adapter"></param>
    /// <param name="deserializer"></param>
    public MessageReceiver(IMessageAdapter adapter, IMessageDeserializer<TMessage> deserializer)
    {
      _adapter = adapter.ThrowIfNull(nameof(adapter));
      _deserializer = deserializer.ThrowIfNull(nameof(deserializer));
    }

    private TMessage ReceiveMessageInternal(string correlationId, out BaseMessage receivedMessage)
    {
      if (!_adapter.IsConnected)
      {
        _adapter.Connect();
      }

      var msg = _adapter.Receive(correlationId);
      receivedMessage = msg;
      if (string.IsNullOrWhiteSpace(msg.GetBodyAsString()))
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

    private bool TryReceiveMessageInternal(string correlationId, out TMessage message, out BaseMessage receivedMessage)
    {
      if (!_adapter.IsConnected)
      {
        _adapter.Connect();
      }

      var isReceived = _adapter.TryReceive(out receivedMessage, correlationId);


      if (isReceived && receivedMessage != null)
      {
        if (string.IsNullOrWhiteSpace(receivedMessage.GetBodyAsString()))
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


    /// <inheritdoc />
    public TMessage Receive()
    {
      CheckDisposed();
      return ReceiveMessageInternal(null, out _);
    }


    /// <inheritdoc />
    public TMessage Receive(out BaseMessage receivedMessage)
    {
      CheckDisposed();
      return ReceiveMessageInternal(null, out receivedMessage);
    }

    /// <inheritdoc />
    public TMessage Receive(string correlationId)
    {
      correlationId.ThrowIfNullOrEmpty(nameof(correlationId));

      CheckDisposed();

      return ReceiveMessageInternal(correlationId, out _);
    }

    /// <inheritdoc />
    public TMessage Receive(string correlationId, out BaseMessage receivedMessage)
    {
      correlationId.ThrowIfNullOrEmpty(nameof(correlationId));

      CheckDisposed();

      return ReceiveMessageInternal(correlationId, out receivedMessage);
    }

    /// <inheritdoc />
    public bool TryReceive(out TMessage message)
    {
      CheckDisposed();
      
      return TryReceiveMessageInternal(null, out message, out _);
    }

    /// <inheritdoc />
    public bool TryReceive(out TMessage message, out BaseMessage receivedMessage)
    {
      CheckDisposed();

      return TryReceiveMessageInternal(null, out message, out receivedMessage);
    }

    /// <inheritdoc />
    public bool TryReceive(string correlationId, out TMessage message)
    {
      correlationId.ThrowIfNullOrEmpty(nameof(correlationId));

      CheckDisposed();

      return TryReceiveMessageInternal(correlationId, out message, out _);
    }

    /// <inheritdoc />
    public bool TryReceive(string correlationId, out TMessage message, out BaseMessage receivedMessage)
    {
      correlationId.ThrowIfNullOrEmpty(nameof(correlationId));

      CheckDisposed();


      return TryReceiveMessageInternal(correlationId,  out message, out receivedMessage);
    }

    private void CheckDisposed()
    {
      if (_disposed)
      {
        throw new ObjectDisposedException("MessageSender", "Message sender is already disposed");
      }
    }

    private bool _disposed;

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
  }
}
