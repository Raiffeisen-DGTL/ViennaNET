using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Messaging.Diagnostic;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class MessagingConnectionCheckerBuilder
  {
    private readonly LinkedList<IMessageAdapterConstructor>
      _constructors = new LinkedList<IMessageAdapterConstructor>();

    public MessagingConnectionCheckerBuilder WithConstructor(IMessageAdapterConstructor constructor)
    {
      _constructors.AddLast(constructor);
      return this;
    }

    public MessagingConnectionCheckerBuilder WithConstructor(
      Func<MessageAdapterConstructorMock, IMessageAdapterConstructor> builder)
    {
      _constructors.AddLast(builder(new MessageAdapterConstructorMock()));
      return this;
    }

    public MessagingConnectionChecker Please()
    {
      return new MessagingConnectionChecker(_constructors, Mock.Of<ILogger<MessagingConnectionChecker>>());
    }
  }
}