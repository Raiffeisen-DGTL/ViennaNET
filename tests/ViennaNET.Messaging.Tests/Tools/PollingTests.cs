using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Processing.Impl;

namespace ViennaNET.Messaging.Tests.Tools;

[TestFixture(Category = "Unit", TestOf = typeof(Polling))]
internal class PollingTests
{
    [Test]
    public void Constructor_ActionIsNull_Exception()
    {
        Assert.That(() => new Polling(1000, null!, Mock.Of<ILogger>()), Throws.ArgumentNullException);
    }

    [Test]
    public void Dispose_IsStarted_Exception()
    {
        var p = new Polling(1000, ct => Task.FromResult(false), Mock.Of<ILogger>());

        p.StartPolling();
        p.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
        {
            var _ = p.IsStarted;
        });
    }

    [Test]
    public void StartPolling_ExceptionInAction_Success()
    {
        var p = new Polling(1000, ct => throw new ApplicationException(), Mock.Of<ILogger>());

        p.StartPolling();

        Assert.That(p.IsStarted, Is.True);
    }

    [Test]
    public void StartPolling_IsStarted_True()
    {
        var p = new Polling(1000, ct => Task.FromResult(false), Mock.Of<ILogger>());

        p.StartPolling();

        Assert.That(p.IsStarted, Is.True);
    }

    [Test]
    public void StartPolling_Twice_Success()
    {
        var p = new Polling(1000, ct => Task.FromResult(false), Mock.Of<ILogger>());

        p.StartPolling();
        p.StartPolling();

        Assert.That(p.IsStarted, Is.True);
    }
}