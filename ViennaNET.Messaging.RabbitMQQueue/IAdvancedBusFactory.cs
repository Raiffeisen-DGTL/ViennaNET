using System;
using EasyNetQ;
using EasyNetQ.DI;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  /// <summary>
  /// Wrapper over static RabbitHutch to make more testable code
  /// </summary>
  public interface IAdvancedBusFactory
  {
    /// <summary>
    /// Creates IAdvancedBus instance
    /// </summary>
    /// <param name="hostName">RabbitMQ server</param>
    /// <param name="hostPort">RabbitMQ port</param>
    /// <param name="virtualHost">VirtualHost</param>
    /// <param name="username">User</param>
    /// <param name="password">Password</param>
    /// <param name="requestedHeartbeat">RequestedHeartbeat - interval for diagnosing connection</param>
    /// <param name="registerServices">Action for add some services/components in inner DI of EasyNetQ</param>
    /// <returns>Configured IAdvancedBus</returns>
    IAdvancedBus Create(
      string hostName, ushort hostPort, string virtualHost, string username, string password, ushort requestedHeartbeat,
      Action<IServiceRegister> registerServices);
  }
}
