using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ViennaNET.CallContext;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Receiving;
using ViennaNET.Messaging.Receiving.Impl;
using ViennaNET.Messaging.Sending;
using ViennaNET.Messaging.Sending.Impl;
using ViennaNET.Messaging.Tools;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.Factories.Impl
{
  /// <inheritdoc cref="IMessagingComponentFactory" />
  public class MessagingComponentFactory : IMessagingComponentFactory
  {
    private readonly IMessageAdapterFactory _adapterFactory;
    private readonly MessagingConfiguration _connectionOptions;
    private readonly IReadOnlyCollection<IMessageDeserializer> _deserializers;
    private readonly IReadOnlyCollection<IMessageSerializer> _serializers;
    private readonly ICallContextFactory _callContextFactory;

    /// <summary>
    ///   Инициализирует компонент ссылками на <see cref="IConfiguration" />,
    ///   <see cref="IMessageAdapterFactory" />,
    ///   коллекции элементов <see cref="IMessageSerializer" /> и <see cref="IMessageDeserializer" />>
    /// </summary>
    /// <param name="configuration">Провайдер конфигурации</param>
    /// <param name="adapterFactory">Фабрика адаптеров для очередей</param>
    /// <param name="serializers">Коллекция сериализаторов</param>
    /// <param name="deserializers">Коллекция десериализаторов</param>
    public MessagingComponentFactory(
      IConfiguration configuration, IMessageAdapterFactory adapterFactory, IReadOnlyCollection<IMessageSerializer> serializers,
      IReadOnlyCollection<IMessageDeserializer> deserializers, ICallContextFactory callContextFactory)
    {
      _adapterFactory = adapterFactory.ThrowIfNull(nameof(adapterFactory));
      _serializers = serializers.ThrowIfNull(nameof(serializers));
      _deserializers = deserializers.ThrowIfNull(nameof(deserializers));
      _callContextFactory = callContextFactory.ThrowIfNull(nameof(callContextFactory));
      _connectionOptions = configuration.ThrowIfNull(nameof(configuration))
                                        .GetSection("messaging")
                                        .Get<MessagingConfiguration>();
    }

    /// <inheritdoc />
    public IMessageSender CreateMessageSender(string queueId)
    {
      var adapter = _adapterFactory.Create(queueId.ThrowIfNullOrEmpty(nameof(queueId)), false);

      return new MessageSender(adapter, _callContextFactory, _connectionOptions.ApplicationName);
    }

    /// <inheritdoc />
    public IMessageReceiver<TMessage> CreateMessageReceiver<TMessage>(string queueId)
    {
      queueId.ThrowIfNullOrEmpty(nameof(queueId));

      var adapter = _adapterFactory.Create(queueId, false);
      var deserializer = GetMessageDeserializer<TMessage>(queueId);

      return new MessageReceiver<TMessage>(adapter, deserializer);
    }

    /// <inheritdoc />
    public ITransactedMessageReceiver<TMessage> CreateTransactedMessageReceiver<TMessage>(string queueId)
    {
      queueId.ThrowIfNullOrEmpty(nameof(queueId));

      var adapter = _adapterFactory.Create(queueId, false);
      var deserializer = GetMessageDeserializer<TMessage>(queueId);

      return new TransactedMessageReceiver<TMessage>((IMessageAdapterWithTransactions)adapter, deserializer);
    }

    /// <inheritdoc />
    public ISerializedMessageSender<TMessage> CreateMessageSender<TMessage>(string queueId)
    {
      queueId.ThrowIfNullOrEmpty(nameof(queueId));
      var adapter = _adapterFactory.Create(queueId, false);
      var serializer = GetMessageSerializer<TMessage>(queueId);

      return new SerializedMessageSender<TMessage>(serializer, adapter, _callContextFactory, _connectionOptions.ApplicationName);
    }

    /// <inheritdoc />
    public ISerializedMessageRpcSender<TMessage, TResponse> CreateMessageRpcSender<TMessage, TResponse>(string queueId)
    {
      queueId.ThrowIfNullOrEmpty(nameof(queueId));
      var adapter = _adapterFactory.Create(queueId, false);

      var serializer = GetMessageSerializer<TMessage>(queueId);
      var deserializer = GetMessageDeserializer<TResponse>(queueId);

      return new SerializedMessageRpcSender<TMessage, TResponse>(serializer, deserializer, adapter, _callContextFactory,
                                                                 _connectionOptions.ApplicationName);
    }

    private IMessageDeserializer<TMessage> GetMessageDeserializer<TMessage>(string queueId)
    {
      IMessageDeserializer<TMessage> deserializer;
      try
      {
        deserializer = _deserializers.OfType<IMessageDeserializer<TMessage>>()
                                     .SingleOrDefault();
      }
      catch (InvalidOperationException exception)
      {
        throw new MessagingException(exception,
                                     $"There are too many message deserializers with the message type {typeof(TMessage)} for queue id: {queueId}");
      }
      catch (Exception exception)
      {
        throw new MessagingException(exception,
                                     $"An error has been occured while fetching message deserializer with the message type {typeof(TMessage)} for queue id: {queueId}");
      }

      if (deserializer == null)
      {
        throw new
          MessagingException($"There are no message deserializers with the message type {typeof(TMessage)} for queue id: {queueId}");
      }

      return deserializer;
    }

    private IMessageSerializer<TMessage> GetMessageSerializer<TMessage>(string queueId)
    {
      IMessageSerializer<TMessage> serializer;
      try
      {
        serializer = _serializers.OfType<IMessageSerializer<TMessage>>()
                                 .SingleOrDefault();
      }
      catch (InvalidOperationException exception)
      {
        throw new MessagingException(exception,
                                     $"There are too many message serializers with the message type {typeof(TMessage)} for the queue id: {queueId}");
      }
      catch (Exception exception)
      {
        throw new MessagingException(exception,
                                     $"An error has been occured while fetching message serializer with the message type {typeof(TMessage)} for queue id: {queueId}");
      }

      if (serializer == null)
      {
        throw new
          MessagingException($"There are no message serializers with the message type {typeof(TMessage)} for the queue id: {queueId}");
      }

      return serializer;
    }
  }
}
