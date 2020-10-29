namespace ViennaNET.Messaging.Tests.Unit.RabbitMq.DSL
{
  internal static class Given
  {
    public static RabbitMqQueueMessageAdapterBuilder RabbitMqQueueMessageAdapter => new RabbitMqQueueMessageAdapterBuilder();

    public static AdvancedBusFactoryMock AdvancedBusFactory => new AdvancedBusFactoryMock();
  }
}
