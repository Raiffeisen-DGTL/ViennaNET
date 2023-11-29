using NUnit.Framework;

namespace ViennaNET.Utils.Tests
{
  [TestFixture(Category = "Unit")]
  public class LanguageUtilsTests
  {
    [Test]
    public void In_WhenObjectsEqual_ShouldReturnTrue()
    {
      var testItem = "test";

      Assert.IsTrue(testItem.In("1", "test", "2"));
    }

    [Test]
    public void In_WhenObjectsNotEqual_ShouldReturnFalse()
    {
      var testItem = 3;

      Assert.IsFalse(testItem.In(1, 2, 4));
    }

    [Test]
    public void In_WhenObjectsListEmpty_ShouldNotThrow()
    {
      var testItem = 1.2;

      Assert.IsFalse(testItem.In());
    }

    [Test]
    public void In_WhenObjectIsNull_ShouldNotThrow()
    {
      object testItem = null;

      Assert.IsFalse(testItem.In());
    }

    [Test]
    public void In_WhenAllIsNull_ShouldNotThrow()
    {
      object testItem = null;

      Assert.IsFalse(testItem.In(null));
    }
  }
}