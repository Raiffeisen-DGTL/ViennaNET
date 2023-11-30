using NUnit.Framework;
using ViennaNET.Messaging.Processing.Impl.Poll;
using ViennaNET.Messaging.Tests.DSL;

namespace ViennaNET.Messaging.Tests.Reactor
{
  [TestFixture(Category = "Unit", TestOf = typeof(QueuePollingReactor))]
  internal class QueuePollingReactorTests
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