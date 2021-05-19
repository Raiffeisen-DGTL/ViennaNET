using System;
using Apache.NMS;

namespace ViennaNET.Messaging.ActiveMQQueue
{
  /// <inheritdoc />
  public class ActiveMqConnectionFactory : IActiveMqConnectionFactory
  {
    /// <inheritdoc />
    public IConnectionFactory GetConnectionFactory(string server, int port)
    {
      var connectUri = new Uri($"activemq:tcp://{server}:{port}");

      return new NMSConnectionFactory(connectUri);
    }
  }
}
