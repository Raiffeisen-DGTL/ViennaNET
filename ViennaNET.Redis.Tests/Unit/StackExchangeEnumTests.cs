using NUnit.Framework;
using StackExchange.Redis;

namespace ViennaNET.Redis.Tests.Unit
{
  [TestFixture(Category = "Unit")]
  public class StackExchangeEnumTests
  {
    [Test]
    public void StackExchange_CommandFlags_DefaultIsNone()
    {
      Assert.AreEqual(CommandFlags.None, default(CommandFlags));
    }

    [Test]
    public void StackExchange_When_DefaultIsAlways()
    {
      Assert.AreEqual(When.Always, default(When));
    }
  }
}
