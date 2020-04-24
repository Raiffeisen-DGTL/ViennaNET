using System;
using ViennaNET.Orm.Factories;
using Moq;
using NUnit.Framework;

namespace ViennaNET.Orm.Tests.Unit.Factories
{
  [TestFixture, Category("Unit"), TestOf(typeof(ExplicitNhSessionScope))]
  public class ExplicitNhSessionScopeTests
  {
    [Test]
    public void Ctor_SessionManagerSet_UnregisterUoWCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var scope = new ExplicitNhSessionScope(sessionManager.Object);

      sessionManager.Verify(x => x.UnregisterUoW(), Times.Once);
    }

    [Test]
    public void Dispose_CalledTwoTimes_CloseAllCalledOnce()
    {
      var countOfCallbackCalls = 0;
      var sessionManager = new Mock<ISessionManager>();
      var scope = new ExplicitNhSessionScope(sessionManager.Object);
      scope.Disposed += (s, e) => ++countOfCallbackCalls;

      scope.Dispose();
      scope.Dispose();

      sessionManager.Verify(x => x.CloseAll(), Times.Once);
      Assert.That(countOfCallbackCalls == 1);
    }

    [Test]
    public void Dispose_Exception_Handled()
    {
      var sessionManager = new Mock<ISessionManager>();
      sessionManager.Setup(x => x.CloseAll())
                    .Throws(new ArgumentException());
      var scope = new ExplicitNhSessionScope(sessionManager.Object);

      scope.Dispose();

      Assert.Pass();
    }
  }
}
