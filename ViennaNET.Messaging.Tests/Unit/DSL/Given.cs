namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal static class Given
  {
    public static QueueReactorFactoryBuilder QueueReactorFactory => new QueueReactorFactoryBuilder();

    public static MessageAdapterBuilder MessageAdapter => new MessageAdapterBuilder();

    public static QueuePollingReactorBuilder QueuePollingReactor => new QueuePollingReactorBuilder();

    public static QueueSubscribedReactorBuilder QueueSubscribedReactor => new QueueSubscribedReactorBuilder();
  }
}
