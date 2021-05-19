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
  /// Отправка сериализованных сообщений
  /// </summary>
  /// <typeparam name="TMessage">Тип сообщения</typeparam>
  public class SerializedMessageSender<TMessage> : MessageSender, ISerializedMessageSender<TMessage>
  {
    private readonly IMessageSerializer<TMessage> _serializer;

    /// <inheritdoc />
    public SerializedMessageSender(
      IMessageSerializer<TMessage> serializer, IMessageAdapter adapter, ICallContextFactory callContextFactory, string applicationName) :
      base(adapter, callContextFactory, applicationName)
    {
      _serializer = serializer.ThrowIfNull(nameof(serializer));
    }

    /// <inheritdoc />
    public string SendMessage(TMessage message, string correlationId = null)
    {
      return SendMessage(message, null, correlationId);
    }

    public string SendMessage(TMessage message, IReadOnlyDictionary<string, object> additionalProperties, string correlationId = null)
    {
      BaseMessage mes;
      try
      {
        mes = _serializer.Serialize(message);
      }
      catch (Exception ex)
      {
        throw new MessagingException(ex, "Can not serialize message to xml");
      }

      mes.CorrelationId = correlationId;
      return SendMessage(mes, additionalProperties);
    }

    /// <inheritdoc />
    public async Task<string> SendMessageAsync(
      TMessage message, string correlationId = null, CancellationToken cancellationToken = default)
    {
      return await SendMessageAsync(message, null, correlationId, cancellationToken);
    }

    public async Task<string> SendMessageAsync(
      TMessage message, IReadOnlyDictionary<string, object> additionalProperties, string correlationId = null,
      CancellationToken cancellationToken = default)
    {
      BaseMessage mes;
      try
      {
        mes = _serializer.Serialize(message);
      }
      catch (Exception ex)
      {
        throw new MessagingException(ex, "Can not serialize message to xml");
      }

      mes.CorrelationId = correlationId;
      return await SendAsync(mes, additionalProperties, cancellationToken);
    }
  }
}
