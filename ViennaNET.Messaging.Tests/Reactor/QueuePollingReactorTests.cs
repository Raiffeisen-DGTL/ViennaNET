using NUnit.Framework;
using ViennaNET.Messaging.Processing.Impl.Poll;
using ViennaNET.Messaging.Tests.Unit.DSL;

namespace ViennaNET.Messaging.Tests.Unit.Reactor
{
  [TestFixture(Category = "Unit", TestOf = typeof(QueuePollingReactor))]
  class QueuePollingReactorTests
  {
    [Test]
    public void StartTest()
    {
      var reactor = Given.QueuePollingReactor.Please();

      reactor.StartProcessing();

      Assert.Pass();
    }

    [Test]
    public void StopTest()
    {
      var reactor = Given.QueuePollingReactor.Please();

      reactor.StartProcessing();
      reactor.Stop();

      Assert.Pass();
    }
  }
}