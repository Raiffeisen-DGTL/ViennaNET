using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViennaNET.Logging;
using Microsoft.AspNetCore.Http;

namespace ViennaNET.WebApi.Configurators.Common.Middleware
{
  /// <summary>
  ///   Логирует входящий запрос и исходящий ответ сервиса
  /// </summary>
  public class LogRequestAndResponseMiddleware
  {
    private static readonly string[] loggingContentTypes = { "application/json", "text/plain" };

    private readonly RequestDelegate _next;

    public LogRequestAndResponseMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context)
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
        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        // run next handling
        await _next(context);

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
      var body = await request.BodyReader.ReadAsync();
      var bodyAsText = Encoding.UTF8.GetString(body.Buffer.ToArray());

      return bodyAsText;
    }
  }
}
