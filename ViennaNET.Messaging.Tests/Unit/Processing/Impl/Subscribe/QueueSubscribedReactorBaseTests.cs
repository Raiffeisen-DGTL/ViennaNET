using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Processing.Impl.Subscribe;

namespace ViennaNET.Messaging.Tests.Unit.Processing.Impl.Subscribe
{
  [TestFixture(Category = "Unit")]
  [TestOf(typeof(QueueSubscribedReactorBase))]
  public class QueueSubscribedReactorBaseTests
  {
    [Test]
    public void StartProcessing_NoErrorsOnConnection_ReturnsTrue()
    {
      // arrange
      var reactor = GetReactor(null, out var fakeAdapter, out var fakeHealthService);

      // act
      var result = reactor.StartProcessing();

      // assert
      Assert.That(result, Is.True);
    }

    [Test]
    public void StartProcessing_AdapterAlreadyConnected_AdapterConnectNotCalled()
    {
      // arrange
      var reactor = GetReactor(true, out var fakeAdapter, out var fakeHealthService);
      fakeAdapter.Setup(x => x.IsConnected)
                 .Returns(true);
      // act
      var result = reactor.StartProcessing();

      // assert
      fakeAdapter.Verify(x => x.Connect(), Times.Never);
      Assert.That(result, Is.False);
    }

    [Test]
    public void StartProcessing_AdapterThrowsExceptionOnConnect_FailReturnsFalse()
    {
      // arrange
      var reactor = GetReactor(true, out var fakeAdapter, out var fakeHealthService);
      fakeAdapter.Setup(x => x.Connect())
                 .Throws<Exception>();
      // act
      var result = reactor.StartProcessing();

      // assert
      Assert.That(result, Is.False);
    }

    [Test]
    public void StartProcessing_AdapterThrowsTimeoutExceptionAndDoNotThrowsOnRetry_SuccessReturnsTrue()
    {
      // arrange
      var isConnectCalled = false;
      var reactor = GetReactor(true, out var fakeAdapter, out var fakeHealthService);
      fakeAdapter.Setup(x => x.Connect())
                 .Callback(() =>
                 {
                   if (!isConnectCalled)
                   {
                     isConnectCalled = true;
                     throw new TimeoutException();
                   }
                 });
      
      // act
      var result = reactor.StartProcessing();

      // assert
      Assert.That(result, Is.True);
    }

    private QueueSubscribedReactorBase GetReactor(
      bool? serviceHealthDependent, out Mock<IMessageAdapterWithSubscribing> fakeAdapter,
      out Mock<IHealthCheckingService> fakeHealthService)
    {
      fakeAdapter = new Mock<IMessageAdapterWithSubscribing>();
      fakeAdapter.Setup(x => x.Configuration)
                 .Returns(new Mock<QueueConfigurationBase>().Object);
      fakeHealthService = new Mock<IHealthCheckingService>();
      var fakeCallContextAccessor = new Mock<IMessagingCallContextAccessor>();

      return new QueueSubscribedReactorWrapper(fakeAdapter.Object, 100, "1", serviceHealthDependent, fakeHealthService.Object,
                                               fakeCallContextAccessor.Object);
    }

    private class QueueSubscribedReactorWrapper : QueueSubscribedReactorBase
    {
      public QueueSubscribedReactorWrapper(
        IMessageAdapterWithSubscribing messageAdapter, int reconnectTimeout, string pollingId, bool? serviceHealthDependent,
        IHealthCheckingService healthCheckingService, IMessagingCallContextAccessor messagingCallContextAccessor) : base(messageAdapter,
                                                                                                                         reconnectTimeout,
                                                                                                                         pollingId,
                                                                                                                         serviceHealthDependent,
                                                                                                                         healthCheckingService,
                                                                                                                         messagingCallContextAccessor)
      {
      }

      protected override bool GetProcessedMessage(BaseMessage message)
      {
        throw new System.NotImplementedException();
      }

      protected override Task<bool> GetProcessedMessageAsync(BaseMessage message)
      {
        throw new System.NotImplementedException();
      }
    }
  }
}