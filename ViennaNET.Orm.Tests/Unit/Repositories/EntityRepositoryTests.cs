using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Repositories;
using Moq;
using NHibernate;
using NHibernate.Transform;
using NUnit.Framework;

namespace ViennaNET.Orm.Tests.Unit.Repositories
{
  [TestFixture, Category("Unit"), TestOf(typeof(EntityRepository<>))]
  public class EntityRepositoryTests
  {
    [Test]
    public void Add_SessionNotReadOnly_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      repository.Add(command);

      session.Verify(x => x.SaveOrUpdate(command), Times.Once);
    }

    [Test]
    public void Add_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
             .Returns(true);
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      Assert.Throws<EntityRepositoryException>(() => repository.Add(command), "The Repository is read-only");
    }

    [Test]
    public async Task AddAsync_SessionNotReadOnly_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      await repository.AddAsync(command);

      session.Verify(x => x.SaveOrUpdateAsync(command, default(CancellationToken)), Times.Once);
    }

    [Test]
    public void AddAsync_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
             .Returns(true);
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      Assert.ThrowsAsync<EntityRepositoryException>(async () => await repository.AddAsync(command, default(CancellationToken)),
                                                    "The Repository is read-only");
    }

    [Test]
    public void CustomQuery_SimpleCall_SessionMethodCalled()
    {
      var query = new TestQuery();

      var sqlQuery = new Mock<ISQLQuery>();
      sqlQuery.Setup(x => x.SetResultTransformer(It.IsAny<IResultTransformer>()))
              .Returns(sqlQuery.Object);
      var session = new Mock<ISession>();
      session.Setup(x => x.CreateSQLQuery(It.IsAny<string>()))
             .Returns(sqlQuery.Object);
      var repository = new EntityRepository<object>(session.Object);

      repository.CustomQuery(query);

      sqlQuery.Verify(x => x.SetParameter(It.IsAny<string>(), It.IsAny<object>()));
      sqlQuery.Verify(x => x.SetResultTransformer(It.IsAny<IResultTransformer>()));
    }

    [Test]
    public async Task CustomQueryAsync_SimpleCall_SessionMethodCalled()
    {
      var query = new TestQuery();

      var sqlQuery = new Mock<ISQLQuery>();
      sqlQuery.Setup(x => x.SetResultTransformer(It.IsAny<IResultTransformer>()))
              .Returns(sqlQuery.Object);
      var session = new Mock<ISession>();
      session.Setup(x => x.CreateSQLQuery(It.IsAny<string>()))
             .Returns(sqlQuery.Object);
      var repository = new EntityRepository<object>(session.Object);

      await repository.CustomQueryAsync(query);

      sqlQuery.Verify(x => x.SetParameter(It.IsAny<string>(), It.IsAny<object>()));
      sqlQuery.Verify(x => x.SetResultTransformer(It.IsAny<IResultTransformer>()));
    }

    [Test]
    public void Delete_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
             .Returns(true);
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      Assert.Throws<EntityRepositoryException>(() => repository.Delete(command), "The Repository is read-only");
    }

    [Test]
    public void DeleteAsync_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
             .Returns(true);
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      Assert.ThrowsAsync<EntityRepositoryException>(async () => await repository.DeleteAsync(command),
                                                    "The Repository is read-only");
    }

    [Test]
    public void Get_NullableTypeKey_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);

      repository.Get<int?>(12);

      session.Verify(x => x.Get<TestCommand>(12), Times.Once);
    }

    [Test]
    public void Get_NullableTypeKeyHasNoValue_SessionMethodDidNotCall()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);

      repository.Get<int?>(null);

      session.Verify(x => x.Get<TestCommand>(12), Times.Never);
    }

    [Test]
    public void Get_ValueTypeKey_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);

      repository.Get(12);

      session.Verify(x => x.Get<TestCommand>(12), Times.Once);
    }

    [Test]
    public async Task GetAsync_NullableTypeKey_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);

      await repository.GetAsync<int?>(12);

      session.Verify(x => x.GetAsync<TestCommand>(12, default(CancellationToken)), Times.Once);
    }

    [Test]
    public async Task GetAsync_NullableTypeKeyHasNoValue_SessionMethodDidNotCall()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);

      await repository.GetAsync<int?>(null);

      session.Verify(x => x.GetAsync<TestCommand>(12, default(CancellationToken)), Times.Never);
    }

    [Test]
    public async Task GetAsync_ValueTypeKey_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);

      await repository.GetAsync(12);

      session.Verify(x => x.GetAsync<TestCommand>(12, default(CancellationToken)), Times.Once);
    }

    [Test]
    public void GetCurrentSession_SessionPassedToRepo_SessionReturned()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);

      var currentSession = repository.GetCurrentSession();

      Assert.That(session.Object == currentSession);
    }

    [Test]
    public void Insert_SessionNotReadOnly_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      repository.Insert(command);

      session.Verify(x => x.Persist(command), Times.Once);
    }

    [Test]
    public void Insert_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
             .Returns(true);
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      Assert.Throws<EntityRepositoryException>(() => repository.Insert(command), "The Repository is read-only");
    }

    [Test]
    public async Task InsertAsync_SessionNotReadOnly_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      await repository.InsertAsync(command);

      session.Verify(x => x.PersistAsync(command, default(CancellationToken)), Times.Once);
    }

    [Test]
    public void InsertAsync_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
             .Returns(true);
      var repository = new EntityRepository<TestCommand>(session.Object);
      var command = new TestCommand();

      Assert.ThrowsAsync<EntityRepositoryException>(async () => await repository.InsertAsync(command),
                                                    "The Repository is read-only");
    }

    [Test]
    public void Query_SimpleCall_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestCommand>(session.Object);

      repository.Query();

      session.Verify(x => x.Query<TestCommand>(), Times.Once);
    }
  }
}
