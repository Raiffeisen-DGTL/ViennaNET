﻿using NUnit.Framework;

namespace ViennaNET.Mediator.Tests.Fake.Handlers
{
  internal class OtherRequestWithSeveralHandlersHandler : IMessageHandler<RequestWithSeveralHandlers, int>
  {
    private const int Result = 10;

    public int Handle(RequestWithSeveralHandlers message)
    {
      TestContext.WriteLine(
        $"The {nameof(OtherRequestWithSeveralHandlersHandler)} handled {message.GetType().FullName}");
      return Result;
    }
  }
}