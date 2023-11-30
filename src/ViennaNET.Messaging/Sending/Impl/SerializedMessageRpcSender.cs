using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.CallContext;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Sending.Impl
{
  /// <summary>
  ///   Отправка сериализованных сообщений
  /// </summary>
  /// <typeparam name="TMessage">Тип сообщения</typeparam>
  /// <typeparam name="TResponse">Тип ответа</typeparam>
  public class SerializedMessageRpcSender<TMessage, TResponse> : MessageSender,
    ISerializedMessageRpcSender<TMessage, TResponse>
  {
    private readonly IMessageAdapter _adapter;
    private readonly IMessageDeserializer<TResponse> _deserializer;
    private readonly IMessageSerializer<TMessage> _serializer;

    /// <inheritdoc />
    public SerializedMessageRpcSender(
      IMessageSerializer<TMessage> serializer,
      IMessageDeserializer<TResponse> deserializer,
      IMessageAdapter adapter,
      ICallContextFactory callContextFactory,
      string applicationName) : base(adapter, callContextFactory, applicationName)
    {
      _adapter = adapter;
      _serializer = serializer.ThrowIfNull(nameof(serializer));
      _deserializer = deserializer.ThrowIfNull(nameof(deserializer));
    }

    /// <inheritdoc />
    public TResponse SendMessageAndWaitResponse(TMessage message)
    {
      return SendMessageAndWaitResponse(message, null);
    }

    public TResponse SendMessageAndWaitResponse(TMessage message,
      IReadOnlyDictionary<string, object> additionalProperties)
    {
      BaseMessage mes;
      try
      {
        mes = _serializer.Serialize(message);
      }
      catch (Exception ex)
      {
        throw new MessagingException(
          ex,
          $"Can't serialize message to xml for queue {_adapter.Configuration.Id}");
      }

      var response = SendAndWaitReplyMessage(mes, additionalProperties);
      return _deserializer.Deserialize(response);
    }

    /// <inheritdoc />
    public async Task<TResponse> SendMessageAndWaitResponseAsync(TMessage message,
      CancellationToken cancellationToken = default)
    {
      return await SendMessageAndWaitResponseAsync(message, null, cancellationToken);
    }

    public async Task<TResponse> SendMessageAndWaitResponseAsync(TMessage message,
      IReadOnlyDictionary<string, object> additionalProperties, CancellationToken cancellationToken = default)
    {
      BaseMessage mes;
      try
      {
        mes = _serializer.Serialize(message);
      }
      catch (Exception ex)
      {
        throw new MessagingException(
          ex,
          $"Can't serialize message to xml for queue {_adapter.Configuration.Id}");
      }

      var response = await SendAndWaitReplyMessageAsync(mes, additionalProperties, cancellationToken);
      return _deserializer.Deserialize(response);
    }
  }
}