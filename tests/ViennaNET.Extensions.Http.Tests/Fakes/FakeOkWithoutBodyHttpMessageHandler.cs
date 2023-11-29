using System.Net;

namespace ViennaNET.Extensions.Http.Tests.Fakes;

public class FakeOkWithoutBodyHttpMessageHandler : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = null });
    }
}