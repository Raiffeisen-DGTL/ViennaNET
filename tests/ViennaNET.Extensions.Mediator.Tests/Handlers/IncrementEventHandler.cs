using System.Diagnostics.CodeAnalysis;
using ViennaNET.Extensions.Mediator.Tests.Models;

namespace ViennaNET.Extensions.Mediator.Tests.Handlers;

[ExcludeFromCodeCoverage(Justification = "Используется для тестирования.")]
public class IncrementEventHandler : IMessageHandler<Event>
{
    public Task HandleAsync(Event message, CancellationToken cancellationToken)
    {
        message.Value++;
        return Task.CompletedTask;
    }
}