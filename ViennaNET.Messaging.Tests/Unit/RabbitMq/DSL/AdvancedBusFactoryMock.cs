using System;
using EasyNetQ;
using EasyNetQ.DI;
using Moq;
using ViennaNET.Messaging.RabbitMQQueue;

namespace ViennaNET.Messaging.Tests.Unit.RabbitMq.DSL
{
  internal class AdvancedBusFactoryMock: IAdvancedBusFactory
  {
    public Mock<IAdvancedBus> AdvancedBusMock { get; } = new Mock<IAdvancedBus>();

    public IAdvancedBus Create(string hostName, ushort hostPort, string virtualHost, string username, string password,
      ushort requestedHeartbeat, Action<IServiceRegister> registerServices)
    {
      return AdvancedBusMock.Object;
    }
  }
}
