using NUnit.Framework;
using ViennaNET.Extensions.Mediator.Tests.Handlers;
using ViennaNET.Extensions.Mediator.Tests.Models;

namespace ViennaNET.Extensions.Mediator.Tests;

[Category("Unit")]
[TestOf(typeof(Mediator))]
public class MediatorTests
{
    [Test]
    public void Ctor_Throws_ArgumentNullException()
    {
        Assert.That(() => new Mediator(null!), Throws.ArgumentNullException);
    }

    [Test]
    public void Ctor_Return_Mediator()
    {
        Assert.That(() => new Mediator(new List<IMessageHandler>()), Throws.Nothing);
    }

    [Test]
    public void SendMessageAsync_Throws_ArgumentNullException()
    {
        const string expectedExMessage = "Value cannot be null. (Parameter 'message')";
        var message = default(Message);
        var mediator = new Mediator(new List<IMessageHandler>());

        Assert.That(async () => await mediator.SendAsync(message!),
            Throws.ArgumentNullException.And.Message.EqualTo(expectedExMessage));
    }

    [Test]
    public void SendMessageAsync_Throws_InvalidOperationException()
    {
        const int initialValue = 1;

        var message = new Message(initialValue);
        var expectedExMessage =
            $"A handler of {typeof(IMessageHandler<Message>)} for the message type {message.GetType()} was not found or found more than one handlers.";
        var mediator = new Mediator(new List<IMessageHandler>());

        Assert.That(async () => await mediator.SendAsync(message),
            Throws.InvalidOperationException.And.Message.EqualTo(expectedExMessage));
    }

    [Test]
    public void SendMessageAsync_WithResult_Throws_InvalidOperationException()
    {
        const int initialValue = 1;

        var message = new Message(initialValue);
        var expectedExMessage =
            $"A handler of {typeof(IMessageHandler<Message, int>)} for the message type {message.GetType()} was not found or found more than one handlers.";
        var mediator = new Mediator(new List<IMessageHandler>());

        Assert.That(async () => await mediator.SendAsync<Message, int>(message),
            Throws.InvalidOperationException.And.Message.EqualTo(expectedExMessage));
    }

    [Test]
    public async Task SendMessageAsync_Message_Handled()
    {
        const int initialValue = 1;

        var message = new Message(initialValue);
        var mediator = new Mediator(new List<IMessageHandler> { new MessageHandler() });

        await mediator.SendAsync(message);

        Assert.That(message.Value, Is.EqualTo(2));
    }

    [Test]
    public async Task SendMessageAsync_Return_ExpectedResult()
    {
        const int initialValue = 1;

        var message = new Message(initialValue);
        var mediator = new Mediator(new List<IMessageHandler> { new MessageHandlerWithResult() });
        var response = await mediator.SendAsync<Message, int>(message);

        Assert.That(response, Is.EqualTo(3));
    }

    [Test]
    public async Task NotifyAsync_Event_Handled()
    {
        const int initialValue = 1;

        var @event = new Event(initialValue);
        var mediator = new Mediator(new List<IMessageHandler>
        {
            // Порядок регистрации имеет значение.
            new MultiplicationEventHandler(), new IncrementEventHandler()
        });

        await mediator.NotifyAsync(@event);

        Assert.That(@event.Value, Is.EqualTo(4));
    }

    [Test]
    public void NotifyAsync_Throws_ArgumentNullException()
    {
        const string expectedExMessage = "Value cannot be null. (Parameter 'event')";

        var @event = default(Event);
        var mediator = new Mediator(new List<IMessageHandler>());

        Assert.That(async () => await mediator.NotifyAsync(@event!),
            Throws.ArgumentNullException.And.Message.EqualTo(expectedExMessage));
    }

    [Test]
    public void NotifyAsync_Throws_InvalidOperationException()
    {
        const int initialValue = 1;

        var @event = new Event(initialValue);
        var expectedExMessage =
            $"A handlers of {typeof(IMessageHandler<Event>)} for the message type {@event.GetType()} was not found.";
        var mediator = new Mediator(new List<IMessageHandler>());

        Assert.That(() => mediator.NotifyAsync(@event),
            Throws.InvalidOperationException.And.Message.EqualTo(expectedExMessage));
    }
}