﻿using MediatorService.Messages;
using ViennaNET.Mediator;

namespace MediatorService.Handlers
{
  public class GetGreetingsHandler : IMessageHandler<GetGreetingsRequest, GetGreetingsResult>
  {
    public GetGreetingsResult Handle(GetGreetingsRequest message)
    {
      return new() {Greetings = new[] {"Guten Tag ViennaNET", "Hello ViennaNET!"}};
    }
  }
}