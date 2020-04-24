using System.Linq;
using ViennaNET.Orm.Factories;
using Moq;
using NHibernate;
using NUnit.Framework;

namespace ViennaNET.Orm.Tests.Unit.Factories
{
  [TestFixture, Category("Unit"), TestOf(typeof(SessionFactoryManager))]
  public class SessionFactoryManagerTests
  {
    private const string Nick = "default";

    private static ISessionFactoryManager GetSessionManager(
      out Mock<ISessionFactory> sessionFactory, out Mock<ISessionFactoryProvidersManager> sessionFactoryProvidersManager)
    {
      sessionFactory = new Mock<ISessionFactory>();
      var sessionFactoryProvider = new Mock<ISessionFactoryProvider>();
      sessionFactoryProvider.Setup(x => x.GetSessionFactory())
                            .Returns(sessionFactory.Object);
      sessionFactoryProvidersManager = new Mock<ISessionFactoryProvidersManager>();
      sessionFactoryProvidersManager.Setup(x => x.GetSessionFactoryProvider(Nick))
                                    .Returns(sessionFactoryProvider.Object);
      return new SessionFactoryManager(sessionFactoryProvidersManager.Object);
    }

    [Test]
    public void GetSessionFactories_HasSessionFactoryProvider_HasOneCachedSessionFactory()
    {
      var sessionManager = GetSessionManager(out var sessionFactory, out _);

      sessionManager.GetSessionFactory(Nick);
      var factories = sessionManager.GetSessionFactories()
                                     .ToList();

      Assert.That(factories.Count == 1);
      Assert.That(factories.First() == sessionFactory.Object);
    }

    [Test]
    public void GetSessionFactory_CalledTwice_OneCallToGetSessionFactoryProvider()
    {
      var sessionManager = GetSessionManager(out _, out var sessionFactoryProvidersManager);

      sessionManager.GetSessionFactory(Nick);
      sessionManager.GetSessionFactory(Nick);

      sessionFactoryProvidersManager.Verify(x => x.GetSessionFactoryProvider(Nick), Times.Once);
    }

    [Test]
    public void GetSessionFactory_HasSessionFactoryProvider_SessionFactoryCreated()
    {
      var sessionManager = GetSessionManager(out var sessionFactory, out _);

      var factory = sessionManager.GetSessionFactory(Nick);

      Assert.That(sessionFactory.Object == factory);
    }
  }
}
