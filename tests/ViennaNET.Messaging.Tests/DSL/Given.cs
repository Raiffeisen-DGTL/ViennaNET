using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Messaging.Tests.Unit.DSL;

namespace ViennaNET.Messaging.Tests.DSL
{
  internal static class Given
  {
    public static QueueReactorFactoryBuilder QueueReactorFactory => new();

    public static MessageAdapterBuilder MessageAdapter => new();

    public static QueuePollingReactorBuilder QueuePollingReactor => new();

    public static QueueSubscribedReactorBuilder QueueSubscribedReactor => new();

    public static MessageAdapterFactoryBuilder MessageAdapterFactory => new();

    public static MessageAdapterConstructorMock MessageAdapterConstructor => new();

    public static CallContextFactoryMock CallContextFactory => new();

    public static MessagingConnectionCheckerBuilder MessagingConnectionChecker =>
      new();

    public static MessagingComponentFactoryBuilder MessagingComponentFactory => new();

    public static MessageReceiverBuilder MessageReceiver => new();

    public static ILoggerFactory FakeLoggerFactory
    {
      get
      {
        var factory = new Mock<ILoggerFactory> { DefaultValue = DefaultValue.Mock };
        return factory.Object;
      }
    }
  }
}