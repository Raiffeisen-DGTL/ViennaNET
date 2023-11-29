using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class AsyncCommandReceiver : IMessageHandlerAsync<Command>
  {
    public Task HandleAsync(Command message, CancellationToken cancellationToken)
    {
      return Task.Factory.StartNew(() =>
      {
        TestContext.WriteLine($"The {nameof(CommandReceiver)} received {message.GetType().FullName}");
      }, cancellationToken);
    }
  }
}