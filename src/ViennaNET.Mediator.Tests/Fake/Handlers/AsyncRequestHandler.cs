using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class AsyncRequestHandler : IMessageHandlerAsync<Request, int>
  {
    private const int result = 10;

    public Task<int> HandleAsync(Request message, CancellationToken cancellationToken)
    {
      return Task.Factory.StartNew(() =>
      {
        TestContext.WriteLine($"The {nameof(RequestHandler)} handled {message.GetType().FullName}");
        return result;
      });
    }
  }
}
