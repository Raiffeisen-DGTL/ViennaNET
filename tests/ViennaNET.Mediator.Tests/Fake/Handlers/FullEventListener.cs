using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  internal class FullEventListener : EventListener, IMessageHandlerAsync<Event>
  {
    public Task HandleAsync(Event message, CancellationToken cancellationToken)
    {
      Handle(message);
      return Task.CompletedTask;
    }
  }
}