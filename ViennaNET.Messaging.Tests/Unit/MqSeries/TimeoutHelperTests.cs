using System;
using System.Threading;
using NUnit.Framework;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;

namespace ViennaNET.Messaging.Tests.Unit.MqSeries
{
  [TestFixture(Category = "Unit", TestOf = typeof(TimeoutHelper))]
  public class TimeoutHelperTests
  {
    public static readonly object[] TestCaseSource = new[]
    {
      new object[] { null, TimeoutHelper.NoWaitTimeout },
      new object[] { TimeSpan.MaxValue, TimeoutHelper.InfiniteWaitTimeout },
      new object[] { TimeSpan.MinValue, TimeoutHelper.NoWaitTimeout },
      new object[] { Timeout.InfiniteTimeSpan, TimeoutHelper.InfiniteWaitTimeout },
      new object[] { TimeSpan.FromSeconds(10), 10000 },
      new object[] { TimeSpan.FromSeconds(-10), TimeoutHelper.NoWaitTimeout },
      new object[] { TimeSpan.Zero, TimeoutHelper.NoWaitTimeout },
    };

    [Test]
    [TestCaseSource("TestCaseSource")]
    public void GetTimeoutTest(TimeSpan? timeout, long expected)
    {
      var retVal = TimeoutHelper.GetTimeout(timeout);

      Assert.AreEqual(expected, retVal);
    }
  }
}
