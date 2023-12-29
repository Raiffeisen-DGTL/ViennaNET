using NUnit.Framework;
using ViennaNET.Messaging.Extensions;

namespace ViennaNET.Messaging.Tests.Extensions;

[TestFixture(Category = "Unit", TestOf = typeof(TimeoutHelper))]
public class TimeoutHelperTests
{
    public static readonly object[] TestCaseSource =
    {
        new object[] { null!, TimeoutHelper.NoWaitTimeout },
        new object[] { TimeSpan.MaxValue, TimeoutHelper.InfiniteWaitTimeout },
        new object[] { TimeSpan.MinValue, TimeoutHelper.NoWaitTimeout },
        new object[] { Timeout.InfiniteTimeSpan, TimeoutHelper.InfiniteWaitTimeout },
        new object[] { TimeSpan.FromSeconds(10), 10000 },
        new object[] { TimeSpan.FromSeconds(-10), TimeoutHelper.NoWaitTimeout },
        new object[] { TimeSpan.Zero, TimeoutHelper.NoWaitTimeout }
    };

    [Test]
    [TestCaseSource(nameof(TestCaseSource))]
    public void GetTimeoutTest(TimeSpan? timeout, long expected)
    {
        var retVal = TimeoutHelper.GetTimeout(timeout);

        Assert.That(retVal, Is.EqualTo(expected));
    }
}