using RabbitMQ.Client;

namespace ViennaNET.Messaging.RabbitMQQueue
{
  internal interface IRabbitMqConnectionFactory
  {
    IConnection Create(RabbitMqQueueConfiguration config);
  }
}