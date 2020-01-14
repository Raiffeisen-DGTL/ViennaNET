using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Company.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Company.WebApi.Core.DefaultConfiguration.HttpClients.Handlers
{
  /// <summary>
  /// Обработчик Http-запросов, пробрасывающий заголовок Authorization из входящего запроса в исходящий
  /// </summary>
  public class RequestAuthorizationHeaderHandler : DelegatingHandler
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestAuthorizationHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor.ThrowIfNull(nameof(httpContextAccessor));
    }

    private static void SetHeaderParameter(
      HttpRequestMessage request, string headerId, HttpRequest incomingRequest, Func<HttpRequest, string> getParameter)
    {
      if (request.Headers.Contains(headerId))
      {
        request.Headers.Remove(headerId);
      }

      var parameter = getParameter(incomingRequest);
      if (!string.IsNullOrEmpty(parameter))
      {
        request.Headers.Add(headerId, parameter);
      }
    }

    private static string GetAuthHeader(HttpRequest incomingRequest)
    {
      // TODO: нужно ли здесь ходить в security за токеном, если да, то как? NTLMа то нет.
      return incomingRequest != null
        ? incomingRequest.Headers[HeaderNames.Authorization]
          .ToString()
        : string.Empty;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var context = _httpContextAccessor.HttpContext;
      var incomingRequest = context?.Request;

      SetHeaderParameter(request, HeaderNames.Authorization, incomingRequest, GetAuthHeader);

      return await base.SendAsync(request, cancellationToken);
    }
  }
}