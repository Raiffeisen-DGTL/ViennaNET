using NUnit.Framework;
using ViennaNET.Messaging.Processing.Impl.Subscribe;
using ViennaNET.Messaging.Tests.Unit.DSL;

namespace ViennaNET.Messaging.Tests.Unit.Reactor
{
  [TestFixture(Category = "Unit", TestOf = typeof(QueueSubscribedReactor))]
  class QueueSubscribedReactorTests
  {
    [Test]
    public void StartTest()
    {
      var reactor = Given.QueueSubscribedReactor.Please();

      reactor.StartProcessing();

      Assert.Pass();
    }

    [Test]
    public void StopTest()
    {
      var reactor = Given.QueueSubscribedReactor.Please();

      reactor.StartProcessing();
      reactor.Stop();

      Assert.Pass();
    }
  }
}
