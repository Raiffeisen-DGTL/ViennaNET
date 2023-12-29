using NUnit.Framework;

namespace ViennaNET.Utils.Tests
{
    [TestFixture(Category = "Unit")]
    public class LanguageUtilsTests
    {
        [Test]
        public void In_WhenObjectsEqual_ShouldReturnTrue()
        {
            Assert.That("test".In("1", "test", "2"), Is.True);
        }

        [Test]
        public void In_WhenObjectsNotEqual_ShouldReturnFalse()
        {
            Assert.That(3.In(1, 2, 4), Is.False);
        }

        [Test]
        public void In_WhenObjectsListEmpty_ShouldNotThrow()
        {
            Assert.That(1.2.In(), Is.False);
        }

        [Test]
        public void In_WhenObjectIsNull_ShouldNotThrow()
        {
            Assert.That(((object?)null).In(), Is.False);
        }

        [Test]
        public void In_WhenAllIsNull_ShouldNotThrow()
        {
            Assert.That(((object?)null).In(null!), Is.False);
        }
    }
}