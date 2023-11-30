using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class AlternateAsyncEventListener : IMessageHandlerAsync<Event>
  {
    public Task HandleAsync(Event message, CancellationToken cancellationToken)
    {
      return Task.Factory.StartNew(
        () => TestContext.WriteLine($"The {nameof(AlternateEventListener)} listened {message.GetType().FullName}"),
        cancellationToken);
    }
  }
}