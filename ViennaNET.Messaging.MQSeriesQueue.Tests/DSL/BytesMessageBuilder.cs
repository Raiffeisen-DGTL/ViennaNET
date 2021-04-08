using System;
using System.Collections.Generic;
using IBM.XMS;
using Moq;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests.DSL
{
  internal class BytesMessageBuilder
  {
    private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
    private byte[] _body = new byte[0];

    public BytesMessageBuilder WithBody(byte[] body)
    {
      _body = body;
      return this;
    }

    public IBytesMessage Please()
    {
      var mock = new Mock<IBytesMessage>();
      mock.SetupAllProperties();
      mock
        .SetupGet(m => m.BodyLength)
        .Returns(() => _body.Length);
      mock
        .Setup(m => m.ReadBytes(It.IsAny<byte[]>()))
        .Returns<byte[]>(arr =>
        {
          Array.Copy(_body, arr, _body.Length);
          return _body.Length;
        });
      mock
        .SetupGet(m => m.PropertyNames)
        .Returns(() => _properties.Keys.GetEnumerator());
      mock
        .Setup(m => m.SetObjectProperty(It.IsAny<string>(), It.IsAny<object>()))
        .Callback<string, object>((k, v) => _properties.Add(k, v));
      mock
        .Setup(m => m.GetObjectProperty(It.IsAny<string>()))
        .Returns<string>(k => _properties[k]);
      mock
        .Setup(m => m.WriteBytes(It.IsAny<byte[]>()))
        .Callback<byte[]>(arr => _body = arr);

      return mock.Object;
    }
  }
}