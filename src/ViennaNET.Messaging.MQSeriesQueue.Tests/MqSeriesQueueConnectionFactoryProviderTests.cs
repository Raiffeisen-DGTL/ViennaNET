using IBM.XMS;
using NUnit.Framework;
using ViennaNET.Messaging.MQSeriesQueue.Infrastructure;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueConnectionFactoryProvider))]
  public class MqSeriesQueueConnectionFactoryProviderTests
  {
    [Test]
    public void CannectionFactoryTest()
    {
      var provider = new MqSeriesQueueConnectionFactoryProvider();

      var factory = provider.GetConnectionFactory(XMSC.CT_WMQ);

      Assert.IsNotNull(factory);
    }
  }
}