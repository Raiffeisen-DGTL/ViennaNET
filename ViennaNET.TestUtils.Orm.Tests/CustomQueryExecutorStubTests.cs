using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.TestUtils.Orm.Tests
{
  [TestFixture]
  public class CustomQueryExecutorStubTests
  {
    private static ICollection<CardEntity> CreateSomeCards()
    {
      return new List<CardEntity>()
      {
        new CardEntity(1, "1111222233334444"),
        new CardEntity(2, "4476030000000000"),
        new CardEntity(3, "1111000000000000"),
        new CardEntity(4, "2222000000000000")
      };
    }

    [Test]
    public void CustomQuery_WhenCalledMultipleTimes_ShouldReturnCollectionAndSaveEachQueryPassed()
    {
      // Arrange
      var customer1 = new CustomerEntity() { Id = "Customer1" };
      var customer2 = new CustomerEntity() { Id = "Customer2" };

      var query1 = new GetCustomerCardsCustomQuery(customer1);
      var query2 = new GetCustomerCardsCustomQuery(customer2);

      var cards = CreateSomeCards();
      var customQueryExecutorStub = CustomQueryExecutorStub.Create(cards);

      // Act
      var result1 = customQueryExecutorStub.CustomQuery(query1);
      var result2 = customQueryExecutorStub.CustomQuery(query2);
      
      // Assert
      Assert.That(result1, Is.EqualTo(cards));
      Assert.That(result2, Is.EqualTo(cards));
      
      Assert.That(customQueryExecutorStub.QueriesCalled, Is.EquivalentTo(new [] {query1, query2}));
    }

    [Test]
    public async Task CustomQueryAsync_WhenCalledMultipleTimes_ShouldReturnCollectionAndSaveEachQueryPassed()
    {
      // Arrange
      var customer1 = new CustomerEntity() { Id = "Customer1" };
      var customer2 = new CustomerEntity() { Id = "Customer2" };

      var query1 = new GetCustomerCardsCustomQuery(customer1);
      var query2 = new GetCustomerCardsCustomQuery(customer2);

      var cards = CreateSomeCards();
      var customQueryExecutorStub = CustomQueryExecutorStub.Create(cards);

      // Act
      var result1 = await customQueryExecutorStub.CustomQueryAsync(query1);
      var result2 = await customQueryExecutorStub.CustomQueryAsync(query2);

      // Assert
      Assert.That(result1, Is.EqualTo(cards));
      Assert.That(result2, Is.EqualTo(cards));

      Assert.That(customQueryExecutorStub.QueriesCalled, Is.EquivalentTo(new[] { query1, query2 }));
    }
  }
}
