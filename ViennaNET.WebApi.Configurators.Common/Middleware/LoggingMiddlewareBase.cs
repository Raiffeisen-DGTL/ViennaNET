using System;
using System.Threading.Tasks;
using ViennaNET.Logging;
using ViennaNET.WebApi.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ViennaNET.WebApi.Configurators.Common.Middleware
{
  public abstract class LoggingMiddlewareBase
  {
    private readonly RequestDelegate _next;

    protected LoggingMiddlewareBase(RequestDelegate next)
    {
      _next = next;
    }

    private static string GetRequestId(HttpContext context)
    {
      if (context.Request.Headers.ContainsKey(CompanyHttpHeaders.RequestId))
      {
        return context.Request.Headers[CompanyHttpHeaders.RequestId];
      }

      return Guid.NewGuid().ToString("N");
    }

    protected abstract string GetIdentityName(HttpContext context);

    private void InitData(HttpContext context)
    {
      try
      {
        Logger.User = GetIdentityName(context);
        Logger.RequestId = GetRequestId(context);
      }
      catch { }
    }

    private void FreeData()
    {
      try
      {
        Logger.ClearUser();
        Logger.ClearRequestId();
      }
      catch { }
    }

    public async Task Invoke(HttpContext context)
    {
      InitData(context);
      try
      {
        await _next(context);
      }
      finally
      {
        FreeData();
      }
    }
  }
}
