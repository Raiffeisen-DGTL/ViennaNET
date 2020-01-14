using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Company.WebApi.Core.DefaultConfiguration.HttpClients;

namespace Company.WebApi.Core.DefaultConfiguration.Middleware
{
  /// <summary>
  /// Добавляет заголовок X-Request-Id к входящему запросу, если он отстутствует. Вызывается в первую очередь
  /// </summary>
  public class RequestRegistrationMiddleware : IMiddleware
  {
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      if (!context.Request.Headers.ContainsKey(CompanyHeaders.RequestId))
      {
        context.Request.Headers.Add(CompanyHeaders.RequestId, Guid.NewGuid().ToString("N"));
      }

      await next(context);
    }
  }
}
