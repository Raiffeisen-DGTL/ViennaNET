using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Company.WebApi.Core.DefaultConfiguration.Middleware
{
  /// <summary>
  ///   Логирует входящий запрос и исходящий ответ сервиса
  /// </summary>
  internal class LogRequestAndResponseMiddleware : IMiddleware
  {
    private static readonly string[] loggingContentTypes = { "application/json", "text/plain" };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      var isRequestDiagnostic = context.Request.Path.Value.Contains("/diagnostic/");

      // log request body
      var requestBody = "";
      if (context.Request.ContentType != null && loggingContentTypes.Any(x => context.Request.ContentType.Contains(x))
                                              && context.Request.ContentLength > 0)
      {
        requestBody = await ExtractBody(context.Request);
      }
      else
      {
        requestBody = context.Request.ContentType;
      }


      if (isRequestDiagnostic)
      {
        Logger.LogDiagnostic($"Request:\nHTTP {context.Request.Method} {context.Request.Path}\n{requestBody}");
      }
      else
      {
        Logger.LogDebug($"Request:\nHTTP {context.Request.Method} {context.Request.Path}\n{requestBody}");
      }

      var originalBody = context.Response.Body;
      var responseBodyForLog = "";
      try
      {
        using (var memStream = new MemoryStream())
        {
          context.Response.Body = memStream;

          // run next handling
          await next(context);

          if (context.Response.ContentType != null && loggingContentTypes.Any(x => context.Response.ContentType.Contains(x))
                                                   && !context.Request.Path.Value.Contains("swagger"))
          {
            memStream.Position = 0;
            responseBodyForLog = new StreamReader(memStream).ReadToEnd();
          }
          else
          {
            responseBodyForLog = context.Response.ContentType;
          }

          memStream.Position = 0;
          await memStream.CopyToAsync(originalBody);
        }
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, "Unhandled service error");
        throw;
      }
      finally
      {
        context.Response.Body = originalBody;
      }

      // log response body
      if (isRequestDiagnostic)
      {
        Logger.LogDiagnostic($"Response:\nHTTP {context.Response.StatusCode}\n{responseBodyForLog}");
      }
      else
      {
        Logger.LogDebug($"Response:\nHTTP {context.Response.StatusCode}\n{responseBodyForLog}");
      }
    }

    private async Task<string> ExtractBody(HttpRequest request)
    {
      request.EnableRewind();
      request.EnableBuffering();

      var buffer = new byte[Convert.ToInt32(request.ContentLength)];
      await request.Body.ReadAsync(buffer, 0, buffer.Length);
      var bodyAsText = Encoding.UTF8.GetString(buffer);
      request.Body.Seek(0, SeekOrigin.Begin);

      return bodyAsText;
    }
  }
}
