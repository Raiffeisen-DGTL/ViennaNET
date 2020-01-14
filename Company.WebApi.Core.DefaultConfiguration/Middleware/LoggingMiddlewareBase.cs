using System;
using System.Threading.Tasks;
using Company.Logging;
using Company.WebApi.Core.DefaultConfiguration.HttpClients;
using Microsoft.AspNetCore.Http;

namespace Company.WebApi.Core.DefaultConfiguration.Middleware
{
  public abstract class LoggingMiddlewareBase : IMiddleware
  {
    private static string GetRequestId(HttpContext context)
    {
      if (context.Request.Headers.ContainsKey(CompanyHeaders.RequestId))
      {
        return context.Request.Headers[CompanyHeaders.RequestId];
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

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      InitData(context);
      try
      {
        await next(context);
      }
      finally
      {
        FreeData();
      }
    }
  }
}
