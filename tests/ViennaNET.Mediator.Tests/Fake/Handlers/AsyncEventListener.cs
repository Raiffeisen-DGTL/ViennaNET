using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class AsyncEventListener : IMessageHandlerAsync<Event>
  {
    public async Task HandleAsync(Event message, CancellationToken cancellationToken)
    {
      await Task.Delay(3000, cancellationToken);
      await Task.Factory.StartNew(
        () => TestContext.WriteLine($"The {nameof(EventListener)} listened {message.GetType().FullName}"),
        cancellationToken);
    }
  }
}