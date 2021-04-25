using IBM.XMS;

namespace ViennaNET.Messaging.MQSeriesQueue.Infrastructure
{
  internal class MqSeriesQueueConnectionFactoryProvider : IMqSeriesQueueConnectionFactoryProvider
  {
    private static readonly object connectionFactoryLock = new object();

    public IConnectionFactory GetConnectionFactory(int connectionType)
    {
      lock (connectionFactoryLock)
      {
        var factoryFactory = XMSFactoryFactory.GetInstance(connectionType);
        return factoryFactory.CreateConnectionFactory();
      }
    }
  }
}
