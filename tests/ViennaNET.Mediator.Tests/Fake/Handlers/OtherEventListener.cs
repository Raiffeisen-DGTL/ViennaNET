using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  public class OtherEventListener : IMessageHandler<Event>
  {
    public void Handle(Event message)
    {
      TestContext.WriteLine($"The {nameof(EventListener)} listened {message.GetType().FullName}");
    }
  }
}