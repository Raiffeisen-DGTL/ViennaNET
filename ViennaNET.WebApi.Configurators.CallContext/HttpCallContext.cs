using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using ViennaNET.CallContext;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Configurators.CallContext
{
  /// <summary>
  /// Контекст вызова формируемый из Http-запроса
  /// </summary>
  public class HttpCallContext : ICallContext
  {
    private static readonly char[] loginSeparators = { '\\' };

    public string RequestId { get; private set; }
    public string UserId { get; private set; }
    public string UserDomain { get; private set; }
    public string RequestCallerIp { get; private set; }
    public string AuthorizeInfo { get; private set; }

    protected HttpCallContext() { }

    public static HttpCallContext Create(HttpContext httpContext)
    {
      var requestId = GetRequestId(httpContext);
      var userName = GetUserName(httpContext);
      var userDomain = GetUserDomain(httpContext, userName);
      var ip = GetIp(httpContext);
      var authInfo = GetAuthInfo(httpContext);

      return new HttpCallContext()
      {
        RequestId = requestId,
        UserId = CleanUserName(userName),
        UserDomain = userDomain,
        RequestCallerIp = ip,
        AuthorizeInfo = authInfo
      };
    }

    private static string GetRequestId(HttpContext context)
    {
      return context.Request.Headers.Keys.Contains(CompanyHttpHeaders.RequestId)
        ? context.Request.Headers[CompanyHttpHeaders.RequestId]
                 .ToString()
        : Guid.NewGuid()
              .ToString("N");
    }

    private static string GetAuthInfo(HttpContext context)
    {
      return context.Request.Headers.Keys.Contains(HeaderNames.Authorization)
        ? context.Request.Headers[HeaderNames.Authorization]
                 .ToString()
        : string.Empty;
    }

    private static string GetIp(HttpContext context)
    {
      return (context.Request.Headers.Keys.Contains(CompanyHttpHeaders.RequestHeaderCallerIp)
        ? context.Request.Headers[CompanyHttpHeaders.RequestHeaderCallerIp]
                 .ToString()
        : context.Connection?.RemoteIpAddress?.MapToIPv4()
                 .ToString()) ?? string.Empty;
    }

    private static string GetUserName(HttpContext context)
    {
      var username = string.Empty;
      if (context.User?.Identity != null)
      {
        username = context.User.Identity.Name;

        username ??= context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                            ?.Value;
      }

      if (context.Request.Headers.Keys.Contains(CompanyHttpHeaders.UserId))
      {
        username = context.Request.Headers[CompanyHttpHeaders.UserId]
                          .ToString();
      }

      return username;
    }

    private static string GetUserDomain(HttpContext context, string fullUserName)
    {
      var userDomain = ExtractUserDomain(fullUserName);

      if (context.Request.Headers.Keys.Contains(CompanyHttpHeaders.UserDomain))
      {
        userDomain = context.Request.Headers[CompanyHttpHeaders.UserDomain]
                            .ToString();
      }

      return userDomain;
    }

    private static string CleanUserName(string name)
    {
      if (name is null)
      {
        return string.Empty;
      }

      var names = name.Split(loginSeparators, StringSplitOptions.RemoveEmptyEntries);
      return names.Length > 1
        ? names.Last()
        : name;
    }

    private static string ExtractUserDomain(string name)
    {
      if (name is null)
      {
        return string.Empty;
      }

      var names = name.Split(loginSeparators, StringSplitOptions.RemoveEmptyEntries);
      return names.Length > 1
        ? names.First()
        : string.Empty;
    }
  }
}
