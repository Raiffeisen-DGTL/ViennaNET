using IBM.XMS;
using Moq;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests.DSL
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
        .Setup(m => m.CreateQueue(It.IsAny<string>()))
        .Returns<string>(queueName => Mock.Of<IDestination>(m => m.Name == queueName));

      return mock.Object;
    }
  }
}