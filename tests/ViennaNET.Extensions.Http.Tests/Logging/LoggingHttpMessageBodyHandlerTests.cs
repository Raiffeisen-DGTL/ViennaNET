using Microsoft.Extensions.Logging;
using Moq;
using ViennaNET.Extensions.Http.Logging;
using ViennaNET.Extensions.Http.Tests.Fakes;

namespace ViennaNET.Extensions.Http.Tests.Logging;

public class LoggingHttpMessageBodyHandlerTests
{
    [Test]
    public void Ctor_DoesNotThrow()
    {
        Assert.That(() => new LoggingHttpMessageBodyHandler(Mock.Of<ILogger<LoggingHttpMessageBodyHandler>>()),
            Throws.Nothing);
    }

    [Test]
    public void Ctor_Throw_ArgumentNullException()
    {
        const string expectedExMessage = "Value cannot be null. (Parameter 'logger')";

        Assert.That(() => new LoggingHttpMessageBodyHandler(null!),
            Throws.ArgumentNullException.And.Message.EqualTo(expectedExMessage));
    }

    [Test]
    public void Ctor_WithHandler_DoesNotThrow()
    {
        Assert.That(
            () => new LoggingHttpMessageBodyHandler(Mock.Of<HttpMessageHandler>(),
                Mock.Of<ILogger<LoggingHttpMessageBodyHandler>>()), Throws.Nothing);
    }

    [Test]
    public void Ctor_WithHandler_Throw_ArgumentNullException()
    {
        const string expectedExMessage = "Value cannot be null. (Parameter 'logger')";

        Assert.That(() => new LoggingHttpMessageBodyHandler(Mock.Of<HttpMessageHandler>(), null!),
            Throws.ArgumentNullException.And.Message.EqualTo(expectedExMessage));
    }

    [Test]
    public void Ctor_WithNullHandler_Throw_ArgumentNullException()
    {
        const string expectedExMessage = "Value cannot be null. (Parameter 'value')";

        Assert.That(() => new LoggingHttpMessageBodyHandler(null!, Mock.Of<ILogger<LoggingHttpMessageBodyHandler>>()),
            Throws.ArgumentNullException.And.Message.EqualTo(expectedExMessage));
    }

    [Test]
    public async Task SendAsync_Call_Log_RequestPayload_Once_And_ResponsePayload_Once()
    {
        var logger = Mock.Of<ILogger<LoggingHttpMessageBodyHandler>>(log => log.IsEnabled(LogLevel.Trace) == true);
        var requestEventId = new EventId(104, "RequestPayload");
        var responseEventId = new EventId(105, "ResponsePayload");
        var mock = Mock.Get(logger);
        var handler = new LoggingHttpMessageBodyHandler(new FakeOkWithBodyHttpMessageHandler(), logger);
        var clientMock = new Mock<HttpClient>(handler);

        clientMock.Setup(instance => instance.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .CallBase();

        await clientMock.Object.PostAsync("http://localhost", new StringContent("Test SendAsync Body"));

        Assert.Multiple(() =>
        {
            mock.Verify(
                instance => instance.Log(
                    LogLevel.Trace,
                    requestEventId,
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);

            mock.Verify(
                instance => instance.Log(
                    LogLevel.Trace,
                    responseEventId,
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
        });
    }

    [Test]
    public async Task SendAsync_Call_Log_RequestPayload_Never_And_ResponsePayload_Once()
    {
        var logger = Mock.Of<ILogger<LoggingHttpMessageBodyHandler>>(log => log.IsEnabled(LogLevel.Trace) == true);
        var requestEventId = new EventId(104, "RequestPayload");
        var responseEventId = new EventId(105, "ResponsePayload");
        var mock = Mock.Get(logger);
        var handler = new LoggingHttpMessageBodyHandler(new FakeOkWithBodyHttpMessageHandler(), logger);
        var clientMock = new Mock<HttpClient>(handler);

        clientMock.Setup(instance => instance.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .CallBase();

        await clientMock.Object.PostAsync("http://localhost", null!);

        Assert.Multiple(() =>
        {
            mock.Verify(
                instance => instance.Log(
                    LogLevel.Trace,
                    requestEventId,
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);

            mock.Verify(
                instance => instance.Log(
                    LogLevel.Trace,
                    responseEventId,
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
        });
    }

    [Test]
    public async Task SendAsync_With_Null_Body_Call_Log_RequestPayload_Never_And_ResponsePayload_Never()
    {
        var logger = Mock.Of<ILogger<LoggingHttpMessageBodyHandler>>(log => log.IsEnabled(LogLevel.Trace) == true);
        var requestEventId = new EventId(104, "RequestPayload");
        var responseEventId = new EventId(105, "ResponsePayload");
        var mock = Mock.Get(logger);
        var handler = new LoggingHttpMessageBodyHandler(new FakeOkWithoutBodyHttpMessageHandler(), logger);
        var clientMock = new Mock<HttpClient>(handler);

        clientMock.Setup(instance => instance.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .CallBase();

        await clientMock.Object.PostAsync("http://localhost", null!);

        Assert.Multiple(() =>
        {
            mock.Verify(
                instance => instance.Log(
                    LogLevel.Trace,
                    requestEventId,
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);

            mock.Verify(
                instance => instance.Log(
                    LogLevel.Trace,
                    responseEventId,
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);
        });
    }

    [Test]
    public async Task SendAsync_With_Empty_Body_Call_Log_RequestPayload_Never_And_ResponsePayload_Never()
    {
        var logger = Mock.Of<ILogger<LoggingHttpMessageBodyHandler>>(log => log.IsEnabled(LogLevel.Trace) == true);
        var requestEventId = new EventId(104, "RequestPayload");
        var responseEventId = new EventId(105, "ResponsePayload");
        var mock = Mock.Get(logger);
        var handler = new LoggingHttpMessageBodyHandler(new FakeOkWithoutBodyHttpMessageHandler(), logger);
        var clientMock = new Mock<HttpClient>(handler);

        clientMock.Setup(instance => instance.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .CallBase();

        await clientMock.Object.PostAsync("http://localhost", new StringContent(string.Empty));

        Assert.Multiple(() =>
        {
            mock.Verify(
                instance => instance.Log(
                    LogLevel.Trace,
                    requestEventId,
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);

            mock.Verify(
                instance => instance.Log(
                    LogLevel.Trace,
                    responseEventId,
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);
        });
    }
}