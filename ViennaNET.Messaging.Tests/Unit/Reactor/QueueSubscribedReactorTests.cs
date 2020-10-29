using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Processing.Impl.Subscribe;
using ViennaNET.Messaging.Tests.Unit.DSL;

namespace ViennaNET.Messaging.Tests.Unit.Reactor
{
  [TestFixture(Category = "Unit", TestOf = typeof(QueueSubscribedReactor))]
  class QueueSubscribedReactorTests
  {
    [Test]
    public void Start_DefaultArgs_NoError()
    {
      var reactor = Given.QueueSubscribedReactor.Please();

      reactor.StartProcessing();

      Assert.Pass();
    }

    [Test]
    public void Stop_DefaultArgs_NoError()
    {
      var reactor = Given.QueueSubscribedReactor.Please();

      reactor.StartProcessing();
      reactor.Stop();

      Assert.Pass();
    }

    [Test]
    public void StartProcessing_HasMessage_ProcessorCalled()
    {
      var message = new TextMessage();
      var processorMock = new Mock<IMessageProcessor>();
      var reactor = Given.QueueSubscribedReactor
        .WithMessageAdapter(
          b => b.Please<IMessageAdapterWithSubscribing>(
            m =>
            {
              m
                .Setup(x => x.Subscribe(It.IsAny<Func<BaseMessage, Task>>()))
                .Callback<Func<BaseMessage, Task>>(cb => cb(message).GetAwaiter().GetResult());
            }))
        .WithMessageProcessor(processorMock.Object)
        .Please();

      reactor.StartProcessing();

      processorMock.Verify(x => x.Process(message));
    }
  }
}
