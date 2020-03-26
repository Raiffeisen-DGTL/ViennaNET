using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class CommandReceiver : IMessageHandler<Command>
  {
    public void Handle(Command message)
    {
      TestContext.WriteLine($"The {nameof(CommandReceiver)} received {message.GetType().FullName}");
    }
  }
}
