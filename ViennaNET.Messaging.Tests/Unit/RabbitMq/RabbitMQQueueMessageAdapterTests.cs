using System;
using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Topology;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.RabbitMQQueue;

namespace ViennaNET.Messaging.Tests.Unit.RabbitMq
{
  [TestFixture(Category = "Unit", TestOf = typeof(RabbitMqQueueMessageAdapter))]
  public class RabbitMQQueueMessageAdapterTests
  {
    [TestCase(true)]
    [TestCase(false)]
    public void Ctor_BusFactoryIsNull_ThrowException(bool isDiagnostic)
    {
      Assert.Throws<ArgumentNullException>(() => new RabbitMqQueueMessageAdapter(null, new RabbitMqQueueConfiguration(), isDiagnostic));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Ctor_ConfigIsNull_ThrowException(bool isDiagnostic)
    {
      var fakeAdvancedBusFactory = new Mock<IAdvancedBusFactory>();
      Assert.Throws<ArgumentNullException>(() => new RabbitMqQueueMessageAdapter(fakeAdvancedBusFactory.Object, null, isDiagnostic));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Ctor_ConfigIsNotNull_DoesNotThrowException(bool isDiagnostic)
    {
      var fakeAdvancedBusFactory = new Mock<IAdvancedBusFactory>();
      Assert.DoesNotThrow(() => new RabbitMqQueueMessageAdapter(fakeAdvancedBusFactory.Object, new RabbitMqQueueConfiguration(),
                                                                isDiagnostic));
    }

    [TestCase("", "", false)]
    [TestCase("exchange", "", false)]
    [TestCase("", "queue", false)]
    [TestCase("exchange", "queue", true)]
    public void Connect_VariousExchangeAndQueueNames_InitializeBindsExpectedCalled(string exchange, string queue, bool isBindCalled)
    {
      // arrange
      var advancedBusMock = new Mock<IAdvancedBus>();
      var fakeAdvancedBusFactory = new Mock<IAdvancedBusFactory>();
      fakeAdvancedBusFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<string>(), It.IsAny<string>(),
                                                 It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<Action<IServiceRegister>>()))
                            .Returns(advancedBusMock.Object);

      var config = new RabbitMqQueueConfiguration { ExchangeName = exchange, QueueName = queue };

      // act
      var adapter = new RabbitMqQueueMessageAdapter(fakeAdvancedBusFactory.Object, config, false);
      adapter.Connect();

      // assert
      var times = isBindCalled
        ? Times.AtLeastOnce()
        : Times.Never();
      advancedBusMock.Verify(x => x.Bind(It.IsAny<IExchange>(), It.IsAny<IQueue>(), It.IsAny<string>()), times);
    }

    [Test]
    public void Connect_ExchangeAndQueueNamesIsNotEmptyListRoutingsIsNotNull_CreateRoutigsBinds()
    {
      // arrange
      var advancedBusMock = new Mock<IAdvancedBus>();
      var fakeAdvancedBusFactory = new Mock<IAdvancedBusFactory>();
      fakeAdvancedBusFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<string>(), It.IsAny<string>(),
                                                 It.IsAny<string>(), It.IsAny<ushort>(), It.IsAny<Action<IServiceRegister>>()))
                            .Returns(advancedBusMock.Object);
      var routings = new [] { "1", "2" };
      var config = new RabbitMqQueueConfiguration { ExchangeName = "exchange", QueueName = "queue", Routings = routings };

      // act
      var adapter = new RabbitMqQueueMessageAdapter(fakeAdvancedBusFactory.Object, config, false);
      adapter.Connect();

      // assert
      advancedBusMock.Verify(x => x.Bind(It.IsAny<IExchange>(), It.IsAny<IQueue>(), It.Is<string>(key => key == routings[0])), Times.AtLeastOnce);
      advancedBusMock.Verify(x => x.Bind(It.IsAny<IExchange>(), It.IsAny<IQueue>(), It.Is<string>(key => key == routings[1])), Times.AtLeastOnce);
    }

  }
}
