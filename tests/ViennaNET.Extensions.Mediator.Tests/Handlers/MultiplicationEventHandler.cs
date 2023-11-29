using System.Diagnostics.CodeAnalysis;
using ViennaNET.Extensions.Mediator.Tests.Models;

namespace ViennaNET.Extensions.Mediator.Tests.Handlers;

[ExcludeFromCodeCoverage(Justification = "Используется для тестирования.")]
public class MultiplicationEventHandler : IMessageHandler<Event>
{
    public Task HandleAsync(Event message, CancellationToken cancellationToken)
    {
        message.Value *= 2;
        return Task.CompletedTask;
    }
}