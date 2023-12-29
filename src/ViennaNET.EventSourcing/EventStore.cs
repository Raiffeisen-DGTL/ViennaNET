using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ViennaNET.EventSourcing.EventMappers;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Messaging.Factories;
using ViennaNET.Orm.Application;
using ViennaNET.Utils;

namespace ViennaNET.EventSourcing;

/// <summary>
///     Реализация хранилища событий. Позволяет дополнительно отправлять сообщения во внешнюю очередь.
/// </summary>
public class EventStore : IEventStore
{
    private readonly IEntityFactoryService _entityFactoryService;
    private readonly IIntegrationEventMapperFactory _integrationEventMapperStrategy;
    private readonly ILogger<EventStore> _logger;
    private readonly IMessagingComponentFactory _messagingComponentFactory;

    /// <summary>
    ///     Инициализирует хранилище зависимостями <see cref="IEntityFactoryService" />,
    ///     <see cref="IMessagingComponentFactory" />, <see cref="IIntegrationEventMapperFactory" />.
    /// </summary>
    /// <param name="logger">Ссылка на экземпляр <see cref="ILogger{TCategoryName}" />.</param>
    /// <param name="entityFactoryService">Фабричный сервис для инфраструктурных операций с БД.</param>
    /// <param name="messagingComponentFactory">Фабрика компонентов для обмена сообщениями.</param>
    /// <param name="integrationEventMapperFactory">Фабрика подбора преобразователей событий.</param>
    public EventStore(
        ILogger<EventStore> logger,
        IEntityFactoryService entityFactoryService,
        IMessagingComponentFactory messagingComponentFactory,
        IIntegrationEventMapperFactory integrationEventMapperFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _entityFactoryService = entityFactoryService.ThrowIfNull(nameof(entityFactoryService));
        _messagingComponentFactory = messagingComponentFactory.ThrowIfNull(nameof(messagingComponentFactory));
        _integrationEventMapperStrategy =
            integrationEventMapperFactory.ThrowIfNull(nameof(integrationEventMapperFactory));
    }

    /// <inheritdoc />
    public IEnumerable<T> LoadFromStream<T>(Expression<Func<T, bool>> predicate) where T : class, IIntegrationEvent
    {
        return _entityFactoryService.Create<T>()
            .Query()
            .Where(predicate)
            .OrderBy(x => x.Timestamp);
    }

    /// <inheritdoc />
    public void AppendToStream<T>(bool withCommit, params IEvent[] events) where T : class, IIntegrationEvent
    {
        using var uow = withCommit ? _entityFactoryService.Create() : null;
        var integrationEvents = events
            .Select(e => _integrationEventMapperStrategy.GetMapper<T>(e).Map(e, JsonConvert.SerializeObject(e)))
            .ToArray();
        
        SaveToStore(integrationEvents);
        uow?.Commit();
    }

    /// <inheritdoc />
    public void AppendToStream<T>(bool withCommit, string queueId, params IEvent[] events)
        where T : class, IIntegrationEvent
    {
        using var uow = withCommit ? _entityFactoryService.Create() : null;
        var integrationEvents = events
            .Select(e => _integrationEventMapperStrategy.GetMapper<T>(e).Map(e, JsonConvert.SerializeObject(e)))
            .ToArray();
        
        SaveToStore(integrationEvents);
        SendEvents(queueId, integrationEvents);
        uow?.Commit();
    }

    /// <inheritdoc />
    public async Task<TReply> AppendToStreamWithReply<T, TReply>(bool withCommit, string queueId, IEvent @event)
        where T : class, IIntegrationEvent where TReply : class
    {
        var body = JsonConvert.SerializeObject(@event);
        var integrationEvent = _integrationEventMapperStrategy.GetMapper<T>(@event)
            .Map(@event, body);

        IUnitOfWork uow = null;
        if (withCommit)
        {
            uow = _entityFactoryService.Create();
        }

        try
        {
            SaveToStore(integrationEvent);
            var result = await SendEventWithReplyAsync<T, TReply>(queueId, integrationEvent);
            uow?.Commit();

            return result;
        }
        finally
        {
            uow?.Dispose();
        }
    }

    private void SaveToStore<T>(params T[] integrationEvents) where T : class, IIntegrationEvent
    {
        try
        {
            var eventStream = _entityFactoryService.Create<T>();

            foreach (var integrationEvent in integrationEvents)
            {
                eventStream.Insert(integrationEvent);
            }
        }
        catch (Exception ex)
        {
            Log.SaveToStoreFailed(_logger, ex);
            throw;
        }
    }

    private void SendEvents<T>(string queueId, params T[] integrationEvents) where T : class, IIntegrationEvent
    {
        try
        {
            using (var sender = _messagingComponentFactory.CreateMessageSender<string>(queueId))
            {
                foreach (var @event in integrationEvents)
                {
                    if (!@event.IsSendable)
                    {
                        continue;
                    }

                    sender.SendMessage(@event.Body);
                }
            }
        }
        catch (Exception ex)
        {
            Log.SendEventsFailed(_logger, queueId, ex);
            throw;
        }
    }

    private async Task<TReply> SendEventWithReplyAsync<T, TReply>(string queueId, T integrationEvent)
        where T : class, IIntegrationEvent where TReply : class
    {
        try
        {
            using (var sender = _messagingComponentFactory.CreateMessageRpcSender<string, TReply>(queueId))
            {
                if (!integrationEvent.IsSendable)
                {
                    return await Task.FromResult<TReply>(null);
                }

                return await sender.SendMessageAndWaitResponseAsync(integrationEvent.Body);
            }
        }
        catch (Exception ex)
        {
            Log.SendWithReplyFailed(_logger, queueId, ex);
            throw;
        }
    }

    private static class Log
    {
        private static readonly Action<ILogger, Exception> saveToStoreFailed = LoggerMessage.Define(
            LogLevel.Error, EventIds.SaveToStoreFailed, "Error while save events to db.");

        private static readonly Action<ILogger, string, Exception> sendEventsFailed = LoggerMessage.Define<string>(
            LogLevel.Error, EventIds.SendEventsFailed, "Error while send events to queue {QueueId}.");

        private static readonly Action<ILogger, string, Exception> sendWithReplyFailed = LoggerMessage.Define<string>(
            LogLevel.Error, EventIds.SendEventWithReplyFailed,
            "Error while send events to queue {QueueId}.");

        public static void SaveToStoreFailed(ILogger logger, Exception ex)
        {
            saveToStoreFailed(logger, ex);
        }

        public static void SendEventsFailed(ILogger logger, string queueId, Exception ex)
        {
            sendEventsFailed(logger, queueId, ex);
        }

        public static void SendWithReplyFailed(ILogger logger, string queueId, Exception ex)
        {
            sendWithReplyFailed(logger, queueId, ex);
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    /// <summary>
    ///     Представляет идентификаторы событий ведения журнала, для <see cref="EventStore" />.
    /// </summary>
    public static class EventIds
    {
        /// <summary>
        ///     Возникает, если не удалось сохранить события в источник данных из-за необработанного исключения.
        /// </summary>
        public static readonly EventId SaveToStoreFailed = new(100, nameof(SaveToStoreFailed));

        /// <summary>
        ///     Возникает, если не удалось отправить события подписчикам из-за необработанного исключения.
        /// </summary>
        public static readonly EventId SendEventsFailed = new(101, nameof(SendEventsFailed));

        /// <summary>
        ///     Возникает, если не удалось отправить событие или дождаться ответа из-за необработанного исключения.
        /// </summary>
        public static readonly EventId SendEventWithReplyFailed = new(102, nameof(SendEventWithReplyFailed));
    }
}