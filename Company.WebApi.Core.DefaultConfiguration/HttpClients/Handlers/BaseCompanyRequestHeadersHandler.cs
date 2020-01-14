using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Company.Utils;
using Microsoft.AspNetCore.Http;

namespace Company.WebApi.Core.DefaultConfiguration.HttpClients.Handlers
{
  /// <summary>
  /// Базовый обработчик Http-запросов, пробрасывающий заголовки CompanyHeaders из входящего запроса в исходящий
  /// </summary>
  public class BaseCompanyRequestHeadersHandler : DelegatingHandler
  {
    protected readonly IHttpContextAccessor _httpContextAccessor;

    public BaseCompanyRequestHeadersHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor.ThrowIfNull(nameof(httpContextAccessor));
    }

    private static string GetRequestId(HttpRequest incomingRequest)
    {
      var defaultRequestId = Guid.NewGuid()
                                 .ToString("N");

      if (incomingRequest == null)
      {
        return defaultRequestId;
      }

      return incomingRequest.Headers.ContainsKey(CompanyHeaders.RequestId)
        ? incomingRequest.Headers[CompanyHeaders.RequestId]
                         .ToString()
        : defaultRequestId;
    }

    private static string GetCallerIp(HttpRequest incomingRequest)
    {
      var isIncomingHasHeader = incomingRequest != null && incomingRequest.Headers.ContainsKey(CompanyHeaders.RequestHeaderCallerIp);

      if (isIncomingHasHeader)
      {
        return incomingRequest.Headers[CompanyHeaders.RequestHeaderCallerIp]
                              .ToString();
      }

      if (incomingRequest?.HttpContext.Connection != null)
      {
        return incomingRequest.HttpContext.Connection.RemoteIpAddress.MapToIPv4()
                              .ToString();
      }

      return string.Empty;
    }

    private static string GetUserId(HttpRequest incomingRequest)
    {
      var isIncomingHasHeader = incomingRequest != null && incomingRequest.Headers.ContainsKey(CompanyHeaders.UserId);

      return isIncomingHasHeader
        ? incomingRequest.Headers[CompanyHeaders.UserId]
                         .ToString()
        : Environment.UserName;
    }

    private static string GetUserDomain(HttpRequest incomingRequest)
    {
      var isIncomingHasHeader = incomingRequest != null && incomingRequest.Headers.ContainsKey(CompanyHeaders.UserDomain);

      return isIncomingHasHeader
        ? incomingRequest.Headers[CompanyHeaders.UserDomain]
                         .ToString()
        : Environment.UserDomainName;
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

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var context = _httpContextAccessor.HttpContext;
      var incomingRequest = context?.Request;

      SetHeaderParameter(request, CompanyHeaders.RequestId, incomingRequest, GetRequestId);
      SetHeaderParameter(request, CompanyHeaders.RequestHeaderCallerIp, incomingRequest, GetCallerIp);
      SetHeaderParameter(request, CompanyHeaders.UserId, incomingRequest, GetUserId);
      SetHeaderParameter(request, CompanyHeaders.UserDomain, incomingRequest, GetUserDomain);

      return await base.SendAsync(request, cancellationToken);
    }
  }
}
