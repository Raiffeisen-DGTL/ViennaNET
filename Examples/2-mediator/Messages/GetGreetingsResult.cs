using System.Collections.Generic;

namespace MediatorService.Messages
{
  public class GetGreetingsResult
  {
    public IReadOnlyCollection<string> Greetings { get; set; }
  }
}
