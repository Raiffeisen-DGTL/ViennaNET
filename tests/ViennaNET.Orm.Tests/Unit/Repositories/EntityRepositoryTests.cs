using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Repositories;
using Moq;
using NHibernate;
using NHibernate.Transform;
using NUnit.Framework;
using ViennaNET.Orm.Tests.Unit.DSL;

namespace ViennaNET.Orm.Tests.Unit.Repositories
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(EntityRepository<>))]
  public class EntityRepositoryTests
  {
    [Test]
    public void Add_SessionNotReadOnly_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entity = new TestEntity();

      repository.Add(entity);

      session.Verify(x => x.SaveOrUpdate(entity), Times.Once);
    }

    [Test]
    public void Add_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
        .Returns(true);
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entity = new TestEntity();

      Assert.Throws<EntityRepositoryException>(() => repository.Add(entity), "The Repository is read-only");
    }

    [Test]
    public async Task AddAsync_SessionNotReadOnly_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entity = new TestEntity();

      await repository.AddAsync(entity);

      session.Verify(x => x.SaveOrUpdateAsync(entity, default), Times.Once);
    }

    [Test]
    public void AddAsync_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
        .Returns(true);
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entity = new TestEntity();

      Assert.ThrowsAsync<EntityRepositoryException>(async () => await repository.AddAsync(entity),
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
      var repository = new EntityRepository<object>(session.Object, new NullLogger<EntityRepository<object>>());

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
      var repository = new EntityRepository<object>(session.Object, new NullLogger<EntityRepository<object>>());

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
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entity = new TestEntity();

      Assert.Throws<EntityRepositoryException>(() => repository.Delete(entity), "The Repository is read-only");
    }

    [Test]
    public void DeleteAsync_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
        .Returns(true);
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entity = new TestEntity();

      Assert.ThrowsAsync<EntityRepositoryException>(async () => await repository.DeleteAsync(entity),
        "The Repository is read-only");
    }

    [Test]
    public void Get_NullableTypeKey_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());

      repository.Get<int?>(12);

      session.Verify(x => x.Get<TestEntity>(12), Times.Once);
    }

    [Test]
    public void Get_NullableTypeKeyHasNoValue_SessionMethodDidNotCall()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());

      repository.Get<int?>(null);

      session.Verify(x => x.Get<TestEntity>(12), Times.Never);
    }

    [Test]
    public void Get_ValueTypeKey_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());

      repository.Get(12);

      session.Verify(x => x.Get<TestEntity>(12), Times.Once);
    }

    [Test]
    public async Task GetAsync_NullableTypeKey_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());

      await repository.GetAsync<int?>(12);

      session.Verify(x => x.GetAsync<TestEntity>(12, default), Times.Once);
    }

    [Test]
    public async Task GetAsync_NullableTypeKeyHasNoValue_SessionMethodDidNotCall()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());

      await repository.GetAsync<int?>(null);

      session.Verify(x => x.GetAsync<TestEntity>(12, default), Times.Never);
    }

    [Test]
    public async Task GetAsync_ValueTypeKey_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());

      await repository.GetAsync(12);

      session.Verify(x => x.GetAsync<TestEntity>(12, default), Times.Once);
    }

    [Test]
    public void GetCurrentSession_SessionPassedToRepo_SessionReturned()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());

      var currentSession = repository.GetCurrentSession();

      Assert.That(session.Object == currentSession);
    }

    [Test]
    public void Insert_SessionNotReadOnly_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entity = new TestEntity();

      repository.Insert(entity);

      session.Verify(x => x.Persist(entity), Times.Once);
    }

    [Test]
    public void Insert_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
        .Returns(true);
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entity = new TestEntity();

      Assert.Throws<EntityRepositoryException>(() => repository.Insert(entity), "The Repository is read-only");
    }

    [Test]
    public async Task InsertAsync_SessionNotReadOnly_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entity = new TestEntity();

      await repository.InsertAsync(entity);

      session.Verify(x => x.PersistAsync(entity, default), Times.Once);
    }

    [Test]
    public void InsertAsync_SessionReadOnly_ExceptionThrown()
    {
      var session = new Mock<ISession>();
      session.Setup(x => x.DefaultReadOnly)
        .Returns(true);
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());
      var entoty = new TestEntity();

      Assert.ThrowsAsync<EntityRepositoryException>(async () => await repository.InsertAsync(entoty),
        "The Repository is read-only");
    }

    [Test]
    public void Query_SimpleCall_SessionMethodCalled()
    {
      var session = new Mock<ISession>();
      var repository = new EntityRepository<TestEntity>(session.Object, new NullLogger<EntityRepository<TestEntity>>());

      repository.Query();

      session.Verify(x => x.Query<TestEntity>(), Times.Once);
    }
  }
}