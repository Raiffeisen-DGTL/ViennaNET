using IBM.XMS;
using NUnit.Framework;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests;

[TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueConnectionFactoryProvider))]
public class MqSeriesQueueConnectionFactoryProviderTests
{
    [Test]
    public void ConnectionFactoryTest()
    {
        Environment.SetEnvironmentVariable("MQCLNTCF", Directory.GetCurrentDirectory());
        var provider = new MqSeriesQueueConnectionFactoryProvider();

        var factory = provider.GetConnectionFactory(XMSC.CT_WMQ);

        Assert.That(factory, Is.Not.Null);
    }
}