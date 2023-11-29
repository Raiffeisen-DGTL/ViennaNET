using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using ViennaNET.Extensions.Mediator.Tests.Models;

namespace ViennaNET.Extensions.Mediator.Tests.Handlers;

[ExcludeFromCodeCoverage(Justification = "Используется для тестирования.")]
internal class MessageHandlerWithResult : IMessageHandler<Message, int>
{
    public Task<int> HandleAsync(Message message, CancellationToken cancellationToken = default)
    {
        TestContext.WriteLine($"The initial value of message type {message.GetType()} equals {message.Value}");

        message.Value++;

        TestContext.WriteLine(
            $"The type of {GetType()} received {message.GetType()} " +
            $"and incremented value of property {nameof(message.Value)} on 1.");

        TestContext.WriteLine($"The type of {GetType()} was returns sum of {nameof(message.Value)} and 1.");

        return Task.FromResult(message.Value + 1);
    }
}