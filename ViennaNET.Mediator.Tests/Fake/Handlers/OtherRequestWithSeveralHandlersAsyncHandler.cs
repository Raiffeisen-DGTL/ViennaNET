using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  class OtherRequestWithSeveralHandlersAsyncHandler : IMessageHandlerAsync<RequestWithSeveralHandlers, int>
  {
    private const int result = 10;

    public Task<int> HandleAsync(RequestWithSeveralHandlers message, CancellationToken cancellationToken)
    {
      return Task.Factory.StartNew(() =>
      {
        TestContext.WriteLine($"The {nameof(OtherRequestWithSeveralHandlersHandler)} handled {message.GetType().FullName}");
        return result;
      });
    }
  }
}
