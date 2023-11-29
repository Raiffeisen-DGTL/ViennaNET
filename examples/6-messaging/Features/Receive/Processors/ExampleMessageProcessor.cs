using System.Threading.Tasks;
using MessagingService.Features.Receive.Messages;
using Microsoft.Extensions.Logging;
using ViennaNET.Messaging;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;

namespace MessagingService.Features.Receive.Processors
{
  public class ExampleMessageProcessor : IMessageProcessorAsync
  {
    private readonly IMessageDeserializer<ExampleMessage> _deserializer;
    private readonly ILogger _logger;

    public ExampleMessageProcessor(IMessageDeserializer<ExampleMessage> deserializer, ILogger<ExampleMessageProcessor> logger)
    {
      _deserializer = deserializer;
      _logger = logger;
    }

    public async Task<bool> ProcessAsync(BaseMessage message)
    {
      var result = _deserializer.Deserialize(message);

      _logger.LogInformation("Received text: {Message}", result.Text);
      await Task.Delay(30000);
      _logger.LogInformation("Message handled");
      
      return true;
    }
  }
}