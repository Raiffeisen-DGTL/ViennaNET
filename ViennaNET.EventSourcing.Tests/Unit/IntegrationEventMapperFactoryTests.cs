using System;
using System.Collections.Generic;
using ViennaNET.EventSourcing.EventMappers;
using ViennaNET.EventSourcing.Exceptions;
using ViennaNET.Mediator.Seedwork;
using NUnit.Framework;
using ViennaNET.Orm.Application;

namespace ViennaNET.EventSourcing.Tests.Unit
{
  [TestFixture(Category = "Unit")]
  public class IntegrationEventMapperFactoryTests
  {
    [Test]
    public void GetMapper_MapperExists_ReturnsMapper()
    {
      // arrange
      var fakeMappers = new List<IIntegrationEventMapper>() { new FirstMapper(), new SecondMapper() };
      IEvent fakeEvent = new SecondEvent();

      // act
      var strategy = new IntegrationEventMapperFactory(fakeMappers);
      var mapper = strategy.GetMapper<SecondIntegrationEvent>(fakeEvent);

      // assert
      Assert.That(mapper, Is.InstanceOf(typeof(SecondMapper)));
    }

    [Test]
    public void GetMapper_MapperNotExists_ThrowsException()
    {
      // arrange
      var fakeMappers = new List<IIntegrationEventMapper>() { new FirstMapper() };
      IEvent fakeEvent = new SecondEvent();

      // act
      var strategy = new IntegrationEventMapperFactory(fakeMappers);

      // assert
      Assert.Throws<IntegrationEventMapperNotFoundException>(() => strategy.GetMapper<SecondIntegrationEvent>(fakeEvent));
    }

    private class FirstMapper : IConcreteIntegrationEventMapper<FirstIntegrationEvent>
    {
      public Type EventType => typeof(FirstEvent);

      public FirstIntegrationEvent Map(IEvent @event, string body)
      {
        return new FirstIntegrationEvent();
      }
    }

    private class SecondMapper : IConcreteIntegrationEventMapper<SecondIntegrationEvent>
    {
      public SecondIntegrationEvent Map(IEvent @event, string body)
      {
        return new SecondIntegrationEvent();
      }

      public Type EventType => typeof(SecondEvent);
    }

    private class FirstEvent : IEvent
    {
    }

    private class SecondEvent : IEvent
    {
    }

    private class FirstIntegrationEvent : IIntegrationEvent
    {
      public int Id { get; }
      public int Type { get; }
      public DateTime Timestamp { get; }
      public string Initiator { get; }
      public string Body { get; }
      public bool IsSendable { get; }
    }

    private class SecondIntegrationEvent : IIntegrationEvent
    {
      public int Id { get; }
      public int Type { get; }
      public DateTime Timestamp { get; }
      public string Initiator { get; }
      public string Body { get; }
      public bool IsSendable { get; }
    }
  }
}
