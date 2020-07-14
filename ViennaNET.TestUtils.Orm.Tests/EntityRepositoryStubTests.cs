using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;
using NUnit.Framework;
using ViennaNET.Orm.Seedwork;
using ViennaNET.TestUtils.Orm.Tests.AggregatedEntities;

namespace ViennaNET.TestUtils.Orm.Tests
{
  [TestFixture]
  [Category("Unit")]
  public class EntityRepositoryStubTests
  {
    private static readonly Expression<Func<CardEntity, bool>> filterByOnes = c => c.Pan.StartsWith("1111");

    private static IEnumerable<CardEntity> CreateSomeCards()
    {
      yield return new CardEntity(1, "1111222233334444");
      yield return new CardEntity(2, "4476030000000000");
      yield return new CardEntity(3, "1111000000000000");
      yield return new CardEntity(4, "2222000000000000");
    }

    private static IEntityRepository<CustomerEntity> CreateCustomersRepository(IEnumerable<CustomerEntity> items)
    {
      return EntityRepositoryStub.Create<CustomerEntity, string>(items);
    }

    private static IEntityRepository<CardEntity> CreateCardsRepository(IEnumerable<CardEntity> items)
    {
      return EntityRepositoryStub.Create(items);
    }

    [Test]
    public void CreatedRepositoryStubWithIntKey_ShouldBeAbleToAddNewItemAndItShouldBeQueryable()
    {
      // Arrange
      var cardEntities = CreateSomeCards().ToList();
      var originalCardEntitiesCount = cardEntities.Count;
      var repo = CreateCardsRepository(cardEntities);

      const int newCardId = 100;
      const string newCardPan = "9999888877776666";
      var newCard = new CardEntity(newCardId, newCardPan);

      // Act
      repo.Add(newCard);

      // Assert
      var repoContentAfterAdding = repo.Query().ToList();
      Assert.That(repoContentAfterAdding.Count, Is.EqualTo(originalCardEntitiesCount + 1));
      Assert.That(repoContentAfterAdding, Contains.Item(newCard));

      var newCardGotById = repo.Get(newCardId);
      Assert.That(newCardGotById, Is.EqualTo(newCard));

      var newCardGotByQuery = repo.Query().Where(c => c.Pan == newCardPan).ToList();
      Assert.That(newCardGotByQuery, Has.Count.EqualTo(1));
      Assert.That(newCardGotByQuery, Has.One.EqualTo(newCard));
    }


    [Test]
    public async Task CreatedRepositoryStubWithIntKey_ShouldBeAbleToAddNewItemAndItShouldBeQueryable_Asynchronously()
    {
      // Arrange
      var cardEntities = CreateSomeCards().ToList();
      var originalCardEntitiesCount = cardEntities.Count;
      var repo = CreateCardsRepository(cardEntities);

      const int newCardId = 100;
      const string newCardPan = "9999888877776666";
      var newCard = new CardEntity(newCardId, newCardPan);

      // Act
      await repo.AddAsync(newCard);

      // Assert
      var repoContentAfterAdding = repo.Query().ToList();
      Assert.That(repoContentAfterAdding.Count, Is.EqualTo(originalCardEntitiesCount + 1));
      Assert.That(repoContentAfterAdding, Contains.Item(newCard));

      var newCardGotById = repo.Get(newCardId);
      Assert.That(newCardGotById, Is.EqualTo(newCard));

      var newCardGotByQuery = repo.Query().Where(c => c.Pan == newCardPan).ToList();
      Assert.That(newCardGotByQuery, Has.Count.EqualTo(1));
      Assert.That(newCardGotByQuery, Has.One.EqualTo(newCard));
    }

    [Test]
    public void CreatedRepositoryStubWithIntKey_ShouldBeAbleToReturnItemById()
    {
      // Arrange
      var cardEntities = CreateSomeCards().ToArray();
      var repo = CreateCardsRepository(cardEntities);

      // Act
      const int id = 2;
      var cardWithId2 = repo.Get(id);

      // Assert
      var expectedCard = cardEntities.Single(c => c.Id == id);
      Assert.That(cardWithId2, Is.EqualTo(expectedCard));
    }

    [Test]
    public async Task CreatedRepositoryStubWithIntKey_ShouldBeAbleToReturnItemById_Asynchronously()
    {
      // Arrange
      var cardEntities = CreateSomeCards().ToArray();
      var repo = CreateCardsRepository(cardEntities);

      // Act
      const int id = 2;
      var cardWithId2 = await repo.GetAsync(id);

      // Assert
      var expectedCard = cardEntities.Single(c => c.Id == id);
      Assert.That(cardWithId2, Is.EqualTo(expectedCard));
    }

    [Test]
    public async Task
      CreatedRepositoryStubWithIntKey_ShouldBeQueryableWithNHibernateFetches_Asynchronously_WithoutExceptions()
    {
      // Arrange
      var maslovIgorFio = "MASLOV Igor";
      var employees = new[] {new EmployeeEntity {Fio = "Rua Pus 9"}, new EmployeeEntity {Fio = maslovIgorFio}};
      var repo = EntityRepositoryStub.Create(employees);

      // Act
      var result = await repo.Query()
        .Where(e => e.Fio == maslovIgorFio)
        .Fetch(e => e.CurrentTeam).ThenFetch(t => t.TeamCostCenter)
        .FetchMany(e => e.EmployeeInTeamsHistory).ThenFetch(t => t.Team).ThenFetchMany(t => t.Managers)
        .OrderBy(e => e.Id)
        .GroupBy(g => g.CurrentTeam)
        .ToListAsync(CancellationToken.None);

      // Assert
      Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public void CreatedRepositoryStubWithIntKey_ShouldBeQueryableWithVerySimpleWhereFilter()
    {
      // Arrange
      var cardEntities = CreateSomeCards().ToArray();
      var repo = CreateCardsRepository(cardEntities);

      // Act
      var cardsStartingWithOnes = repo.Query()
        .Where(filterByOnes)
        .ToList();

      // Assert
      Assert.That(cardsStartingWithOnes, Has.Count.EqualTo(2));
      var expectedCard0 = cardEntities.Single(c => c.Pan == "1111222233334444");
      var expectedCard1 = cardEntities.Single(c => c.Pan == "1111000000000000");
      Assert.That(cardsStartingWithOnes, Has.One.EqualTo(expectedCard0));
      Assert.That(cardsStartingWithOnes, Has.One.EqualTo(expectedCard1));
    }

    [Test]
    public async Task CreatedRepositoryStubWithIntKey_ShouldBeQueryableWithVerySimpleWhereFilter_Asynchronously()
    {
      // Arrange
      var cardEntities = CreateSomeCards().ToArray();
      var repo = CreateCardsRepository(cardEntities);

      // Act
      var cardsStartingWithOnes = await repo.Query()
        .Where(filterByOnes)
        .ToListAsync();

      // Assert
      var expectedCards = repo.Query()
        .Where(filterByOnes)
        .ToList();
      Assert.That(cardsStartingWithOnes, Is.EquivalentTo(expectedCards));
    }

    [Test]
    public async Task CreatedRepositoryStubWithIntKey_ShouldBeToDeleteWithVerySimpleWhereFilter()
    {
      // Arrange
      var cardEntities = CreateSomeCards().ToArray();
      var repo = CreateCardsRepository(cardEntities);

      // Act
      await repo.Query()
        .Where(filterByOnes)
        .DeleteAsync(CancellationToken.None);

      // Assert
      var cardsExpectedToBeDeleted = cardEntities.Where(filterByOnes.Compile()).ToList();
      Assert.That(cardsExpectedToBeDeleted, Has.Count.GreaterThanOrEqualTo(1),
        "some cards in given array should match the filter");
      var cardsExpectedToBeKept = cardEntities.Except(cardsExpectedToBeDeleted).ToList();

      var repoContentAfterAdding = repo.Query().ToList();
      Assert.That(repoContentAfterAdding, Is.EquivalentTo(cardsExpectedToBeKept));
    }

    [Test]
    public async Task CreatedRepositoryStubWithIntKey_AddShouldBeToDeleteAndInsert()
    {
      // Arrange
      var cardEntities = CreateSomeCards().ToArray();
      var repo = CreateCardsRepository(cardEntities);

      // Act
      const int newCardId = 100;
      var newCardPan = "9999888877776666";
      var newCard = new CardEntity(newCardId, newCardPan);
      await repo.AddAsync(newCard);
      newCardPan = Guid.NewGuid().ToString();
      var newCard1 = new CardEntity(newCardId, newCardPan);
      await repo.AddAsync(newCard1);

      // Assert
      var updatedCard = await repo.GetAsync(newCardId);
      Assert.AreEqual(newCardPan, updatedCard.Pan);
    }

    [Test]
    public async Task CreatedRepositoryStubWithIntKey_GetWithWrongIds_ShouldBeNull()
    {
      // Arrange
      var cardEntities = CreateSomeCards().ToArray();
      var repo = CreateCardsRepository(cardEntities);

      // Act
      var card = await repo.GetAsync(-1000);

      // Assert
      Assert.IsNull(card);
    }
  }
}