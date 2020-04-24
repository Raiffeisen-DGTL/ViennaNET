using System;
using ViennaNET.Orm.DI;
using ViennaNET.Orm.Factories;
using Moq;
using NHibernate;
using NUnit.Framework;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Tests.Unit.Factories
{
  [TestFixture, Category("Unit"), TestOf(nameof(EntityFactoryService))]
  public class EntityFactoryServiceTests
  {
    private static Mock<IApplicationContextProvider> GetApplicationContextProvider()
    {
      var applicationContextProvider = new Mock<IApplicationContextProvider>();
      applicationContextProvider.Setup(x => x.GetNickForCommand(It.IsAny<Type>()))
                                .Returns("default");
      return applicationContextProvider;
    }

    private static EntityFactoryService GetEntityFactoryServiceForQuery(Mock<ISession> session)
    {
      var applicationContextProvider = new Mock<IApplicationContextProvider>();
      applicationContextProvider.Setup(x => x.GetNick(It.IsAny<Type>()))
                                .Returns("default");

      var sessionManagerProvider = GetSessionManagerProvider(session);

      var entityFactoryService = new EntityFactoryService(sessionManagerProvider.Object, new Mock<ISessionFactoryManager>().Object,
                                                          applicationContextProvider.Object);
      return entityFactoryService;
    }

    private static Mock<ISessionManagerProvider> GetSessionManagerProvider(Mock<ISession> session)
    {
      var sessionManager = new Mock<ISessionManager>();
      sessionManager.Setup(x => x.GetSession(It.IsAny<string>()))
                    .Returns(session.Object);
      sessionManager.Setup(x => x.RegisterUoW(It.IsAny<IUnitOfWork>()))
                    .Returns(true);
      var sessionManagerProvider = new Mock<ISessionManagerProvider>();
      sessionManagerProvider.Setup(x => x.GetSessionManager())
                            .Returns(sessionManager.Object);
      return sessionManagerProvider;
    }

    [Test]
    public void Create_HasType_RepositoryCreated()
    {
      var applicationContextProvider = GetApplicationContextProvider();

      var sessionManagerProvider = GetSessionManagerProvider(new Mock<ISession>());

      var entityFactoryService = new EntityFactoryService(sessionManagerProvider.Object, new Mock<ISessionFactoryManager>().Object,
                                                          applicationContextProvider.Object);

      var repository = entityFactoryService.Create<object>();

      Assert.Pass();
    }

    [Test]
    public void Create_NoHasType_Exception()
    {
      const string message = "No nick for the type";

      var applicationContextProvider = new Mock<IApplicationContextProvider>();
      applicationContextProvider.Setup(x => x.GetNick(It.IsAny<Type>()))
                                .Throws(new ArgumentException(message));

      var sessionManagerProvider = GetSessionManagerProvider(new Mock<ISession>());

      var entityFactoryService = new EntityFactoryService(sessionManagerProvider.Object, new Mock<ISessionFactoryManager>().Object,
                                                          applicationContextProvider.Object);

      Assert.Throws<ArgumentException>(() => entityFactoryService.Create<object>(), message);
    }

    [Test]
    public void CreateCommandExecutor_HasType_RepositoryCreated()
    {
      var applicationContextProvider = GetApplicationContextProvider();

      var sessionManagerProvider = GetSessionManagerProvider(new Mock<ISession>());

      var entityFactoryService = new EntityFactoryService(sessionManagerProvider.Object, new Mock<ISessionFactoryManager>().Object,
                                                          applicationContextProvider.Object);

      var executor = entityFactoryService.CreateCommandExecutor<TestCommand>();

      Assert.Pass();
    }

    [Test]
    public void CreateCommandExecutor_NoHasType_Exception()
    {
      const string message = "No nick for the command";

      var applicationContextProvider = new Mock<IApplicationContextProvider>();
      applicationContextProvider.Setup(x => x.GetNickForCommand(It.IsAny<Type>()))
                                .Throws(new ArgumentException(message));

      var sessionManagerProvider = GetSessionManagerProvider(new Mock<ISession>());

      var entityFactoryService = new EntityFactoryService(sessionManagerProvider.Object, new Mock<ISessionFactoryManager>().Object,
                                                          applicationContextProvider.Object);

      Assert.Throws<ArgumentException>(() => entityFactoryService.CreateCommandExecutor<TestCommand>(), message);
    }

    [Test]
    public void CreateUnitOfWork_NoExceptions_UnitOfWorkCreated()
    {
      var applicationContextProvider = GetApplicationContextProvider();

      var sessionManagerProvider = GetSessionManagerProvider(new Mock<ISession>());

      var entityFactoryService = new EntityFactoryService(sessionManagerProvider.Object, new Mock<ISessionFactoryManager>().Object,
                                                          applicationContextProvider.Object);

      var uow = entityFactoryService.Create();

      Assert.Pass();
    }

    [Test]
    public void GetByNameSingle_NoExceptions_GetNamedQueryCalled()
    {
      var session = new Mock<ISession>();
      var query = new Mock<IQuery>();
      session.Setup(x => x.GetNamedQuery("query"))
             .Returns(query.Object);

      var entityFactoryService = GetEntityFactoryServiceForQuery(session);

      var result = entityFactoryService.GetByNameSingle("query");

      session.Verify(x => x.GetNamedQuery("query"), Times.Once);
      query.Verify(x => x.UniqueResult(), Times.Once);
    }

    [Test]
    public void GetByNameSingleT_NoExceptions_GetNamedQueryCalled()
    {
      var session = new Mock<ISession>();
      var query = new Mock<IQuery>();
      session.Setup(x => x.GetNamedQuery("query"))
             .Returns(query.Object);

      var entityFactoryService = GetEntityFactoryServiceForQuery(session);

      var result = entityFactoryService.GetByNameSingle<object>("query");

      session.Verify(x => x.GetNamedQuery("query"), Times.Once);
      query.Verify(x => x.UniqueResult<object>(), Times.Once);
    }

    [Test]
    public void GetScopedSession_CreateRepository_SessionManagerGotFromScope()
    {
      var applicationContextProvider = GetApplicationContextProvider();

      var sessionManagerProvider = new Mock<ISessionManagerProvider>();

      var sessionFactoryManager = new Mock<ISessionFactoryManager>();
      var sessionFactory = new Mock<ISessionFactory>();
      sessionFactory.Setup(x => x.OpenSession())
                    .Returns(new Mock<ISession>().Object);
      sessionFactoryManager.Setup(x => x.GetSessionFactory(It.IsAny<string>()))
                           .Returns(sessionFactory.Object);

      var entityFactoryService = new EntityFactoryService(sessionManagerProvider.Object, sessionFactoryManager.Object,
                                                          applicationContextProvider.Object);

      using (entityFactoryService.GetScopedSession())
      {
        var repository = entityFactoryService.Create<object>();
      }

      sessionManagerProvider.Verify(x => x.GetSessionManager(), Times.Never);
    }
  }
}
