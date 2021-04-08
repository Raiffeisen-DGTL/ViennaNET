using Apache.NMS;
using Moq;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
  internal class SessionBuilder
  {
    public ISession Please()
    {
      var mock = new Mock<ISession>();
      mock
        .Setup(m => m.CreateBytesMessage())
        .Returns(Given.BytesMessage.Please());
      mock
        .Setup(m => m.CreateTextMessage(It.IsAny<string>()))
        .Returns<string>(body => Given.TextMessage.WithBody(body).Please());
      mock
        .Setup(m => m.GetQueue(It.IsAny<string>()))
        .Returns<string>(queueName => Mock.Of<IQueue>(m => m.QueueName == queueName));

      return mock.Object;
    }
  }
}