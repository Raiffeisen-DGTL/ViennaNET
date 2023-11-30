using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using ViennaNET.Extensions.Mediator.Tests.Models;

namespace ViennaNET.Extensions.Mediator.Tests.Handlers;

[ExcludeFromCodeCoverage(Justification = "Используется для тестирования.")]
internal class MessageHandler : IMessageHandler<Message>
{
    public Task HandleAsync(Message message, CancellationToken cancellationToken = default)
    {
        TestContext.WriteLine($"The initial value of message type {message.GetType()} equals {message.Value}");

        message.Value++;

        TestContext.WriteLine(
            $"The type of {GetType()} received {message.GetType()} " +
            $"and incremented value of property {nameof(message.Value)} on 1.");
        
        return Task.CompletedTask;
    }
}