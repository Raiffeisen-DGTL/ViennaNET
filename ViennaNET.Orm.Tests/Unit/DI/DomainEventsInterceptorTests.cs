using ViennaNET.Mediator.Seedwork;
using ViennaNET.Orm.DI;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace ViennaNET.Orm.Tests.Unit.DI
{
  [TestFixture, Category("Unit"), TestOf(typeof(DomainEventsInterceptor))]
  public class DomainEventsInterceptorTests
  {
    [Test]
    public void OnLoad_EntityWithoutCollector_CollectorSet()
    {
      var eventCollectorMock = new Mock<IEventCollector>();
      var eventCollectorFactoryMock = new Mock<IEventCollectorFactory>();
      eventCollectorFactoryMock.Setup(x => x.Create())
                               .Returns(eventCollectorMock.Object);
      var entity = new Entity();
      var interceptor = new DomainEventsInterceptor(eventCollectorFactoryMock.Object);

      interceptor.OnLoad(entity, 0, null, null, null);

      Assert.That(entity.Collector == eventCollectorMock.Object);
    }

    private class Entity : IEntityEventPublisher
    {
      public IEventCollector Collector { get; private set; }

      public void SetCollector(IEventCollector eventCollector)
      {
        Collector = eventCollector;
      }

      public IReadOnlyCollection<IEvent> GetChanges()
      {
        throw new System.NotImplementedException();
      }
    }
  }
}
