using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ViennaNET.KasperskyScanEngine.Client.Models;

namespace ViennaNET.KasperskyScanEngine.Client.Tests;

public class KseApiTests
{
    private Mock<IOptionsSnapshot<JsonSerializerOptions>> JsonOptionsMock { get; set; } = null!;
    private Mock<HttpMessageHandler> HandlerMock { get; set; } = null!;
    private Mock<HttpClient> ClientMock { get; set; } = null!;

    private void MockHandlerResponse(string? content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        SetupHandlerMock(new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content ?? string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json)
        });
    }

    private void MockClientResponse(string? content, HttpStatusCode statusCode = HttpStatusCode.Created)
    {
        ClientMock.Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new
                HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content ?? string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json)
            });
    }

    private void SetupHandlerMock(HttpResponseMessage response)
    {
        HandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response)
            .Verifiable();
    }

    [SetUp]
    public void Setup()
    {
        var jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonValueConverterBooleanString() },
            WriteIndented = false,
            AllowTrailingCommas = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = KseClientSerializerContext.Default
        };

        JsonOptionsMock = new Mock<IOptionsSnapshot<JsonSerializerOptions>>();
        HandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        ClientMock = new Mock<HttpClient>(HandlerMock.Object);

        ClientMock.Object.BaseAddress = new Uri("https://tst.raif.ru");
        JsonOptionsMock.Setup(o => o.Get(IKasperskyScanEngineApi.JsonSerializerOptionsName)).Returns(jsonOptions);
    }

    [Test]
    public void Ctor_DoesNotThrow()
    {
        Assert.That(() => new KseApi(ClientMock.Object, JsonOptionsMock.Object), Throws.Nothing);
    }

    [Test]
    public void Ctor_Throw_ClientParam_ArgumentNullException()
    {
        const string expectedExMessage = "Value cannot be null. (Parameter 'client')";

        Assert.That(() => new KseApi(null!, JsonOptionsMock.Object),
            Throws.ArgumentNullException.And.Message.EqualTo(expectedExMessage));
    }

    [Test]
    public void Ctor_Throw_OptionsSnapshotParam_ArgumentNullException()
    {
        const string expectedExMessage = "Value cannot be null. (Parameter 'optionsSnapshot')";

        Assert.That(() => new KseApi(ClientMock.Object, null!),
            Throws.ArgumentNullException.And.Message.EqualTo(expectedExMessage));
    }

    [Test]
    public async Task GetKsnInfoAsync_Returns_KsnInfo_ConnectedStatus()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);
        SetupHandlerMock(new HttpResponseMessage
        {
            Content = new StringContent(Given.KsnInfo.ConnectedJson, Encoding.UTF8, MediaTypeNames.Application.Json)
        });

        var response = await api.GetKsnInfoAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Status, Is.EqualTo("Connected"));
            Assert.That(response?.WhiteApplications, Is.EqualTo(6882322203));
            Assert.That(response?.MalwareApplications, Is.EqualTo(2125763035));
            Assert.That(response?.BlockedThreats, Is.EqualTo(14542499));
            Assert.That(response?.Region, Is.EqualTo("global"));
            Assert.That(response?.ResponseTimestamp, Is.EqualTo("2024 02 10 12:15:02"));
        });
    }

    [Test]
    public async Task GetKsnInfoAsync_Returns_KsnInfo_TurnedOffStatus()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);
        SetupHandlerMock(new HttpResponseMessage
        {
            Content = new StringContent(Given.KsnInfo.TurnedOffJson, Encoding.UTF8, MediaTypeNames.Application.Json)
        });

        var response = await api.GetKsnInfoAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Status, Is.EqualTo("KSN turned off"));
            Assert.That(response?.WhiteApplications, Is.Null);
            Assert.That(response?.MalwareApplications, Is.Null);
            Assert.That(response?.BlockedThreats, Is.Null);
            Assert.That(response?.Region, Is.Null);
            Assert.That(response?.ResponseTimestamp, Is.Null);
        });
    }

    [Test]
    public async Task GetUpdateStatusAsync_Returns_UpdateStatusResponse_NotStarted()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);
        SetupHandlerMock(new HttpResponseMessage
        {
            Content = new StringContent(Given.UpdateStatus.NotStartedJson, Encoding.UTF8,
                MediaTypeNames.Application.Json)
        });

        var response = await api.GetUpdateStatusAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Status, Is.EqualTo("not started"));
            Assert.That(response?.LastUpdateResult, Is.EqualTo("success"));
            Assert.That(response?.LastUpdateTime, Is.EqualTo("21:03:53 30.01.2019"));
            Assert.That(response?.Progress, Is.Null);
            Assert.That(response?.ActionNeeded, Is.Null);
            Assert.That(response?.ActionApplyPeriod, Is.Null);
        });
    }

    [Test]
    public async Task GetUpdateStatusAsync_Returns_UpdateStatusResponse_InProgress()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);
        SetupHandlerMock(new HttpResponseMessage
        {
            Content = new StringContent(Given.UpdateStatus.InProgressJson, Encoding.UTF8,
                MediaTypeNames.Application.Json)
        });

        var response = await api.GetUpdateStatusAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Status, Is.EqualTo("in progress"));
            Assert.That(response?.Progress, Is.EqualTo("100%"));
            Assert.That(response?.ActionNeeded, Is.EqualTo("Product restart needed"));
            Assert.That(response?.ActionApplyPeriod, Is.EqualTo(2));
            Assert.That(response?.LastUpdateResult, Is.Null);
            Assert.That(response?.LastUpdateTime, Is.Null);
        });
    }

    [Test]
    public async Task PostUpdateStartAsync_Returns_UpdateStatusResponse_Started()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);
        MockClientResponse(Given.UpdateStatus.StartedJson, HttpStatusCode.OK);

        var response = await api.PostUpdateStartAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Status, Is.EqualTo("update started"));
            Assert.That(response?.LastUpdateResult, Is.Null);
            Assert.That(response?.LastUpdateTime, Is.Null);
            Assert.That(response?.Progress, Is.Null);
            Assert.That(response?.ActionNeeded, Is.Null);
            Assert.That(response?.ActionApplyPeriod, Is.Null);
        });
    }

    [Test]
    public async Task PostUpdateStartAsync_Returns_UpdateStatusResponse_Method_Not_Allowed()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(null, HttpStatusCode.MethodNotAllowed);


        await Assert.ThatAsync(() => api.PostUpdateStartAsync(), Throws.InstanceOf<HttpRequestException>());
    }

    [Test]
    public async Task PostClearStatisticsAsync_Throws_InvalidOperationException_Unexpected()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(null, HttpStatusCode.BadGateway);

        await Assert.ThatAsync(() => api.PostClearStatisticsAsync(), Throws.InstanceOf<HttpRequestException>());
    }

    [Test]
    public async Task PostClearStatisticsAsync_Returns_Cleared()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.ErrorResponse.StatisticsCleared, HttpStatusCode.OK);

        var response = await api.PostClearStatisticsAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Message, Is.EqualTo("CLEARED"));
        });
    }

    [Test]
    public async Task GetStatisticsAsync_Returns_Statistics()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockHandlerResponse(Given.StatisticsResponse.OkJson);

        var response = await api.GetStatisticsAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Data.TotalRequests, Is.EqualTo(3));
            Assert.That(response?.Data.InfectedRequests, Is.EqualTo(3));
            Assert.That(response?.Data.ProtectedRequests, Is.EqualTo(3));
            Assert.That(response?.Data.ErrorRequests, Is.EqualTo(0));
            Assert.That(response?.Data.EngineErrors, Is.EqualTo(0));
            Assert.That(response?.Data.ProcessedData, Is.EqualTo(204));
            Assert.That(response?.Data.InfectedData, Is.EqualTo(204));
            Assert.That(response?.Data.ProcessedUrls, Is.EqualTo(1));
            Assert.That(response?.Data.InfectedUrls, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task GetLicenseInfoAsync_Returns_LicenseInfo_OfflineMode()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockHandlerResponse(Given.LicenseInfo.OkOfflineModeJson);

        var response = await api.GetLicenseInfoAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.KeyFileName, Is.EqualTo("test.key"));
            Assert.That(response?.ExpirationDate, Is.EqualTo("05.12.2020"));
            Assert.That(response?.IsOfflineActivationMode, Is.True);
            Assert.That(response?.ActivationCode, Is.Null);
            Assert.That(response?.TicketExpired, Is.Null);
        });
    }

    [Test]
    public async Task GetLicenseInfoAsync_Returns_LicenseInfo_OnlineMode()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockHandlerResponse(Given.LicenseInfo.OkOnlineModeJson);

        var response = await api.GetLicenseInfoAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.ActivationCode, Is.EqualTo("TEST-*****-*****-12345"));
            Assert.That(response?.ExpirationDate, Is.EqualTo("05.12.2020"));
            Assert.That(response?.TicketExpired, Is.EqualTo("The license ticket has expired."));
            Assert.That(response?.IsOfflineActivationMode, Is.False);
            Assert.That(response?.KeyFileName, Is.Null);
        });
    }

    [Test]
    public async Task GetVersionAsync_Returns_Version()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockHandlerResponse(Given.VersionResponse.OkJson);

        var response = await api.GetVersionAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.KavSdkVersion.ToString(), Is.EqualTo("8.8.2.58"));
        });
    }

    [Test]
    public async Task GetBaseDateAsync_Returns_BaseDate()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockHandlerResponse(Given.BasesDateResponse.OkJson);

        var response = await api.GetBaseDateAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.DatabaseVersion, Is.EqualTo("30.01.2019 18:38 GMT"));
        });
    }

    [Test]
    public async Task PostCheckUrlAsync_Returns_CheckUrlResponse()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.CheckUrlResponse.OkDetectJson, HttpStatusCode.OK);

        var response = await api.PostCheckUrlAsync(new CheckUrlRequest("http://bug.qainfo.ru/TesT/Wmuf_w"));

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Url.ToString(), Is.EqualTo("http://bug.qainfo.ru/TesT/Wmuf_w"));
            Assert.That(response?.IsClean, Is.False);
            Assert.That(response?.IsDetect, Is.True);
            Assert.That(response?.ScanResult, Is.EqualTo("DETECT"));
            Assert.That(response?.DetectionName, Is.EqualTo("PHISHING_URL"));
        });
    }

    [Test]
    public async Task PostCheckUrlAsync_Returns_CheckUrlResponse_Clean()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.CheckUrlResponse.OkCleanJson, HttpStatusCode.OK);

        var response = await api.PostCheckUrlAsync(new CheckUrlRequest("http://bug.qainfo.ru/TesT/Wmuf_w"));

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Url.ToString(), Is.EqualTo("http://bug.qainfo.ru/TesT/Wmuf_w"));
            Assert.That(response?.IsClean, Is.True);
            Assert.That(response?.IsDetect, Is.False);
            Assert.That(response?.ScanResult, Is.EqualTo("CLEAN"));
            Assert.That(response?.DetectionName, Is.Null);
        });
    }

    [Test]
    public async Task PostCheckUrlAsync_Throws_InvalidOperationException()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.ErrorResponse.BadRequest, HttpStatusCode.BadRequest);

        await Assert.ThatAsync(() => api.PostCheckUrlAsync(new CheckUrlRequest("http://bug.qainfo.ru/TesT/Wmuf_w")),
            Throws.InvalidOperationException.And.Message.EqualTo("Тестовое сообщение об ошибке"));
    }

    [Test]
    public async Task PostCheckUrlAsync_Throws_InvalidOperationException_Unexpected()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(null, HttpStatusCode.BadGateway);

        await Assert.ThatAsync(() => api.PostCheckUrlAsync(new CheckUrlRequest("http://bug.qainfo.ru/TesT/Wmuf_w")),
            Throws.InvalidOperationException.And.Message
                .EqualTo("Непредвиденная ошибка при выполнении запроса: checkurl"));
    }

    [Test]
    public async Task PostScanMemoryAsync_Returns_ScanMemoryResponse_RootObj()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.ScanMemoryResponse.OkWithRootObjDetectJson, HttpStatusCode.OK);

        var bytes = Encoding.UTF8.GetBytes("WDVPIVAlQEFQWzRcUFpYNTQoUF");
        var response = await api.PostScanMemoryAsync(new ScanMemoryRequest(bytes, "test.txt"));

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Content, Is.EqualTo("memory"));
            Assert.That(response?.ScanResult, Is.EqualTo("DETECT"));
            Assert.That(response?.IsClean, Is.False);
            Assert.That(response?.IsDetect, Is.True);
            Assert.That(response?.Name, Is.EqualTo("test.txt"));
            Assert.That(response?.DetectionName, Is.EqualTo("EICAR-Test-File"));
            Assert.That(response?.IsMacrosDetected, Is.False);
            Assert.That(response?.SubScanResults, Is.Null);
        });
    }

    [Test]
    public async Task PostScanMemoryAsync_Returns_ScanMemoryResponse_RootObj_Clean()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.ScanMemoryResponse.OkWithRootObjCleanJson, HttpStatusCode.OK);

        var bytes = Encoding.UTF8.GetBytes("WDVPIVAlQEFQWzRcUFpYNTQoUF");
        var response = await api.PostScanMemoryAsync(new ScanMemoryRequest(bytes, "test.txt"));

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Content, Is.EqualTo("memory"));
            Assert.That(response?.ScanResult, Is.EqualTo("CLEAN"));
            Assert.That(response?.IsClean, Is.True);
            Assert.That(response?.IsDetect, Is.False);
            Assert.That(response?.Name, Is.EqualTo("test.txt"));
            Assert.That(response?.DetectionName, Is.Null);
            Assert.That(response?.IsMacrosDetected, Is.False);
            Assert.That(response?.SubScanResults, Is.Null);
        });
    }

    [Test]
    public async Task PostScanMemoryAsync_Stream_Returns_ScanMemoryResponse_RootObj()
    {
        const string file = "WDVPIVQEFzRcUFpYNTQoUF4pN0NDKTd9JEVJQ0FSLVNUQU5EQVJELUFOVElWSVJVUy1URVNULUZJTEUhJEgrSCo=";
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.ScanMemoryResponse.OkWithRootObjDetectJson, HttpStatusCode.OK);

        var response = await api.PostScanMemoryAsync(new MemoryStream(Convert.FromBase64String(file)), "test.txt");

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Content, Is.EqualTo("memory"));
            Assert.That(response?.ScanResult, Is.EqualTo("DETECT"));
            Assert.That(response?.Name, Is.EqualTo("test.txt"));
            Assert.That(response?.DetectionName, Is.EqualTo("EICAR-Test-File"));
            Assert.That(response?.IsMacrosDetected, Is.False);
            Assert.That(response?.SubScanResults, Is.Null);
        });
    }

    [Test]
    public async Task PostScanMemoryAsync_Stream_Throws_ArgumentNullException()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.ScanMemoryResponse.OkWithRootObjDetectJson, HttpStatusCode.OK);

        await Assert.ThatAsync(() => api.PostScanMemoryAsync(null!, "test.txt"), Throws.ArgumentNullException);
    }

    [Test]
    public async Task PostScanMemoryAsync_Returns_ScanMemoryResponse_WithSubScan()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.ScanMemoryResponse.OkWithSubScanResultsJson, HttpStatusCode.OK);

        var bytes = Encoding.UTF8.GetBytes("WDVPIVAlQEFQWzRcUFpYNTQoUF");
        var response = await api.PostScanMemoryAsync(new ScanMemoryRequest(bytes, "test.zip"));

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Content, Is.EqualTo("memory"));
            Assert.That(response?.ScanResult, Is.EqualTo("DETECT"));
            Assert.That(response?.Name, Is.EqualTo("test.zip"));
            Assert.That(response?.DetectionName, Is.EqualTo("EICAR-Test-File"));
            Assert.That(response?.IsMacrosDetected, Is.False);
            Assert.That(response?.SubScanResults, Is.Not.Null);
            Assert.That(response?.SubScanResults, Has.Length.EqualTo(1));
            Assert.That(response?.SubScanResults?[0].Content, Is.EqualTo("test.docx"));
            Assert.That(response?.SubScanResults?[0].ScanResult, Is.EqualTo("DETECT"));
            Assert.That(response?.SubScanResults?[0].DetectionName, Is.EqualTo("EICAR-Test-File"));
            Assert.That(response?.SubScanResults?[0].IsMacrosDetected, Is.True);
        });
    }

    [Test]
    public async Task PostScanFileAsync_Returns_ScanFileResponse_RootObj()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.ScanFileResponse.OkWithRootObjDetectJson, HttpStatusCode.OK);

        var response = await api.PostScanFileAsync(new ScanFileRequest("test.txt"));

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Path, Is.EqualTo("test.txt"));
            Assert.That(response?.ScanResult, Is.EqualTo("DETECT"));
            Assert.That(response?.DetectionName, Is.EqualTo("EICAR-Test-File"));
            Assert.That(response?.IsClean, Is.False);
            Assert.That(response?.IsDetect, Is.True);
            Assert.That(response?.IsMacrosDetected, Is.False);
            Assert.That(response?.SubScanResults, Is.Null);
        });
    }

    [Test]
    public async Task PostScanFileAsync_Returns_ScanFileResponse_RootObj_Clean()
    {
        var api = new KseApi(ClientMock.Object, JsonOptionsMock.Object);

        MockClientResponse(Given.ScanFileResponse.OkWithRootObjCleanJson, HttpStatusCode.OK);

        var response = await api.PostScanFileAsync(new ScanFileRequest("test.txt"));

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.Path, Is.EqualTo("test.txt"));
            Assert.That(response?.ScanResult, Is.EqualTo("CLEAN"));
            Assert.That(response?.DetectionName, Is.Null);
            Assert.That(response?.IsClean, Is.True);
            Assert.That(response?.IsDetect, Is.False);
            Assert.That(response?.IsMacrosDetected, Is.False);
            Assert.That(response?.SubScanResults, Is.Null);
        });
    }
}