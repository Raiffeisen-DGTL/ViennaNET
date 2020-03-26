using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class AlternateEventListener : IMessageHandler<Event>
  {
    public void Handle(Event message)
    {
      TestContext.WriteLine($"The {nameof(AlternateEventListener)} listened {message.GetType().FullName}");
    }
  }
}
