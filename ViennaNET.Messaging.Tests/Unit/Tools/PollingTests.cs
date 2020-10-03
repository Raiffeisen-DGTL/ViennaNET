using System;
using System.Threading.Tasks;
using NUnit.Framework;
using ViennaNET.Messaging.Processing.Impl;

namespace ViennaNET.Messaging.Tests.Unit.Tools
{
  [TestFixture(Category = "Unit", TestOf = typeof(Polling))]
  class PollingTests
  {
    [Test]
    public void StartTest()
    {
      var p = new Polling(1000, ct => Task.FromResult<bool>(false));

      p.StartPolling();

      Assert.IsTrue(p.IsStarted);
    }

    [Test]
    public void StopTest()
    {
      var p = new Polling(1000, ct => Task.FromResult(false));

      p.StartPolling();
      p.Dispose();

      Assert.False(p.IsStarted);
    }

    [Test]
    public void ExceptionTest()
    {
      var p = new Polling(1000, ct => throw new ApplicationException());

      p.StartPolling();

      Assert.IsTrue(p.IsStarted);
    }

    [Test]
    public void StartTwiceTest()
    {
      var p = new Polling(1000, ct => Task.FromResult(false));

      p.StartPolling();
      p.StartPolling();

      Assert.IsTrue(p.IsStarted);
    }

    [Test]
    public void NullActionTest()
    {
      Assert.Throws<ArgumentNullException>(() => new Polling(1000, null));
    }
  }
}
