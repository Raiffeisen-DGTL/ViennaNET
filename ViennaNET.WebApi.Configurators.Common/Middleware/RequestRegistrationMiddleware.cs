using System;
using System.Threading.Tasks;
using ViennaNET.WebApi.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ViennaNET.WebApi.Configurators.Common.Middleware
{
  /// <summary>
  /// Добавляет заголовок X-Request-Id к входящему запросу, если он отстутствует. Вызывается в первую очередь
  /// </summary>
  public class RequestRegistrationMiddleware
  {
    private readonly RequestDelegate _next;

    public RequestRegistrationMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
      if (!context.Request.Headers.ContainsKey(CompanyHttpHeaders.RequestId))
      {
        context.Request.Headers.Add(CompanyHttpHeaders.RequestId, Guid.NewGuid()
                                                                      .ToString("N"));
      }

      await _next(context);
    }
  }
}
