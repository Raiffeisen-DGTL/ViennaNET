using System;
using System.Threading.Tasks;
using NUnit.Framework;
using ViennaNET.Mediator.Exceptions;
using ViennaNET.Mediator.Pipeline;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Mediator.Tests.Fake;
using ViennaNET.Mediator.Tests.Fake.Handlers;

namespace ViennaNET.Mediator.Tests.Units.Mediators
{
  [TestFixture(Category = "Unit", TestOf = typeof(Mediator.Mediators.Mediator))]
  public class MediatorTests
  {
    [OneTimeSetUp]
    public void TestInit()
    {
      var preProcessor = new PreProcessorService();
      var mediator = new Mediator.Mediators.Mediator(preProcessor);
      mediator.Register(
        new IMessageHandler[]
        {
          new CommandReceiver(), new EventListener(), new OtherEventListener(), new RequestHandler()
        },
        new IMessageHandlerAsync[]
        {
          new AsyncCommandReceiver(), new AsyncEventListener(), new OtherAsyncEventListener(),
          new AsyncRequestHandler()
        });
      _mediator = mediator;

      var altMediator = new Mediator.Mediators.Mediator(preProcessor);
      altMediator.Register(
        new IMessageHandler[]
        {
          new AlternateCommandReceiver(), new OtherAlternateCommandReceiver(), new EventListener(),
          new OtherEventListener(), new RequestWithSeveralHandlersHandler(),
          new OtherRequestWithSeveralHandlersHandler()
        },
        new IMessageHandlerAsync[]
        {
          new AlternateAsyncCommandReceiver(), new OtherAlternateAsyncCommandReceiver(), new AsyncEventListener(),
          new OtherAsyncEventListener(), new RequestWithSeveralHandlersAsyncHandler(),
          new OtherRequestWithSeveralHandlersAsyncHandler()
        });
      _altMediator = altMediator;
    }

    private IMediator _mediator;
    private IMediator _altMediator;

    [Test]
    public void SendCommandMessageAsyncTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync(new Command { Name = "Command one" });
      }, Throws.Nothing);
    }

    [Test]
    public void SendCommandMessageAsyncWithoutReceiverTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync(new AlternateCommand { Name = "Alternate command one" });
      }, Throws.TypeOf<ExecuteCommandException>());
    }

    [Test]
    public void SendCommandMessageAsyncToSeveralReceiversTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync(new AlternateCommand { Name = "Alternate command one" });
      }, Throws.TypeOf<ExecuteCommandException>());
    }

    [Test]
    public void SendEventMessageAsyncTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync(new Event { Name = "Event one" });
      }, Throws.Nothing);
    }

    [Test]
    public void SendEventMessageAsyncWithoutListenersTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync(new AlternateEvent { Name = "Event one" });
      }, Throws.TypeOf<PublishEventException>());
    }

    [Test(Description = "Test asynchronously sending a null message and expecting ArgumentException.")]
    public void SendNullMessageAsyncTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync<IMessage>(default);
      }, Throws.TypeOf<ArgumentNullException>(), "Message must not be null.");
    }

    [Test(Description =
      "Test asynchronously sending a unsupported message and expecting UnsupportedTypeMessageException.")]
    public void SendUnsupportedMessageAsyncTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync(new UnsupportedMessage());
      }, Throws.TypeOf<UnsupportedTypeMessageException>(), "Message must have type supported to mediator.");
    }

    [Test(ExpectedResult = RequestHandler.Result)]
    public async Task<int> SendRequestMessageAsyncTest()
    {
      return await _mediator.SendMessageAsync<Request, int>(new Request { Name = "Request one" });
    }

    [Test]
    public void SendRequestMessageAsyncWithoutHandlerTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync<RequestWithoutHandler, int>(new RequestWithoutHandler
        {
          Name = "RequestWithoutHandler one"
        });
      }, Throws.TypeOf<SendRequestException>());
    }

    [Test]
    public void SendRequestMessageAsyncToSeveralHandlersTest()
    {
      Assert.That(async () =>
      {
        await _altMediator.SendMessageAsync<RequestWithSeveralHandlers, int>(new RequestWithSeveralHandlers
        {
          Name = "RequestWithSeveralHandlers one"
        });
      }, Throws.TypeOf<SendRequestException>());
    }

    [Test(Description = "Test asynchronously sending a null request message and expecting ArgumentException.")]
    public void SendNullRequestMessageAsyncTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync<IRequest, int>(default);
      }, Throws.TypeOf<ArgumentNullException>(), "Request message must not be null.");
    }

    [Test(Description =
      "Test asynchronously sending a unsupported request message and expecting UnsupportedTypeMessageException.")]
    public void SendUnsupportedRequestMessageAsyncTest()
    {
      Assert.That(async () =>
      {
        await _mediator.SendMessageAsync<IMessage, int>(new UnsupportedRequestMessage());
      }, Throws.TypeOf<UnsupportedTypeMessageException>(), "Request message must have type supported to mediator.");
    }

    [Test]
    public void SendCommandMessageTest()
    {
      Assert.That(() =>
      {
        _mediator.SendMessage(new Command { Name = "Command one" });
      }, Throws.Nothing);
    }

    [Test]
    public void SendCommandMessageWithoutReceiverTest()
    {
      Assert.That(() =>
      {
        _mediator.SendMessage(new AlternateCommand { Name = "Alternate command one" });
      }, Throws.TypeOf<ExecuteCommandException>());
    }

    [Test]
    public void SendCommandMessageToSeveralReceiversTest()
    {
      Assert.That(() =>
      {
        _altMediator.SendMessage(new AlternateCommand { Name = "Alternate command one" });
      }, Throws.TypeOf<ExecuteCommandException>());
    }

    [Test]
    public void SendEventMessageTest()
    {
      Assert.That(() =>
      {
        _mediator.SendMessage(new Event { Name = "Event one" });
      }, Throws.Nothing);
    }

    [Test]
    public void SendEventMessageWithoutListenersTest()
    {
      Assert.That(() =>
      {
        _mediator.SendMessage(new AlternateEvent { Name = "Event one" });
      }, Throws.TypeOf<PublishEventException>());
    }

    [Test(Description = "Test synchronously sending a null message and expecting ArgumentException.")]
    public void SendNullMessageTest()
    {
      Assert.That(() =>
      {
        _mediator.SendMessage<IMessage>(default);
      }, Throws.TypeOf<ArgumentNullException>(), "Message must not be null.");
    }

    [Test(Description =
      "Test synchronously sending a unsupported message and expecting UnsupportedTypeMessageException.")]
    public void SendUnsupportedMessageTest()
    {
      Assert.That(() =>
      {
        _mediator.SendMessage(new UnsupportedMessage());
      }, Throws.TypeOf<UnsupportedTypeMessageException>(), "Message must have type supported to mediator.");
    }

    [Test(ExpectedResult = RequestHandler.Result)]
    public int SendRequestMessageTest()
    {
      return _mediator.SendMessage<Request, int>(new Request { Name = "Request one" });
    }

    [Test]
    public void SendRequestMessageWithoutHandlerTest()
    {
      Assert.That(() =>
      {
        return _mediator.SendMessage<RequestWithoutHandler, int>(new RequestWithoutHandler
        {
          Name = "RequestWithoutHandler one"
        });
      }, Throws.TypeOf<SendRequestException>());
    }

    [Test]
    public void SendRequestMessageToSeveralHandlersTest()
    {
      Assert.That(() =>
      {
        _altMediator.SendMessage<RequestWithSeveralHandlers, int>(new RequestWithSeveralHandlers
        {
          Name = "RequestWithSeveralHandlers one"
        });
      }, Throws.TypeOf<SendRequestException>());
    }

    [Test(Description = "Test synchronously sending a null request message and expecting ArgumentException.")]
    public void SendNullRequestMessageTest()
    {
      Assert.That(() =>
      {
        _mediator.SendMessage<IRequest, int>(default);
      }, Throws.TypeOf<ArgumentNullException>(), "Request message must not be null.");
    }

    [Test(Description =
      "Test synchronously sending a unsupported request message and expecting UnsupportedTypeMessageException.")]
    public void SendUnsupportedRequestMessageTest()
    {
      Assert.That(() =>
      {
        _mediator.SendMessage<IMessage, int>(new UnsupportedRequestMessage());
      }, Throws.TypeOf<UnsupportedTypeMessageException>(), "Request message must have type supported to mediator.");
    }

    [Test]
    public async Task ConcurrentSendMessageAsyncTest()
    {
      var sendCommandMessageTask = _mediator.SendMessageAsync(new Command { Name = "Command one" });
      var sendEventMessageTask = _mediator.SendMessageAsync(new Event { Id = 1, Name = "Event one" });
      var sendEventMessageTask2 = _mediator.SendMessageAsync(new Event { Id = 1, Name = "Event one" });
      var sendRequestMessageTask = _mediator.SendMessageAsync<Request, int>(new Request { Name = "Request one" });

      TestContext.WriteLine("Other action...");

      await Task.WhenAll(sendCommandMessageTask, sendEventMessageTask, sendRequestMessageTask);
    }

    [Test]
    public void RegisterEventListenerTest()
    {
      Assert.That(() =>
      {
        ((IMessageRecipientsRegistrar)_mediator).RegisterEventListener<Event, AlternateEventListener>(
          new AlternateEventListener());
      }, Throws.Nothing);
    }

    [Test(Description = "Test registration already registered event listener.")]
    public void RegisterAlreadyRegisteredEventListenerTest()
    {
      Assert.That(() =>
        {
          ((IMessageRecipientsRegistrar)_mediator).RegisterEventListener<Event, EventListener>(new EventListener());
        }, Throws.TypeOf<ArgumentException>(),
        "There should be no possibility to register the listener more than once.");
    }
  }
}