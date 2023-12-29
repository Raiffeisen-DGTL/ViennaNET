using NUnit.Framework;
using StackExchange.Redis;

namespace ViennaNET.Redis.Tests.Unit;

[TestFixture(Category = "Unit")]
public class StackExchangeEnumTests
{
    [Test]
    public void StackExchange_CommandFlags_DefaultIsNone()
    {
        Assert.That(CommandFlags.None, Is.Default);
    }

    [Test]
    public void StackExchange_When_DefaultIsAlways()
    {
        Assert.That(When.Always, Is.Default);
    }
}