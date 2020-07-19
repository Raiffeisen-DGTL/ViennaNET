using MessagingService.Features.Receive.Messages;
using ViennaNET.Messaging;
using ViennaNET.Messaging.Messages;
using ViennaNET.Messaging.Tools;
using ViennaNET.Logging;

namespace MessagingService.Features.Receive.Processors
{
  public class ExampleMessageProcessor : IMessageProcessor
  {
    private readonly IMessageDeserializer<ExampleMessage> _deserializer;

    public ExampleMessageProcessor(IMessageDeserializer<ExampleMessage> deserializer)
    {
      _deserializer = deserializer;
    }

    public bool Process(BaseMessage message)
    {
      var result = _deserializer.Deserialize(message);

      Logger.LogInfo($"Received text: {result.Text}");

      return true;
    }
  }
}
