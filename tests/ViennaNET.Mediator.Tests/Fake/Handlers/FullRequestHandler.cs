using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  internal class FullRequestHandler : RequestHandler, IMessageHandlerAsync<Request, int>
  {
    public Task<int> HandleAsync(Request message, CancellationToken cancellationToken)
    {
      return Task.FromResult(Handle(message));
    }
  }
}