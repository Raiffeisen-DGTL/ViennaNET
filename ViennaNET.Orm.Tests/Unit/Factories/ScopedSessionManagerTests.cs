using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Orm.Factories;
using ViennaNET.Orm.Repositories;
using Moq;
using NHibernate;
using NUnit.Framework;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Tests.Unit.Factories
{
  [TestFixture, Category("Unit"), TestOf(nameof(ScopedSessionManager))]
  public class ScopedSessionManagerTests
  {
    [Test]
    public void GetSession_NoUnitOfWork_ReadonlySessionCreated()
    {
      var manager = GetSessionFactoryManager(out var session);

      var newSession = manager.GetSession("nick");

      Assert.Multiple(() =>
      {
        Assert.That(session.Object == newSession);
        Assert.That(newSession.DefaultReadOnly);
      });
    }

    [Test]
    public void GetSession_HasUnitOfWork_MutableSessionCreated()
    {
      var manager = GetSessionFactoryManager(out var session);

      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();
      manager.RegisterUoW(uow.Object);
      var newSession = manager.GetSession("nick");

      Assert.Multiple(() =>
      {
        Assert.That(session.Object == newSession);
        Assert.That(!newSession.DefaultReadOnly);
        session.Verify(x => x.BeginTransaction(It.IsAny<IsolationLevel>()));
      });
    }

    [Test]
    public void RegisterUoW_SetTwice_OnceSet()
    {
      var manager = GetSessionFactoryManager(out _);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();

      var result = manager.RegisterUoW(uow.Object);
      
      Assert.That(result);

      result = manager.RegisterUoW(uow.Object);

      Assert.That(!result);
    }

    [Test]
    public void UnregisterUoW_UoWRegistered_RemovesCurrentUoW()
    {
      var manager = GetSessionFactoryManager(out _);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();

      manager.RegisterUoW(uow.Object);
      manager.UnregisterUoW();
      var result = manager.RegisterUoW(uow.Object);

      Assert.That(result);
    }

    [Test]
    public void CloseAll_NoSessions_NothingDisposed()
    {
      var manager = GetSessionFactoryManager(out var session);

      manager.CloseAll();
      
      session.Verify(x => x.Dispose(), Times.Never);
    }

    [Test]
    public void CloseAll_HasSession_DisposeCalled()
    {
      var manager = GetSessionFactoryManager(out var session);
      manager.GetSession("nick");

      manager.CloseAll();

      session.Verify(x => x.Dispose(), Times.Once);
    }

    [Test]
    public void CloseAll_UoWRegistered_RemovesCurrentUoW()
    {
      var manager = GetSessionFactoryManager(out _);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();

      manager.RegisterUoW(uow.Object);
      manager.CloseAll();
      var result = manager.RegisterUoW(uow.Object);

      Assert.That(result);
    }

    [Test]
    public void CommitAll_TransactionInactive_CommitDidNotCall()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(false);
      var manager = GetSessionFactoryManager(out _, transaction.Object);
      manager.GetSession("nick");

      manager.CommitAll();

      transaction.Verify(x => x.Commit(), Times.Never);
    }

    [Test]
    public void CommitAll_TransactionActive_CommitCalled()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(true);
      var manager = GetSessionFactoryManager(out _, transaction.Object);
      manager.GetSession("nick");

      manager.CommitAll();

      transaction.Verify(x => x.Commit(), Times.Once);
    }

    [Test]
    public async Task CommitAllAsync_TransactionInactive_CommitDidNotCall()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(false);
      var manager = GetSessionFactoryManager(out _, transaction.Object);
      manager.GetSession("nick");

      await Task.WhenAll(manager.CommitAllAsync(default(CancellationToken)));

      transaction.Verify(x => x.CommitAsync(default(CancellationToken)), Times.Never);
    }

    [Test]
    public async Task CommitAllAsync_TransactionActive_CommitCalled()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(true);
      var manager = GetSessionFactoryManager(out _, transaction.Object);
      manager.GetSession("nick");

      await Task.WhenAll(manager.CommitAllAsync(default(CancellationToken)));

      transaction.Verify(x => x.CommitAsync(default(CancellationToken)), Times.Once);
    }

    [Test]
    public void SaveAll_HasNoSessions_SaveDidNotCall()
    {
      var manager = GetSessionFactoryManager(out var session);

      manager.SaveAll();

      session.Verify(x => x.Flush(), Times.Never);
    }

    [Test]
    public void SaveAll_HasSessions_SaveCalled()
    { 
      var manager = GetSessionFactoryManager(out var session);
      manager.GetSession("nick");

      manager.SaveAll();

      session.Verify(x => x.Flush(), Times.Once);
    }

    [Test]
    public async Task SaveAllAsync_HasNoSessions_SaveDidNotCall()
    {
      var manager = GetSessionFactoryManager(out var session);

      await Task.WhenAll(manager.SaveAllAsync(default(CancellationToken)));

      session.Verify(x => x.FlushAsync(default(CancellationToken)), Times.Never);
    }

    [Test]
    public async Task SaveAllAsync_HasSessions_SaveCalled()
    {
      var manager = GetSessionFactoryManager(out var session);
      manager.GetSession("nick");

      await Task.WhenAll(manager.SaveAllAsync(default(CancellationToken)));

      session.Verify(x => x.FlushAsync(default(CancellationToken)), Times.Once);
    }

    [Test]
    public void StartTransactionAll_HasNoSessions_BeginTransactionDidNotCall()
    {
      var manager = GetSessionFactoryManager(out var session);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();
      manager.RegisterUoW(uow.Object);
      manager.GetSession("nick");

      manager.StartTransactionAll();

      session.Verify(x => x.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Exactly(2));
    }

    [Test]
    public void StartTransactionAll_HasSessions_BeginTransactionCalled()
    {
      var manager = GetSessionFactoryManager(out var session);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();
      manager.RegisterUoW(uow.Object);

      manager.StartTransactionAll();

      session.Verify(x => x.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Never);
    }

    [Test]
    public void RollbackAll_HasNoSessions_NothingCalled()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(true);
      var manager = GetSessionFactoryManager(out var session, transaction.Object);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();
      manager.RegisterUoW(uow.Object);

      manager.RollbackAll(null);

      Assert.Multiple(() =>
      {
        transaction.Verify(x => x.Rollback(), Times.Never);
        session.Verify(x => x.Dispose(), Times.Never);
      });
    }

    [Test]
    public void RollbackAll_TransactionIsActive_RollbackCalled()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(true);
      var manager = GetSessionFactoryManager(out var session, transaction.Object);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();
      manager.RegisterUoW(uow.Object);
      manager.GetSession("nick");

      manager.RollbackAll(null);

      Assert.Multiple(() =>
      {
        transaction.Verify(x => x.Rollback(), Times.Once);
        session.Verify(x => x.Dispose(), Times.Never);
      });
    }

    [Test]
    public void RollbackAll_TransactionIsInactive_RollbackDidNotCalled()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(false);
      var manager = GetSessionFactoryManager(out var session, transaction.Object);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();
      manager.RegisterUoW(uow.Object);
      manager.GetSession("nick");

      manager.RollbackAll(null);

      Assert.Multiple(() =>
      {
        transaction.Verify(x => x.Rollback(), Times.Never);
        session.Verify(x => x.Dispose(), Times.Never);
      });
    }

    [Test]
    public void RollbackAll_CloseSessionsAndHasException_SessionClearCalled()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(false);
      var manager = GetSessionFactoryManager(out var session, transaction.Object);

      var uow = new Mock<IUnitOfWork>();
      var settings = uow.As<IUoWSettings>();
      settings.Setup(x => x.CloseSessions)
              .Returns(true);
      manager.RegisterUoW(uow.Object);
      manager.GetSession("nick");

      manager.RollbackAll(true);

      Assert.Multiple(() =>
      {
        Assert.That(session.Object.DefaultReadOnly);
        session.Verify(x => x.Clear(), Times.Once);
        transaction.Verify(x => x.Rollback(), Times.Never);
        session.Verify(x => x.Dispose(), Times.Once);
      });
    }

    [Test]
    public async Task RollbackAllAsync_TransactionIsActive_RollbackCalled()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(true);
      var manager = GetSessionFactoryManager(out var session, transaction.Object);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();
      manager.RegisterUoW(uow.Object);
      manager.GetSession("nick");

      await Task.WhenAll(manager.RollbackAllAsync(null, default(CancellationToken)));

      Assert.Multiple(() =>
      {
        transaction.Verify(x => x.RollbackAsync(default(CancellationToken)), Times.Once);
        session.Verify(x => x.Dispose(), Times.Never);
      });
    }

    [Test]
    public async Task RollbackAllAsync_TransactionIsInactive_RollbackDidNotCalled()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(false);
      var manager = GetSessionFactoryManager(out var session, transaction.Object);
      var uow = new Mock<IUnitOfWork>();
      uow.As<IUoWSettings>();
      manager.RegisterUoW(uow.Object);
      manager.GetSession("nick");

      await Task.WhenAll(manager.RollbackAllAsync(null, default(CancellationToken)));

      Assert.Multiple(() =>
      {
        transaction.Verify(x => x.RollbackAsync(default(CancellationToken)), Times.Never);
        session.Verify(x => x.Dispose(), Times.Never);
      });
    }

    [Test]
    public async Task RollbackAllAsync_CloseSessionsAndHasException_SessionClearCalled()
    {
      var transaction = new Mock<ITransaction>();
      transaction.Setup(x => x.IsActive)
                 .Returns(false);
      var manager = GetSessionFactoryManager(out var session, transaction.Object);

      var uow = new Mock<IUnitOfWork>();
      var settings = uow.As<IUoWSettings>();
      settings.Setup(x => x.CloseSessions)
              .Returns(true);
      manager.RegisterUoW(uow.Object);
      manager.GetSession("nick");

      await Task.WhenAll(manager.RollbackAllAsync(true, default(CancellationToken)));

      Assert.Multiple(() =>
      {
        Assert.That(session.Object.DefaultReadOnly);
        session.Verify(x => x.Clear(), Times.Once);
        transaction.Verify(x => x.RollbackAsync(default(CancellationToken)), Times.Never);
        session.Verify(x => x.Dispose(), Times.AtLeastOnce);
      });
    }

    [Test]
    public void GetStatelessSession_SimpleCall_SessionMethodCalled()
    {
      var session = new Mock<IStatelessSession>();
      var sessionFactoryManager = new Mock<ISessionFactoryManager>();
      var sessionFactory = new Mock<ISessionFactory>();
      sessionFactory.Setup(x => x.OpenStatelessSession())
                    .Returns(session.Object);
      sessionFactoryManager.Setup(x => x.GetSessionFactory(It.IsAny<string>()))
                           .Returns(sessionFactory.Object);

      var manager = new ScopedSessionManager(sessionFactoryManager.Object);

      manager.GetStatelessSession("nick");

      sessionFactory.Verify(x => x.OpenStatelessSession(), Times.Once);
    }


    private static ScopedSessionManager GetSessionFactoryManager(out Mock<ISession> session, ITransaction transaction = null)
    {
      session = new Mock<ISession>();
      session.SetupProperty(x => x.DefaultReadOnly);
      session.Setup(x => x.Transaction)
             .Returns(transaction);
      var sessionFactoryManager = new Mock<ISessionFactoryManager>();
      var sessionFactory = new Mock<ISessionFactory>();
      sessionFactory.Setup(x => x.OpenSession())
                    .Returns(session.Object);
      sessionFactoryManager.Setup(x => x.GetSessionFactory(It.IsAny<string>()))
                           .Returns(sessionFactory.Object);

      var manager = new ScopedSessionManager(sessionFactoryManager.Object);
      return manager;
    }
  }
}
