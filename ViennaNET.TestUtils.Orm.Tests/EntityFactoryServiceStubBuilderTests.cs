using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
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
      var customersRepository = new Mock<IEntityRepository<CustomerEntity>>().Object;
      var cardsRepository = new Mock<IEntityRepository<CardEntity>>().Object;

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