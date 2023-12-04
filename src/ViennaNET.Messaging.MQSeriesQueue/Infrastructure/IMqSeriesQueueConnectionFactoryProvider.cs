using IBM.XMS;

namespace ViennaNET.Messaging.MQSeriesQueue.Infrastructure
{
  internal interface IMqSeriesQueueConnectionFactoryProvider
  {
    IConnectionFactory GetConnectionFactory(int connectionType);
  }
}