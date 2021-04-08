using System.Collections.Generic;
using System.Linq;
using IBM.XMS;
using Moq;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests.DSL
{
  internal class TextMessageBuilder
  {
    private string _body;
    private IDictionary<string, object> _props;
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
      var mock = new Mock<ITextMessage>();
      mock.SetupAllProperties();
      mock
        .Setup(m => m.GetObjectProperty(It.IsAny<string>()))
        .Returns<string>(p => _props[p]);
      mock
        .SetupGet(m => m.PropertyNames)
        .Returns(_props == null ? Enumerable.Empty<string>().GetEnumerator() : _props.Keys.GetEnumerator());

      if (!string.IsNullOrEmpty(_replyQueue))
      {
        mock
          .SetupGet(m => m.JMSReplyTo)
          .Returns(Mock.Of<IDestination>(m => m.Name == _replyQueue));
      }

      var retVal = mock.Object;
      retVal.Text = _body;
      return retVal;
    }
  }
}