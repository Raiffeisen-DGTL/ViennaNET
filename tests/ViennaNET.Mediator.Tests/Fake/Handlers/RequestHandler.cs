using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class RequestHandler : IMessageHandler<Request, int>
  {
    public const int Result = 10;

    public int Handle(Request message)
    {
      TestContext.WriteLine($"The {nameof(RequestHandler)} handled {message.GetType().FullName}");
      return Result;
    }
  }
}