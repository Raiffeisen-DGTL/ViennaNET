using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.TestUtils.Orm.Tests
{
  [TestFixture]
  public class CommandExecutorStubTests
  {
    [Test]
    public void Execute_WhenCalledMultipleTimes_ShouldReturnResultAndSaveEachCommandPassed()
    {
      // Arrange
      const int expectedResult = 1;
      var card1 = new CardEntity(123, "123");
      var card2 = new CardEntity(456, "456");

      var command1 = new ExpireCardCommand(card1);
      var command2 = new ExpireCardCommand(card2);

      var commandExecutorStub = CommandExecutorStub.Create<ExpireCardCommand>(expectedResult);

      // Act
      var result1 = commandExecutorStub.Execute(command1);
      var result2 = commandExecutorStub.Execute(command2);

      // Assert
      Assert.That(result1, Is.EqualTo(expectedResult));
      Assert.That(result2, Is.EqualTo(expectedResult));

      Assert.That(commandExecutorStub.CommandsCalled, Is.EquivalentTo(new[] {command1, command2}));
    }

    [Test]
    public async Task ExecuteAsync_WhenCalledMultipleTimes_ShouldReturnResultAndSaveEachCommandPassed()
    {
      // Arrange
      const int expectedResult = 1;
      var card1 = new CardEntity(123, "123");
      var card2 = new CardEntity(456, "456");

      var command1 = new ExpireCardCommand(card1);
      var command2 = new ExpireCardCommand(card2);

      var commandExecutorStub = CommandExecutorStub.Create<ExpireCardCommand>(expectedResult);

      // Act
      var result1 = await commandExecutorStub.ExecuteAsync(command1);
      var result2 = await commandExecutorStub.ExecuteAsync(command2);

      // Assert
      Assert.That(result1, Is.EqualTo(expectedResult));
      Assert.That(result2, Is.EqualTo(expectedResult));

      Assert.That(commandExecutorStub.CommandsCalled, Is.EquivalentTo(new[] { command1, command2 }));
    }
  }
}