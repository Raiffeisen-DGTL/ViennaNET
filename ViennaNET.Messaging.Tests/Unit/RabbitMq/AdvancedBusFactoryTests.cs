using NUnit.Framework;
using ViennaNET.Messaging.RabbitMQQueue;

namespace ViennaNET.Messaging.Tests.Unit.RabbitMq
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
