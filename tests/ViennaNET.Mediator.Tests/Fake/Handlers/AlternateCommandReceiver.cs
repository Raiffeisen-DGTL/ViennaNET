using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class AlternateCommandReceiver : IMessageHandler<AlternateCommand>
  {
    public void Handle(AlternateCommand message)
    {
      TestContext.WriteLine($"The {nameof(AlternateCommandReceiver)} received {message.GetType().FullName}");
    }
  }
}