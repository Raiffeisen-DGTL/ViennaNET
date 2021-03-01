using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ViennaNET.Diagnostic;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Processing.Impl.Subscribe;
using ViennaNET.Messaging.Tests.Unit.DSL;

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
      var reactor = GetReactor();

      // act
      var result = reactor.StartProcessing();

      // assert
      Assert.That(result, Is.True);
    }

    [Test]
    public void StartProcessing_AdapterAlreadyConnected_AdapterConnectNotCalled()
    {
      // arrange
      var fakeAdapter = Given.MessageAdapter.MockPlease<IMessageAdapterWithSubscribing>();
      fakeAdapter.Setup(x => x.IsConnected)
                 .Returns(true);
      var reactor = GetReactor(messageAdapter: fakeAdapter.Object);
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
      var fakeAdapter = Given.MessageAdapter.MockPlease<IMessageAdapterWithSubscribing>();
      fakeAdapter.Setup(x => x.Connect())
                 .Throws<Exception>();
      var reactor = GetReactor(messageAdapter: fakeAdapter.Object);
      
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
      var fakeAdapter = Given.MessageAdapter.MockPlease<IMessageAdapterWithSubscribing>();
      fakeAdapter.Setup(x => x.Connect())
                 .Callback(() => 
                 {
                   if (!isConnectCalled)
                   {
                     isConnectCalled = true;
                     throw new TimeoutException();
                   }
                 });
      var reactor = GetReactor(messageAdapter: fakeAdapter.Object);

      // act
      var result = reactor.StartProcessing();

      // assert
      Assert.That(result, Is.True);
    }

    [Test]
    public async Task StartProcessing_ConcurrentMessages_ShouldProcessSuccessfully()
    {
      // arrange
      var messageAdapter = Given.MessageAdapter.Please<IMessageAdapterWithSubscribing>();
      var reactor = GetReactor(messageAdapter: messageAdapter);
      reactor.StartProcessing();

      // act & assert
      Task SendMessage() => Task.Run(() => messageAdapter.Send(new TextMessage()));
      var tasks = Enumerable.Range(1, 10).Select(_ => SendMessage());
      await Task.WhenAll(tasks);
    }

    [Test]
    public void StartProcessing_ClearCallContextFailed_ShouldProcessMessage()
    {
      var messageAdapter = Given.MessageAdapter.Please<IMessageAdapterWithSubscribing>();
      var messagingCallContextAccessorMock = new Mock<IMessagingCallContextAccessor>();
      messagingCallContextAccessorMock.Setup(x => x.CleanContext()).Throws<Exception>();
      var reactor = GetReactor(messageAdapter: messageAdapter,
                               messagingCallContextAccessor: messagingCallContextAccessorMock.Object);
      
      reactor.StartProcessing();
      messageAdapter.Send(new TextMessage());

      Assert.That(reactor.WasProcessed, Is.True);
    }

    [Test]
    public void StartProcessing_WhenDiagnosticFailed_ShouldUnsubscribe()
    {
      var messageAdapter = Given.MessageAdapter.MockPlease<IMessageAdapterWithSubscribing>();
      var healthCheckingServiceMock = new Mock<IHealthCheckingService>();
      var reactor = GetReactor(messageAdapter: messageAdapter.Object,
                               healthService: healthCheckingServiceMock.Object);
      
      reactor.StartProcessing();
      healthCheckingServiceMock.Raise(x => x.DiagnosticFailedEvent += null);

      messageAdapter.Verify(x => x.Unsubscribe());
    }

    private static QueueSubscribedReactorWrapper GetReactor(
      bool? serviceHealthDependent = true,
      IMessageAdapterWithSubscribing messageAdapter = null,
      IHealthCheckingService healthService = null,
      IMessagingCallContextAccessor messagingCallContextAccessor = null)
      => new QueueSubscribedReactorWrapper(messageAdapter ?? Given.MessageAdapter.Please<IMessageAdapterWithSubscribing>(),
                                           100,
                                           serviceHealthDependent,
                                           healthService ?? Mock.Of<IHealthCheckingService>(),
                                           messagingCallContextAccessor ?? Mock.Of<IMessagingCallContextAccessor>());

    private class QueueSubscribedReactorWrapper : QueueSubscribedReactorBase
    {
      public QueueSubscribedReactorWrapper(
        IMessageAdapterWithSubscribing messageAdapter,
        int reconnectTimeout,
        bool? serviceHealthDependent,
        IHealthCheckingService healthCheckingService,
        IMessagingCallContextAccessor messagingCallContextAccessor) : base(messageAdapter,
                                                                           reconnectTimeout,
                                                                           serviceHealthDependent,
                                                                           healthCheckingService,
                                                                           messagingCallContextAccessor,
                                                                           Mock.Of<ILogger>())
      {
      }
      
      public bool WasProcessed { get; private set; }

      protected override bool GetProcessedMessage(BaseMessage message)
      {
        WasProcessed = true;
        return true;
      }

      protected override Task<bool> GetProcessedMessageAsync(BaseMessage message)
      {
        WasProcessed = true;
        return Task.FromResult(true);
      }
    }
  }
}