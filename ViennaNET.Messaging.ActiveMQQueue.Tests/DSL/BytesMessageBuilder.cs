using System;
using Apache.NMS;
using Moq;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
  internal class BytesMessageBuilder
  {
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
        .Setup(m => m.WriteBytes(It.IsAny<byte[]>()))
        .Callback<byte[]>(arr => _body = arr);
      mock
        .SetupGet(m => m.Properties)
        .Returns(() => new Mock<IPrimitiveMap>().Object);


      return mock.Object;
    }
  }
}