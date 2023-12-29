using System.Text;
using Confluent.Kafka;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.Extensions;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.KafkaQueue.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(KafkaMessageConverter))]
    internal class KafkaMessageConverterTests
    {
        [Test]
        public void ConvertToKafkaMessage_TextMessageNoKey()
        {
            var srcMessage = new TextMessage { Body = "Body text" };
            srcMessage.Properties.Add("Prop1", "Val1");
            var property = srcMessage.Properties.First();

            var dstMessage = srcMessage.ConvertToKafkaMessage();

            var keyHeader = dstMessage.Headers.FirstOrDefault(h => h.Key == property.Key);

            Assert.Multiple(() =>
            {
                Assert.That(srcMessage.Body, Is.EqualTo(Encoding.UTF8.GetString(dstMessage.Value)));
                Assert.That(keyHeader, Is.Not.Null);
                Assert.That(property.Value, Is.EqualTo(Encoding.UTF8.GetString(keyHeader!.GetValueBytes())));
                Assert.That(property.Value, Is.EqualTo(Encoding.UTF8.GetString(keyHeader.GetValueBytes())));
            });
        }

        [Test]
        public void ConvertToKafkaMessage_TextMessageIntProperty()
        {
            var srcMessage = new TextMessage { Body = "Body text" };
            srcMessage.Properties.Add("Prop1", 1);
            _ = srcMessage.Properties.First();

            Assert.That(() => srcMessage.ConvertToKafkaMessage(), Throws.InstanceOf<MessagingException>());
        }

        [Test]
        public void ConvertToKafkaMessage_BytesMessageNoKey()
        {
            var srcMessage = new BytesMessage { Body = [0xDE, 0xAD, 0xBE, 0xEF] };

            var dstMessage = srcMessage.ConvertToKafkaMessage();

            var idHeader =
                dstMessage.Headers.FirstOrDefault(h => h.Key == KafkaMessageConverter.CorrelationIdHeaderName);

            Assert.Multiple(() =>
            {
                Assert.That(dstMessage.Value, Is.EqualTo(srcMessage.Body).AsCollection);
                Assert.That(idHeader, Is.Not.Null);
                Assert.That(srcMessage.MessageId, Is.EqualTo(Encoding.UTF8.GetString(idHeader!.GetValueBytes())));
            });
        }

        public static object[] ConvertToKafkaMessage_TextMessageHasKey_Source =
        [
            new object[] { true, BitConverter.GetBytes(true) }, new object[] { 7, BitConverter.GetBytes(7) },
            new object[] { 7.7, BitConverter.GetBytes(7.7) }, new object[] { "qwe", Encoding.UTF8.GetBytes("qwe") },
            new object[] { new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, new byte[] { 0xDE, 0xAD, 0xBE, 0xEF } },
            new object[] { null!, null! }
        ];

        [TestCaseSource(nameof(ConvertToKafkaMessage_TextMessageHasKey_Source))]
        public void ConvertToKafkaMessage_TextMessageHasKey(object key, byte[] keyBytes)
        {
            var srcMessage = new TextMessage { Body = "Body text" };
            srcMessage.Properties.Add("Prop1", "Val1");
            var property = srcMessage.Properties.First();
            srcMessage.Properties.Add("KafkaKey", key);

            var dstMessage = srcMessage.ConvertToKafkaMessage();

            var propHeader = dstMessage.Headers.FirstOrDefault(h => h.Key == property.Key);
            var idHeader =
                dstMessage.Headers.FirstOrDefault(h => h.Key == KafkaMessageConverter.CorrelationIdHeaderName);

            Assert.Multiple(() =>
            {
                Assert.That(dstMessage.Key, Is.EqualTo(keyBytes).AsCollection);
                Assert.That(srcMessage.Body, Is.EqualTo(Encoding.UTF8.GetString(dstMessage.Value)));
                Assert.That(propHeader, Is.Not.Null);
                Assert.That(property.Value, Is.EqualTo(Encoding.UTF8.GetString(propHeader!.GetValueBytes())));
                Assert.That(idHeader, Is.Not.Null);
                Assert.That(srcMessage.MessageId, Is.EqualTo(Encoding.UTF8.GetString(idHeader!.GetValueBytes())));
            });
        }

        [Test]
        public void ConvertToKafkaMessage_TextMessageInvalidKey()
        {
            var srcMessage = new TextMessage { Body = "Body text" };
            srcMessage.Properties.Add("KafkaKey", new object());

            Assert.That(() => srcMessage.ConvertToKafkaMessage(), Throws.InstanceOf<MessagingException>());
        }

        [Test]
        public void ConvertToKafkaMessage_TestMessageCorrelId()
        {
            var srcMessage = new TextMessage { Body = "Body text", CorrelationId = "qwe123" };

            var dstMessage = srcMessage.ConvertToKafkaMessage();

            var idHeader =
                dstMessage.Headers.FirstOrDefault(h => h.Key == KafkaMessageConverter.CorrelationIdHeaderName);

            Assert.Multiple(() =>
            {
                Assert.That(srcMessage.Body, Is.EqualTo(Encoding.UTF8.GetString(dstMessage.Value)));
                Assert.That(idHeader, Is.Not.Null);
                Assert.That(srcMessage.CorrelationId, Is.EqualTo(Encoding.UTF8.GetString(idHeader!.GetValueBytes())));
            });
        }

        [Test]
        public void ConvertToBaseMessage_HasKey()
        {
            const string headerValue = "header value";
            var result = new ConsumeResult<byte[], byte[]>
            {
                Topic = "kafkaTopic",
                Message = new Message<byte[], byte[]>
                {
                    Key = [0xDE, 0xAD, 0xBE, 0xEF],
                    Value = [0xBE, 0xEF, 0xDE, 0xAD],
                    Timestamp = new Timestamp(DateTime.Now),
                    Headers = new Headers { { "Key1", Encoding.UTF8.GetBytes(headerValue) } }
                }
            };

            var message = result.ConvertToBaseMessage();

            Assert.Multiple(() =>
            {
                Assert.That(message, Is.InstanceOf<BytesMessage>());
                Assert.That(result.Topic, Is.EqualTo(message.ReplyQueue));
                Assert.That(message.GetMessageBodyAsBytes(), Is.EqualTo(result.Message.Value).AsCollection);
                Assert.That(result.Message.Timestamp.UtcDateTime, Is.EqualTo(message.SendDateTime!.Value));
                Assert.That(message.Properties, Has.Count.EqualTo(2));
                Assert.That(message.Properties.Keys, Has.Member(result.Message.Headers[0].Key));
                Assert.That(message.Properties["Key1"], Is.EqualTo(headerValue));
                Assert.That(message.Properties.Keys, Has.Member("KafkaKey"));
                Assert.That((byte[])message.Properties["KafkaKey"], Is.EqualTo(result.Message.Key).AsCollection);
            });
        }

        [Test]
        public void ConvertToBaseMessage_HasCorrelId()
        {
            const string correlId = "qwe123";
            var result = new ConsumeResult<byte[], byte[]>
            {
                Topic = "kafkaTopic",
                Message = new Message<byte[], byte[]>
                {
                    Value = [0xBE, 0xEF, 0xDE, 0xAD],
                    Headers = new Headers
                    {
                        { KafkaMessageConverter.CorrelationIdHeaderName, Encoding.UTF8.GetBytes(correlId) }
                    }
                }
            };

            var message = result.ConvertToBaseMessage();

            Assert.Multiple(() =>
            {
                Assert.That(message, Is.InstanceOf<BytesMessage>());
                Assert.That(result.Topic, Is.EqualTo(message.ReplyQueue));
                Assert.That(message.GetMessageBodyAsBytes(), Is.EqualTo(result.Message.Value).AsCollection);
                Assert.That(message.CorrelationId, Is.EqualTo(correlId));
                Assert.That(message.MessageId, Is.EqualTo(correlId));
            });
        }
    }
}