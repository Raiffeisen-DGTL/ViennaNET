using System;
using System.Threading.Tasks;
using ViennaNET.WebApi.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ViennaNET.WebApi.Configurators.CallContext.Middleware
{
  /// <summary>
  /// Формирует контекст вызова из контекста Http-запроса
  /// </summary>
  public class CallContextMiddleware : IMiddleware
  {
    private readonly IHttpCallContextAccessor _httpCallContextAccessor;

    public CallContextMiddleware(IHttpCallContextAccessor httpCallContextAccessor)
    {
      _httpCallContextAccessor = httpCallContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      if (!context.Request.Headers.ContainsKey(CompanyHttpHeaders.RequestId))
      {
        context.Request.Headers.Add(CompanyHttpHeaders.RequestId, Guid.NewGuid()
                                                                      .ToString("N"));
      }

      try
      {
        _httpCallContextAccessor.SetContext(HttpCallContext.Create(context));

        await next(context);
      }
      finally
      {
        _httpCallContextAccessor.CleanContext();
      }
    }
  }
}
