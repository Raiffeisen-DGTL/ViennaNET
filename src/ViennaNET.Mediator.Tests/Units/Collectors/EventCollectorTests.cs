using NUnit.Framework;
using ViennaNET.Mediator.Collectors;
using ViennaNET.Mediator.Pipeline;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Mediator.Tests.Fake;
using ViennaNET.Mediator.Tests.Fake.Handlers;

namespace ViennaNET.Mediator.Tests.Units.Collectors
{
  [TestFixture(Category = "Unit", TestOf = typeof(EventCollector))]
  public class EventCollectorTests
  {
    private IEventCollectorFactory _eventCollector;

    [OneTimeSetUp]
    public void TestInit()
    {
      var preProcessor = new PreProcessorService();
      var mediator = new Mediator.Mediators.Mediator(preProcessor);
      mediator.Register(new IMessageHandler[]
      {
        new CommandReceiver(), new EventListener(), new OtherEventListener(),
        new RequestHandler()
      }, new IMessageHandlerAsync[]
      {
        new AsyncCommandReceiver(), new AsyncEventListener(), new OtherAsyncEventListener(),
        new AsyncRequestHandler()
      });
      _eventCollector =
        new EventCollectorFactory(mediator);
    }

    [Test]
    public void EnqueueTest()
    {
      Assert.That(() =>
      {
        using (var eventCollector = _eventCollector.Create())
        {
          eventCollector.Enqueue(new Event());
          eventCollector.Enqueue(new AlternateEvent());
        };
      }, Throws.Nothing);
    }

    [Test(ExpectedResult = 2)]
    public int EventsTest()
    {
      using (var eventCollector = _eventCollector.Create())
      {
        eventCollector.Enqueue(new Event());
        eventCollector.Enqueue(new AlternateEvent());
        return eventCollector.Events.Count;
      }
    }

    [Test]
    public void PublishAsyncTest()
    {
      Assert.That(async () =>
      {
        using (var eventCollector = _eventCollector.Create())
        {
          eventCollector.Enqueue(new Event());
          await eventCollector.PublishAsync();
        }
      }, Throws.Nothing);
    }

    [Test()]
    public void PublishTest()
    {
      Assert.That(() =>
      {
        using (var eventCollector = _eventCollector.Create())
        {
          eventCollector.Enqueue(new Event());
          eventCollector.Publish();
        }
      }, Throws.Nothing);
    }
  }
}
