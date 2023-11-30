using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  internal class OtherRequestWithSeveralHandlersAsyncHandler : IMessageHandlerAsync<RequestWithSeveralHandlers, int>
  {
    private const int Result = 10;

    public Task<int> HandleAsync(RequestWithSeveralHandlers message, CancellationToken cancellationToken)
    {
      return Task.Factory.StartNew(() =>
      {
        TestContext.WriteLine(
          $"The {nameof(OtherRequestWithSeveralHandlersHandler)} handled {message.GetType().FullName}");
        return Result;
      }, cancellationToken);
    }
  }
}