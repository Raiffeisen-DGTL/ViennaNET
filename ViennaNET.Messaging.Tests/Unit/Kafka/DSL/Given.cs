namespace ViennaNET.Messaging.Tests.Unit.Kafka.DSL
{
  internal static class Given
  {
    public static KafkaQueueMessageAdapterBuilder KafkaQueueMessageAdapter => new KafkaQueueMessageAdapterBuilder();

    public static KafkaConnectionFactoryMock KafkaConnectionFactory => new KafkaConnectionFactoryMock();
  }
}
