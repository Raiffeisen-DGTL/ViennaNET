using System;
using System.Data;
using System.Linq;
using ViennaNET.EventSourcing.EventMappers;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Messaging.Sending;
using ViennaNET.Orm.Seedwork;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Factories;
using ViennaNET.Orm.Application;

namespace ViennaNET.EventSourcing.Tests.Unit
{
  [TestFixture(Category = "Unit")]
  public class EventStoreTests
  {
    private static (EventStore, Mock<ISerializedMessageSender<string>>, Mock<IEntityFactoryService>) CreateEventStore(bool sendable)
    {
      var fakeEfs = new Mock<IEntityFactoryService>();
      var fakeEventRepo = new Mock<IEntityRepository<FakeIntegrationEvent>>();
      var fakeUow = new Mock<IUnitOfWork>();
      fakeEfs.Setup(x => x.Create(IsolationLevel.ReadCommitted, true, false))
             .Returns(fakeUow.Object);
      fakeEfs.Setup(x => x.Create<FakeIntegrationEvent>())
             .Returns(fakeEventRepo.Object);
      var fakeSenderFactory = new Mock<IMessagingComponentFactory>();
      var fakeSender = new Mock<ISerializedMessageSender<string>>();
      fakeSenderFactory.Setup(x => x.CreateMessageSender<string>(It.IsAny<string>()))
                       .Returns(fakeSender.Object);
      var fakeMappersStrategy = new Mock<IIntegrationEventMapperFactory>();
      fakeMappersStrategy.Setup(x => x.GetMapper<FakeIntegrationEvent>(It.IsAny<FakeEvent>()))
                         .Returns(new FakeMapper(sendable));
      var eventStore = new EventStore(fakeEfs.Object, fakeSenderFactory.Object, fakeMappersStrategy.Object);
      return (eventStore, fakeSender, fakeEfs);
    }

    internal class FakeMapper : IConcreteIntegrationEventMapper<FakeIntegrationEvent>
    {
      private readonly bool _isSendable;

      public FakeMapper(bool isSendable)
      {
        _isSendable = isSendable;
      }

      public FakeIntegrationEvent Map(IEvent @event, string body)
      {
        return new FakeIntegrationEvent()
        {
          Timestamp = DateTime.Now,
          Body = body,
          Id = 1,
          Initiator = ((FakeEvent)@event).Initiator,
          Type = 3,
          IsSendable = _isSendable
        };
      }

      public Type EventType => typeof(FakeEvent);
    }

    internal class FakeEvent : IEvent
    {
      public string Initiator { get; set; }
    }

    public class FakeIntegrationEvent : IIntegrationEvent
    {
      public int Id { get; set; }
      public int Type { get; set; }
      public DateTime Timestamp { get; set; }
      public string Initiator { get; set; }
      public string Body { get; set; }
      public bool IsSendable { get; set; }
    }

    [Test]
    public void AppendToStream_HasNoSendableEvents_NotSendedToQueue()
    {
      // arrange
      var (eventStore, fakeSender, fakeEfs) = CreateEventStore(false);
      var fakeEvent = new FakeEvent { Initiator = "some content", };

      // act
      eventStore.AppendToStream<FakeIntegrationEvent>(true, "", fakeEvent);

      // assert
      fakeSender.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void AppendToStream_HasSendableEvents_SendedToQueue()
    {
      // arrange
      var (eventStore, fakeSender, fakeEfs) = CreateEventStore(true);
      var fakeEvent = new FakeEvent { Initiator = "some content", };

      // act
      eventStore.AppendToStream<FakeIntegrationEvent>(true, "", fakeEvent);

      // assert
      fakeSender.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void AppendToStreamWithoutQueueId_HasSendableEvents_SendedToQueueNotCalled()
    {
      // arrange
      var (eventStore, fakeSender, fakeEfs) = CreateEventStore(true);
      var fakeEvent = new FakeEvent { Initiator = "some initiator", };

      // act
      eventStore.AppendToStream<FakeIntegrationEvent>(true, fakeEvent);

      // assert
      fakeSender.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void AppendToStream_WithoutCommit_UowNotCreated()
    {
      // arrange
      var (eventStore, fakeSender, fakeEfs) = CreateEventStore(true);
      var fakeEvent = new FakeEvent { Initiator = "some initiator", };

      // act
      eventStore.AppendToStream<FakeIntegrationEvent>(false, fakeEvent);

      // assert
      fakeEfs.Verify(x => x.Create(It.IsAny<IsolationLevel>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public void LoadFromStream_FilterByInitiator_EventsHasBeenFiltered()
    {
      // arrange
      var fakeEfs = new Mock<IEntityFactoryService>();
      var fakeEventRepo = new Mock<IEntityRepository<FakeIntegrationEvent>>();
      const string initiatorBezrukikh = "user1";
      const string initiatorWithGoldenHands = "user2";
      fakeEventRepo.Setup(x => x.Query())
                   .Returns(new[]
                   {
                     new FakeIntegrationEvent { Initiator = initiatorBezrukikh, Timestamp = new DateTime(2014, 1, 1) },
                     new FakeIntegrationEvent { Initiator = initiatorWithGoldenHands, Timestamp = new DateTime(2015, 1, 1) }
                   }.AsQueryable());
      var fakeUow = new Mock<IUnitOfWork>();
      fakeEfs.Setup(x => x.Create(IsolationLevel.ReadCommitted, true, false))
             .Returns(fakeUow.Object);
      fakeEfs.Setup(x => x.Create<FakeIntegrationEvent>())
             .Returns(fakeEventRepo.Object);
      var fakeSenderFactory = new Mock<IMessagingComponentFactory>();
      var fakeSender = new Mock<ISerializedMessageSender<string>>();
      var fakeMappersStrategy = new Mock<IIntegrationEventMapperFactory>();
      var eventStore = new EventStore(fakeEfs.Object, fakeSenderFactory.Object, fakeMappersStrategy.Object);

      // act
      var ev = eventStore.LoadFromStream<FakeIntegrationEvent>(x => x.Initiator == initiatorWithGoldenHands)
                         .ToList();

      // assert
      Assert.That(ev.Count == 1);
      Assert.That(ev.First()
                    .Initiator == initiatorWithGoldenHands);
    }

    [Test]
    public void LoadFromStream_NoFilter_EventsHasBeenSortedByTimestamp()
    {
      // arrange
      var fakeEfs = new Mock<IEntityFactoryService>();
      var fakeEventRepo = new Mock<IEntityRepository<FakeIntegrationEvent>>();
      const string initiatorBezrukikh = "user1";
      const string initiatorWithGoldenHands = "user2";
      fakeEventRepo.Setup(x => x.Query())
                   .Returns(new[]
                   {
                     new FakeIntegrationEvent { Initiator = initiatorWithGoldenHands, Timestamp = new DateTime(2015, 1, 1) },
                     new FakeIntegrationEvent { Initiator = initiatorBezrukikh, Timestamp = new DateTime(2014, 1, 1) }
                   }.AsQueryable());
      var fakeUow = new Mock<IUnitOfWork>();
      fakeEfs.Setup(x => x.Create(IsolationLevel.ReadCommitted, true, false))
             .Returns(fakeUow.Object);
      fakeEfs.Setup(x => x.Create<FakeIntegrationEvent>())
             .Returns(fakeEventRepo.Object);
      var fakeSenderFactory = new Mock<IMessagingComponentFactory>();
      var fakeMappersStrategy = new Mock<IIntegrationEventMapperFactory>();
      var eventStore = new EventStore(fakeEfs.Object, fakeSenderFactory.Object, fakeMappersStrategy.Object);

      // act
      var ev = eventStore.LoadFromStream<FakeIntegrationEvent>(x => true)
                         .ToList();

      // assert
      Assert.That(ev.Count == 2);
      Assert.That(ev.First()
                    .Initiator == initiatorBezrukikh);
      Assert.That(ev.Last()
                    .Initiator == initiatorWithGoldenHands);
    }
  }
}
