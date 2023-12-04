using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ViennaNET.WebApi.ExceptionHandling
{
  /// <inheritdoc />
  /// <summary>
  ///   Атрибут, позволяющий обрабатывать исключения и возвращать необходимый HTTP-код.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
  public sealed class ExceptionHandlerAttribute : ExceptionFilterAttribute
  {
    private readonly HttpStatusCode _code;
    private readonly bool _includeExceptionInfo;
    private readonly Type _type;

    public ExceptionHandlerAttribute(Type type, HttpStatusCode code, bool includeExceptionInfo = true)
    {
      _type = type;
      _code = code;
      _includeExceptionInfo = includeExceptionInfo;
    }

    private object GetExceptionInfo(Exception exception)
    {
      if (_includeExceptionInfo)
      {
        return new ErrorResult
        {
          Message = exception.Message,
          ExceptionType = exception.GetType()
            .ToString(),
          StackTrace = exception.StackTrace
        };
      }

      return exception.Message;
    }

    public override void OnException(ExceptionContext actionExecutedContext)
    {
      if (actionExecutedContext.Exception.GetType() != _type)
      {
        return;
      }

      var result = new ObjectResult(GetExceptionInfo(actionExecutedContext.Exception)) { StatusCode = (int)_code };

      actionExecutedContext.Result = result;
    }

    public override Task OnExceptionAsync(ExceptionContext actionExecutedContext)
    {
      OnException(actionExecutedContext);

      return Task.FromResult<object>(null);
    }
  }
}