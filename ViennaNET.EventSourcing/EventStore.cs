using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ViennaNET.EventSourcing.EventMappers;
using ViennaNET.Logging;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Utils;
using Newtonsoft.Json;
using ViennaNET.Messaging.Factories;
using ViennaNET.Orm.Application;

namespace ViennaNET.EventSourcing
{
  /// <summary>
  ///   Реализация хранилища событий. Позволяет дополнительно отправлять сообщения во внешнюю очередь.
  /// </summary>
  public class EventStore : IEventStore
  {
    private readonly IEntityFactoryService _entityFactoryService;
    private readonly IIntegrationEventMapperFactory _integrationEventMapperStrategy;
    private readonly IMessagingComponentFactory _messagingComponentFactory;

    /// <summary>
    ///   Инициализирует хранилище зависимостями <see cref="IEntityFactoryService" />,
    ///   <see cref="IMessagingComponentFactory" />, <see cref="IIntegrationEventMapperFactory" />.
    /// </summary>
    /// <param name="entityFactoryService">Фабричный сервис для инфраструктурных операций с БД.</param>
    /// <param name="messagingComponentFactory">Фабрика компонентов для обмена сообщениями.</param>
    /// <param name="integrationEventMapperFactory">Фабрика подбора преобразователей событий.</param>
    public EventStore(
      IEntityFactoryService entityFactoryService, IMessagingComponentFactory messagingComponentFactory,
      IIntegrationEventMapperFactory integrationEventMapperFactory)
    {
      _entityFactoryService = entityFactoryService.ThrowIfNull(nameof(entityFactoryService));
      _messagingComponentFactory = messagingComponentFactory.ThrowIfNull(nameof(messagingComponentFactory));
      _integrationEventMapperStrategy = integrationEventMapperFactory.ThrowIfNull(nameof(integrationEventMapperFactory));
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
      var integrationEvents = events.Select(e =>
                                    {
                                      var body = JsonConvert.SerializeObject(e);
                                      return _integrationEventMapperStrategy.GetMapper<T>(e)
                                                                            .Map(e, body);
                                    })
                                    .ToArray();

      IUnitOfWork uow = null;
      if (withCommit)
      {
        uow = _entityFactoryService.Create();
      }

      try
      {
        SaveToStore(integrationEvents);
        uow?.Commit();
      }
      finally
      {
        uow?.Dispose();
      }
    }

    /// <inheritdoc />
    public void AppendToStream<T>(bool withCommit, string queueId, params IEvent[] events)
      where T : class, IIntegrationEvent
    {
      var integrationEvents = events.Select(e =>
                                    {
                                      var body = JsonConvert.SerializeObject(e);
                                      return _integrationEventMapperStrategy.GetMapper<T>(e)
                                                                            .Map(e, body);
                                    })
                                    .ToArray();

      IUnitOfWork uow = null;
      if (withCommit)
      {
        uow = _entityFactoryService.Create();
      }

      try
      {
        SaveToStore(integrationEvents);
        SendEvents(queueId, integrationEvents);
        uow?.Commit();
      }
      finally
      {
        uow?.Dispose();
      }
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
        Logger.LogErrorFormat(ex, "Error while save events to db");
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
        Logger.LogErrorFormat(ex, "Error while send events to queue");
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
        Logger.LogErrorFormat(ex, "Error while send events to queue");
        throw;
      }
    }
  }
}
