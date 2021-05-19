using Microsoft.Extensions.Logging;
using Moq;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal static class Given
  {
    public static QueueReactorFactoryBuilder QueueReactorFactory => new QueueReactorFactoryBuilder();

    public static MessageAdapterBuilder MessageAdapter => new MessageAdapterBuilder();

    public static QueuePollingReactorBuilder QueuePollingReactor => new QueuePollingReactorBuilder();

    public static QueueSubscribedReactorBuilder QueueSubscribedReactor => new QueueSubscribedReactorBuilder();

    public static MessageAdapterFactoryBuilder MessageAdapterFactory => new MessageAdapterFactoryBuilder();

    public static MessageAdapterConstructorMock MessageAdapterConstructor => new MessageAdapterConstructorMock();

    public static CallContextFactoryMock CallContextFactory => new CallContextFactoryMock();

    public static MessagingConnectionCheckerBuilder MessagingConnectionChecker =>
      new MessagingConnectionCheckerBuilder();

    public static MessagingComponentFactoryBuilder MessagingComponentFactory => new MessagingComponentFactoryBuilder();

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