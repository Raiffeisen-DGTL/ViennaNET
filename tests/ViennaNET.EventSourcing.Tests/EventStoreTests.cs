using System.Data;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ViennaNET.EventSourcing.EventMappers;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Messaging.Factories;
using ViennaNET.Messaging.Sending;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.EventSourcing.Tests;

internal record FakeEvent(string Initiator) : IEvent;

internal class FakeMapper : IConcreteIntegrationEventMapper<FakeIntegrationEvent>
{
    private readonly bool _isSendable;

    public FakeMapper(bool isSendable)
    {
        _isSendable = isSendable;
    }

    public FakeIntegrationEvent Map(IEvent @event, string body)
    {
        return new FakeIntegrationEvent(1, 3, DateTime.Now, ((FakeEvent)@event).Initiator, body, _isSendable);
    }

    public Type EventType => typeof(FakeEvent);
}

public record FakeIntegrationEvent(int Id, int Type, DateTime Timestamp, string Initiator, string Body, bool IsSendable)
    : IIntegrationEvent;

[TestFixture(Category = "Unit", TestOf = typeof(EventStore))]
public class EventStoreTests
{
    [SetUp]
    public void CreateEventStore()
    {
        _efServiceMock = new Mock<IEntityFactoryService>();
        _senderMock = new Mock<ISerializedMessageSender<string>>();
        _msgComponentFactoryMock = new Mock<IMessagingComponentFactory>();
        _mapperFactoryMock = new Mock<IIntegrationEventMapperFactory>();
        _repositoryMock = new Mock<IEntityRepository<FakeIntegrationEvent>>();
        _loggerMock = new Mock<ILogger<EventStore>>();

        var logger = _loggerMock.Object;
        var uot = Mock.Of<IUnitOfWork>();
        var repository = _repositoryMock.Object;
        var sender = _senderMock.Object;
        var mapperFactory = _mapperFactoryMock.Object;
        var msgCompFactory = _msgComponentFactoryMock.Object;
        var entityFactoryService = _efServiceMock.Object;

        _efServiceMock.Setup(x => x.Create(IsolationLevel.ReadCommitted, true, false)).Returns(uot);
        _efServiceMock.Setup(x => x.Create<FakeIntegrationEvent>()).Returns(repository);
        _msgComponentFactoryMock.Setup(x => x.CreateMessageSender<string>(It.IsAny<string>())).Returns(sender);
        _store = new EventStore(logger, entityFactoryService, msgCompFactory, mapperFactory);
    }

    private Mock<ISerializedMessageSender<string>> _senderMock = null!;
    private Mock<IEntityFactoryService> _efServiceMock = null!;
    private Mock<IMessagingComponentFactory> _msgComponentFactoryMock = null!;
    private Mock<IIntegrationEventMapperFactory> _mapperFactoryMock = null!;
    private Mock<IEntityRepository<FakeIntegrationEvent>> _repositoryMock = null!;
    private Mock<ILogger<EventStore>> _loggerMock = null!;
    private EventStore _store = null!;

    [Test]
    public void Ctor_EventStore_Without_Logger_Throws_ArgumentNullException()
    {
        var efService = _efServiceMock.Object;
        var msgCompFactory = _msgComponentFactoryMock.Object;
        var mapperFactory = _mapperFactoryMock.Object;

        Assert.That(() => new EventStore(null!, efService, msgCompFactory, mapperFactory),
            Throws.ArgumentNullException.And.Message.EqualTo("Value cannot be null. (Parameter 'logger')"));
    }

    [Test]
    public void AppendToStream_Without_UoW_Throws_Nothing()
    {
        var @event = new FakeEvent("some content");

        _efServiceMock.Setup(x => x.Create(It.IsAny<IsolationLevel>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns((IUnitOfWork)null!);
        _mapperFactoryMock.Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>()))
            .Returns(new FakeMapper(true));

        Assert.That(() => _store.AppendToStream<FakeIntegrationEvent>(false, @event), Throws.Nothing);
    }

    [Test]
    public void AppendToStream_With_QueueId_And_Without_UoW_Throws_Nothing()
    {
        var @event = new FakeEvent("some content");

        _efServiceMock.Setup(x => x.Create(It.IsAny<IsolationLevel>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns((IUnitOfWork)null!);
        _mapperFactoryMock.Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>()))
            .Returns(new FakeMapper(true));

        Assert.That(() => _store.AppendToStream<FakeIntegrationEvent>(false, "test-queue-id", @event), Throws.Nothing);
    }

    [Test]
    public void SaveToStoreThrowsExceptionAndAsResultExWasLoggedAndReThrows()
    {
        var @event = new FakeEvent("some content");

        _loggerMock.Setup(logger => logger.IsEnabled(LogLevel.Error)).Returns(true);
        _repositoryMock.Setup(repository => repository.Insert(It.IsAny<FakeIntegrationEvent>()))
            .Throws(new InvalidOperationException());
        _mapperFactoryMock.Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>()))
            .Returns(new FakeMapper(false));

        Assert.Multiple(() =>
        {
            Assert.That(() => _store.AppendToStream<FakeIntegrationEvent>(true, "test-queue-id", @event),
                Throws.InvalidOperationException);

            _loggerMock.Verify(logger => logger.Log(LogLevel.Error, EventStore.EventIds.SaveToStoreFailed,
                It.IsAny<It.IsValueType>(), It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsValueType, Exception?, string>>()), Times.Once);
        });
    }

    [Test]
    public void SendEventsThrowsExceptionAndAsResultExWasLoggedAndReThrows()
    {
        var @event = new FakeEvent("some content");

        _loggerMock.Setup(logger => logger.IsEnabled(LogLevel.Error)).Returns(true);
        _senderMock.Setup(sender => sender.SendMessage(It.IsAny<string>(), It.IsAny<string>()))
            .Throws<InvalidOperationException>();
        _mapperFactoryMock.Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>()))
            .Returns(new FakeMapper(true));

        Assert.Multiple(() =>
        {
            Assert.That(() => _store.AppendToStream<FakeIntegrationEvent>(true, "test-queue-id", @event),
                Throws.InvalidOperationException);

            _loggerMock.Verify(logger => logger.Log(LogLevel.Error, EventStore.EventIds.SendEventsFailed,
                It.IsAny<It.IsValueType>(), It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsValueType, Exception?, string>>()), Times.Once);
        });
    }

    [Test]
    public void SendEventWithReplyAsyncThrowsExceptionAndAsResultExWasLoggedAndReThrows()
    {
        var @event = new FakeEvent("some content");
        var mapper = new FakeMapper(true);
        var senderMock = new Mock<ISerializedMessageRpcSender<string, string>>();

        senderMock
            .Setup(sender => sender.SendMessageAndWaitResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws<InvalidOperationException>();
        _msgComponentFactoryMock
            .Setup(x => x.CreateMessageRpcSender<string, string>(It.IsAny<string>()))
            .Returns(senderMock.Object);
        _loggerMock.Setup(logger => logger.IsEnabled(LogLevel.Error)).Returns(true);
        _mapperFactoryMock.Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>())).Returns(mapper);


        Assert.Multiple(() =>
        {
            Assert.That(
                () => _store.AppendToStreamWithReply<FakeIntegrationEvent, string>(true, "test-queue-id", @event),
                Throws.InvalidOperationException);

            _loggerMock.Verify(logger => logger.Log(LogLevel.Error, EventStore.EventIds.SendEventWithReplyFailed,
                It.IsAny<It.IsValueType>(), It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsValueType, Exception?, string>>()), Times.Once);
        });
    }

    [Test]
    public void AppendToStreamWithReplyNotSendableEventAndAsResultWasNotSendToQueue()
    {
        var @event = new FakeEvent("some content");
        var mapper = new FakeMapper(false);
        var senderMock = new Mock<ISerializedMessageRpcSender<string, string>>();

        _mapperFactoryMock.Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>())).Returns(mapper);
        _msgComponentFactoryMock.Setup(x => x.CreateMessageRpcSender<string, string>(It.IsAny<string>()))
            .Returns(senderMock.Object);

        _store.AppendToStreamWithReply<FakeIntegrationEvent, string>(true, "test-queue-id", @event);

        senderMock.Verify(x => x.SendMessageAndWaitResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public void AppendToStreamNotSendableEventAndAsResultWasNotSendToQueue()
    {
        _mapperFactoryMock
            .Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>()))
            .Returns(new FakeMapper(false));

        _store.AppendToStream<FakeIntegrationEvent>(true, "test-queue-id", new FakeEvent("some content"));

        _senderMock.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void AppendToStreamSendableEventAndAsResultWasSendToQueue()
    {
        _mapperFactoryMock
            .Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>()))
            .Returns(new FakeMapper(true));

        _store.AppendToStream<FakeIntegrationEvent>(true, "test-queue-id", new FakeEvent("some content"));

        _senderMock.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void AppendToStreamWithoutQueueIdAndAsResultEventWasNotSentToQueue()
    {
        _mapperFactoryMock
            .Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>()))
            .Returns(new FakeMapper(true));

        _store.AppendToStream<FakeIntegrationEvent>(true, new FakeEvent("some content"));

        _senderMock.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void AppendToStreamWithoutCommitAndAsResultUoWWasNotCreated()
    {
        _mapperFactoryMock
            .Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>()))
            .Returns(new FakeMapper(true));

        _store.AppendToStream<FakeIntegrationEvent>(false, new FakeEvent("some content"));

        _efServiceMock.Verify(x => x.Create(It.IsAny<IsolationLevel>(), It.IsAny<bool>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Test]
    public void LoadFromStream_FilterByInitiator_EventsHasBeenFiltered()
    {
        const string user1 = "user1";
        const string user2 = "user2";

        var events = new[]
        {
            new FakeIntegrationEvent(1, 2, new DateTime(2014, 1, 1), user1, string.Empty, false),
            new FakeIntegrationEvent(1, 2, new DateTime(2015, 1, 1), user2, string.Empty, false)
        };

        _repositoryMock.Setup(x => x.Query()).Returns(events.AsQueryable());

        var ev = _store.LoadFromStream<FakeIntegrationEvent>(x => x.Initiator == user2).ToList();

        Assert.That(ev, Has.Count.EqualTo(1).And.ItemAt(0).EqualTo(events[1]));
    }

    [Test]
    public void LoadFromStream_NoFilter_EventsHasBeenSortedByTimestamp()
    {
        const string user1 = "user1";
        const string user2 = "user2";

        var events = new[]
        {
            new FakeIntegrationEvent(1, 2, new DateTime(2015, 1, 1), user2, string.Empty, false),
            new FakeIntegrationEvent(1, 2, new DateTime(2014, 1, 1), user1, string.Empty, false)
        };

        _repositoryMock.Setup(x => x.Query()).Returns(events.AsQueryable());

        var ev = _store.LoadFromStream<FakeIntegrationEvent>(x => true).ToList();

        Assert.That(ev, Has.Count.EqualTo(2).And.ItemAt(0).EqualTo(events[1]).And.ItemAt(1).EqualTo(events[0]));
    }
}