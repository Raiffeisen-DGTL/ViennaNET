using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using ViennaNET.Mediator.DefaultConfiguration;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.DefaultConfiguration;
using ViennaNET.Orm.Diagnostic;
using ViennaNET.Orm.Repositories;
using ViennaNET.Orm.SQLite.DefaultConfiguration;
using ViennaNET.Orm.Tests.LocalDatabaseTests.EntityFactoryTestData;
using ViennaNET.Security;

namespace ViennaNET.Orm.Tests.LocalDatabaseTests;

[TestFixture]
[Category("Unit")]
[TestOf(typeof(IEntityFactoryService))]
public class EntityFactoryTest
{
    [OneTimeSetUp]
    public void SetupEntityFactory()
    {
        var container = new Container();

        container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("TestConfiguration/localDatabaseTests.json", true)
            .Build();

        var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
        container.RegisterInstance<IConfiguration>(configuration);

        container.RegisterInstance(new Mock<ISecurityContextFactory>().Object);
        container.RegisterInstance(loggerFactory);
        container.RegisterInstance(new Mock<ILogger<ScopedSessionManager>>().Object);
        container.RegisterInstance(new Mock<ILogger<OrmConnectionsChecker>>().Object);

        container.RegisterPackages(new[]
        {
            typeof(MediatorPackage).Assembly, typeof(OrmPackage).Assembly, typeof(SQLiteOrmPackage).Assembly,
            typeof(TestPackage).Assembly
        });

        _factory = container.GetInstance<IEntityFactoryService>();
    }

    private IEntityFactoryService _factory = null!;

    [Test]
    public void TestCommit()
    {
        //arrange
        const int testValue = 5;
        var testItem = new TestEntity();

        using (_ = _factory.GetScopedSession())
        using (var uow = _factory.Create(IsolationLevel.ReadCommitted, true, true))
        {
            var dataRepository = _factory.Create<TestEntity>();
            dataRepository.Add(testItem);
            uow.Commit();
        }

        //act
        using (_ = _factory.GetScopedSession())
        using (var uow = _factory.Create(IsolationLevel.ReadCommitted, true, true))
        {
            var dataRepository = _factory.Create<TestEntity>();
            var modifyData = dataRepository.Get(testItem.Id);
            modifyData.Status = testValue;
            uow.Save();
            uow.Commit();
        }

        //assert
        int readValue;
        using (_ = _factory.GetScopedSession())
        using (var uow = _factory.Create(IsolationLevel.ReadCommitted, true, true))
        {
            var dataRepository = _factory.Create<TestEntity>();
            var readEntity = dataRepository.Get(testItem.Id);
            readValue = readEntity.Status;
            dataRepository.Delete(readEntity);
            uow.Commit();
        }

        Assert.That(readValue, Is.EqualTo(testValue));
    }

    [Test]
    public void TestRollback()
    {
        //arrange
        const int testValue = 5;
        var testItem = new TestEntity();

        using (_ = _factory.GetScopedSession())
        using (var uow = _factory.Create(IsolationLevel.ReadCommitted, true, true))
        {
            var dataRepository = _factory.Create<TestEntity>();
            dataRepository.Add(testItem);
            uow.Commit();
        }

        //act
        using (_ = _factory.GetScopedSession())
        using (var uow = _factory.Create(IsolationLevel.ReadCommitted, true, true))
        {
            var dataRepository = _factory.Create<TestEntity>();
            var modifyData = dataRepository.Get(testItem.Id);
            modifyData.Status = testValue;
            uow.Save();
            uow.Rollback();
        }

        //assert
        int readValue;
        using (_ = _factory.GetScopedSession())
        using (var uow = _factory.Create(IsolationLevel.ReadCommitted, true, true))
        {
            var dataRepository = _factory.Create<TestEntity>();
            var readEntity = dataRepository.Get(testItem.Id);
            readValue = readEntity.Status;
            dataRepository.Delete(readEntity);
            uow.Commit();
        }

        Assert.That(readValue, Is.Not.EqualTo(testValue));
    }

    [Test]
    public void TestDoubleDisposingAfterRollback()
    {
        //arrange
        const int testValue = 5;
        var testItem = new TestEntity();

        using (_ = _factory.GetScopedSession())
        using (var uow = _factory.Create(IsolationLevel.ReadCommitted, true, true))
        {
            var dataRepository = _factory.Create<TestEntity>();
            dataRepository.Add(testItem);
            uow.Commit();
        }

        //act
        using (var session = _factory.GetScopedSession())
        using (var uow = _factory.Create(IsolationLevel.ReadCommitted, true, true))
        {
            var dataRepository = _factory.Create<TestEntity>();
            var modifyData = dataRepository.Get(testItem.Id);
            modifyData.Status = testValue;

            uow.Save();
            uow.Rollback();

            uow.Dispose();
            session.Dispose();
        }

        //assert
        int readValue;
        using (_ = _factory.GetScopedSession())
        using (var uow = _factory.Create(IsolationLevel.ReadCommitted, true, true))
        {
            var dataRepository = _factory.Create<TestEntity>();
            var readEntity = dataRepository.Get(testItem.Id);
            readValue = readEntity.Status;
            dataRepository.Delete(readEntity);
            uow.Commit();
        }

        Assert.That(readValue, Is.Not.EqualTo(testValue));
    }
}