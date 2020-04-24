using System;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Repositories;
using ViennaNET.Orm.Seedwork;
using Moq;
using NHibernate;
using NUnit.Framework;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Tests.Unit.Repositories
{
  [TestFixture, Category("Unit"), TestOf(typeof(UnitOfWork))]
  public class UnitOfWorkTests
  {
    [Test]
    public void Create_CannotRegisterUoW_ExceptionThrown()
    {
      var entityFactoryService = new Mock<IEntityFactoryService>();
      var sessionManager = new Mock<ISessionManager>();

      Assert.Throws<UowException>(() => new UnitOfWork(entityFactoryService.Object, sessionManager.Object), "Unit of Work already exists");
    }

    [Test]
    public void Create_RegisterUoW_StartTransactionAllCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      CreateUnitOfWork(sessionManager, out _, out _);

      sessionManager.Verify(x => x.StartTransactionAll());
    }

    [Test]
    public void Commit_SimpleCall_SessionManagerMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out _);

      uow.Commit();

      sessionManager.Verify(x => x.CommitAll());
    }

    [Test]
    public void Save_SimpleCall_SessionManagerMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out _);

      uow.Save();

      sessionManager.Verify(x => x.SaveAll());
    }

    [Test]
    public void Rollback_SimpleCall_SessionManagerMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out _);

      uow.Rollback();

      sessionManager.Verify(x => x.RollbackAll(true));
    }

    [Test]
    public async Task CommitAsync_SimpleCall_SessionManagerMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out _);

      await uow.CommitAsync();

      sessionManager.Verify(x => x.CommitAllAsync(default(CancellationToken)));
    }

    [Test]
    public async Task SaveAsync_SimpleCall_SessionManagerMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out _);

      await uow.SaveAsync();

      sessionManager.Verify(x => x.SaveAllAsync(default(CancellationToken)));
    }

    [Test]
    public async Task RollbackAsync_SimpleCall_SessionManagerMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out _);

      await uow.RollbackAsync();

      sessionManager.Verify(x => x.RollbackAllAsync(null, default(CancellationToken)));
    }

    [Test]
    public void MarkDirty_CallsSessionProvider_EvictMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out var session, out var entityRepository);
      var entity = new TestCommand();
      
      uow.MarkDirty(entity);

      session.Verify(x => x.Evict(entity));
      entityRepository.Verify(x => x.Add(entity));
    }

    [Test]
    public void MarkNew_CallsSessionProvider_EvictMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out var entityRepository);
      var entity = new TestCommand();

      uow.MarkNew(entity);

      entityRepository.Verify(x => x.Add(entity));
    }

    [Test]
    public void MarkDeleted_CallsSessionProvider_EvictMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out var entityRepository);
      var entity = new TestCommand();

      uow.MarkDeleted(entity);

      entityRepository.Verify(x => x.Delete(entity));
    }

    [Test]
    public async Task MarkDirtyAsync_CallsSessionProvider_EvictMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out var session, out var entityRepository);
      var entity = new TestCommand();

      await uow.MarkDirtyAsync(entity);

      Assert.Multiple(() =>
      {
        session.Verify(x => x.EvictAsync(entity, default(CancellationToken)));
        entityRepository.Verify(x => x.AddAsync(entity, default(CancellationToken)));
      });
    }

    [Test]
    public async Task MarkNewAsync_CallsSessionProvider_EvictMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out var entityRepository);
      var entity = new TestCommand();

      await uow.MarkNewAsync(entity);

      entityRepository.Verify(x => x.AddAsync(entity, default(CancellationToken)));
    }

    [Test]
    public async Task MarkDeletedAsync_CallsSessionProvider_EvictMethodCalled()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out var entityRepository);
      var entity = new TestCommand();

      await uow.MarkDeletedAsync(entity);

      entityRepository.Verify(x => x.DeleteAsync(entity, default(CancellationToken)));
    }


    [Test]
    public void Dispose_CalledTwoTimes_RollbackAllCalledOnce()
    {
      var sessionManager = new Mock<ISessionManager>();
      var uow = CreateUnitOfWork(sessionManager, out _, out _);

      uow.Dispose();
      uow.Dispose();

      Assert.Multiple(() =>
      {
        sessionManager.Verify(x => x.RollbackAll(true), Times.Once);
        sessionManager.Verify(x => x.UnregisterUoW(), Times.Once);
      });
    }

    [Test]
    public void Dispose_Exception_Handled()
    {
      var sessionManager = new Mock<ISessionManager>();
      sessionManager.Setup(x => x.RollbackAll(It.IsAny<bool?>()))
                    .Throws(new ArgumentException());
      var uow = CreateUnitOfWork(sessionManager, out _, out _);

      uow.Dispose();

      Assert.Pass();
    }

    private static UnitOfWork CreateUnitOfWork(
      Mock<ISessionManager> sessionManager, out Mock<ISession> session, out Mock<IEntityRepository<TestCommand>> entityRepository)
    {
      session = new Mock<ISession>();
      entityRepository = new Mock<IEntityRepository<TestCommand>>();
      var sessionProvider = entityRepository.As<ISessionProvider>();
      sessionProvider.Setup(x => x.GetCurrentSession())
                     .Returns(session.Object);
      var entityFactoryService = new Mock<IEntityFactoryService>();
      entityFactoryService.Setup(x => x.Create<TestCommand>())
                          .Returns(entityRepository.Object);
      sessionManager.Setup(x => x.RegisterUoW(It.IsAny<IUnitOfWork>()))
                    .Returns(true);
      var uow = new UnitOfWork(entityFactoryService.Object, sessionManager.Object);
      return uow;
    }
  }
}
