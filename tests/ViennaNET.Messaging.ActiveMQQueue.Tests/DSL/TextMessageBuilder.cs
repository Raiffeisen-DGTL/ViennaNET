using System.Collections;
using System.Collections.Generic;
using Apache.NMS;
using Moq;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
  internal class TextMessageBuilder
  {
    private string _body;
    private IDictionary<string, object> _props = new Dictionary<string, object>();
    private string _replyQueue;

    public TextMessageBuilder WithProperties(IDictionary<string, object> props)
    {
      _props = props;
      return this;
    }

    public TextMessageBuilder WithReplyQueue(string queueName)
    {
      _replyQueue = queueName;
      return this;
    }

    public TextMessageBuilder WithBody(string body)
    {
      _body = body;
      return this;
    }

    public ITextMessage Please()
    {
      var keys = new ArrayList();
      foreach (var item in _props.Keys)
      {
        keys.Add(item);
      }

      var values = new ArrayList();
      foreach (var item in _props.Values)
      {
        values.Add(item);
      }

      var propertiesMock = new Mock<IPrimitiveMap>();
      propertiesMock.SetupAllProperties();
      propertiesMock.Setup(x => x.Count).Returns(_props.Count);
      propertiesMock.Setup(x => x.Keys).Returns(keys);
      propertiesMock.Setup(x => x.Values).Returns(values);
      propertiesMock.Setup(x => x[It.IsAny<string>()])
        .Returns((string key) => _props[key]);


      var mock = new Mock<ITextMessage>();
      mock.SetupAllProperties();

      if (!string.IsNullOrEmpty(_replyQueue))
      {
        mock
          .SetupGet(m => m.NMSReplyTo)
          .Returns(Mock.Of<IQueue>(m => m.QueueName == _replyQueue));
      }

      mock
        .SetupGet(m => m.Properties)
        .Returns(propertiesMock.Object);

      var retVal = mock.Object;
      retVal.Text = _body;
      return retVal;
    }
  }
}