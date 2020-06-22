using System.Threading.Tasks;
using EmptyService.Features.Send.Messages;
using ViennaNET.Messaging.Factories;

namespace EmptyService.Features.Send.Services
{
  public class SendingService : ISendingService
  {
    private const string queueId = "exampleQueueId";

    private readonly IMessagingComponentFactory _factory;

    public SendingService(IMessagingComponentFactory factory)
    {
      _factory = factory;
    }

    public async Task Send(string text)
    {
      var message = new ExampleMessage() { Text = text };

      using var sender = _factory.CreateMessageSender<ExampleMessage>(queueId);

      await sender.SendMessageAsync(message);
    }
  }
}
