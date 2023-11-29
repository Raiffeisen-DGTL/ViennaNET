using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.TestUtils.Orm.Tests
{
  [TestFixture]
  [Category("Unit")]
  public class EntityFactoryServiceStubBuilderTests
  {
    [Test]
    public void Builder_WhenSomeRepositoriesAdded_ShouldCorrectlyReturnThem()
    {
      // Arrange
      var customersRepository = Mock.Of<IEntityRepository<CustomerEntity>>();
      var cardsRepository = Mock.Of<IEntityRepository<CardEntity>>();

      // Act
      var efs = EntityFactoryServiceStubBuilder.Create()
        .WithRepository(customersRepository)
        .WithRepository(cardsRepository)
        .Build();

      // Assert
      var customersRepositoryResolved = efs.Create<CustomerEntity>();
      Assert.That(customersRepositoryResolved, Is.EqualTo(customersRepository));

      var cardsRepositoryResolved = efs.Create<CardEntity>();
      Assert.That(cardsRepositoryResolved, Is.EqualTo(cardsRepository));
    }

    [Test]
    public void Builder_WhenSomeCustomQueryExecutorsAdded_ShouldCorrectlyReturnThem()
    {
      // Arrange
      var cardsCustomQueryExecutor = Mock.Of<ICustomQueryExecutor<CardEntity>>();
      var customerCustomQueryExecutor = Mock.Of<ICustomQueryExecutor<CustomerEntity>>();

      // Act
      var efs = EntityFactoryServiceStubBuilder.Create()
        .WithCustomQueryExecutor(cardsCustomQueryExecutor)
        .WithCustomQueryExecutor(customerCustomQueryExecutor)
        .Build();

      // Assert
      var cardsCustomQueryExecutorResolved = efs.CreateCustomQueryExecutor<CardEntity>();
      Assert.That(cardsCustomQueryExecutorResolved, Is.EqualTo(cardsCustomQueryExecutor));

      var customerCustomQueryExecutorResolved = efs.CreateCustomQueryExecutor<CustomerEntity>();
      Assert.That(customerCustomQueryExecutorResolved, Is.EqualTo(customerCustomQueryExecutor));
    }

    [Test]
    public void Builder_WhenSomeCommandExecutorsAdded_ShouldCorrectlyReturnThem()
    {
      // Arrange
      var expireCardCommandExecutor = Mock.Of<ICommandExecutor<ExpireCardCommand>>();
      var setEmptyLimitCardCommandExecutor = Mock.Of<ICommandExecutor<SetEmptyLimitCardCommand>>();

      // Act
      var efs = EntityFactoryServiceStubBuilder.Create()
        .WithCommandExecutor(expireCardCommandExecutor)
        .WithCommandExecutor(setEmptyLimitCardCommandExecutor)
        .Build();

      // Assert
      var expireCardCommandExecutorResolved = efs.CreateCommandExecutor<ExpireCardCommand>();
      Assert.That(expireCardCommandExecutorResolved, Is.EqualTo(expireCardCommandExecutor));

      var setEmptyLimitCardCommandExecutorResolved = efs.CreateCommandExecutor<SetEmptyLimitCardCommand>();
      Assert.That(setEmptyLimitCardCommandExecutorResolved, Is.EqualTo(setEmptyLimitCardCommandExecutor));
    }

    [Test]
    public void Builder_WhenUowCreated_ShouldBeAbleToCommitRollbackAndDisposeWithoutExceptions()
    {
      // Arrange
      var customersRepository = new Mock<IEntityRepository<CustomerEntity>>().Object;

      // Act
      var efs = EntityFactoryServiceStubBuilder.Create()
        .WithRepository(customersRepository)
        .Build();

      async Task Act()
      {
        using var uow = efs.Create();
        var _ = efs.Create<CustomerEntity>();

        uow.Commit();
        await uow.CommitAsync(CancellationToken.None);
        uow.Rollback();
        await uow.RollbackAsync(CancellationToken.None);
      }

      // Assert
      Assert.DoesNotThrowAsync(Act);
    }

    [Test]
    public void Builder_WhenSomeUowCreated_ShouldWorkWithoutExceptions()
    {
      // Arrange
      var customersRepository = new Mock<IEntityRepository<CustomerEntity>>().Object;
      var customersUow = new Mock<IUnitOfWork>().Object;

      // Act
      var efs = EntityFactoryServiceStubBuilder.Create()
        .WithRepository(customersRepository)
        .WithUow(customersUow)
        .Build();

      async Task Act()
      {
        using var uow = efs.Create();
        var _ = efs.Create<CustomerEntity>();

        uow.Commit();
        await uow.CommitAsync(CancellationToken.None);
        uow.Rollback();
        await uow.RollbackAsync(CancellationToken.None);
      }

      // Assert
      Assert.DoesNotThrowAsync(Act);
    }

    [Test]
    public void Builder_ShouldBeAbleToCreateScopedSessionWithoutExceptions()
    {
      // Arrange
      var customersRepository = new Mock<IEntityRepository<CustomerEntity>>().Object;

      // Act
      var efs = EntityFactoryServiceStubBuilder.Create()
        .WithRepository(customersRepository)
        .Build();

      void Act()
      {
        using var uow = efs.GetScopedSession();
        var _ = efs.Create<CustomerEntity>();
      }

      // Assert
      Assert.DoesNotThrow(Act);
    }
  }
}