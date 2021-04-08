using NUnit.Framework;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(AdvancedBusFactory))]
  public class AdvancedBusFactoryTests
  {
    [Test]
    public void Create_CorrectButNoRealParams_DoesNotThrowException()
    {
      // arrange
      var advancedBusFactory = new AdvancedBusFactory();

      // act & assert
      Assert.DoesNotThrow(() =>
        advancedBusFactory.Create("-a1", 1, "1", "1", "1", 0, s => { }));
    }
  }
}