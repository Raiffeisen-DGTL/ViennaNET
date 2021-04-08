using System;
using EasyNetQ;
using EasyNetQ.DI;
using Moq;

namespace ViennaNET.Messaging.RabbitMQQueue.Tests.DSL
{
  internal class AdvancedBusFactoryMock : IAdvancedBusFactory
  {
    public Mock<IAdvancedBus> AdvancedBusMock { get; } = new Mock<IAdvancedBus>();

    public IAdvancedBus Create(string hostName, ushort hostPort, string virtualHost, string username, string password,
      ushort requestedHeartbeat, Action<IServiceRegister> registerServices)
    {
      return AdvancedBusMock.Object;
    }
  }
}