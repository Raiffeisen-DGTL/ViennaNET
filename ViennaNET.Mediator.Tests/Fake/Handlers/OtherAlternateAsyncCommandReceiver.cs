using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class OtherAlternateAsyncCommandReceiver : IMessageHandlerAsync<AlternateCommand>
  {
    public Task HandleAsync(AlternateCommand message, CancellationToken cancellationToken)
    {
      return Task.Factory.StartNew(() =>
      {
        TestContext.WriteLine($"The {nameof(AlternateCommandReceiver)} received {message.GetType().FullName}");
      }, cancellationToken);
    }

  }
}
