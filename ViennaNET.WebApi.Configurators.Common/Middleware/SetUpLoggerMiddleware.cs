using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ViennaNET.WebApi.Configurators.Common.Middleware
{
  /// <summary>
  /// Заполняет в логгере поля RequestId и User из входящего запроса
  /// </summary>
  public class SetUpLoggerMiddleware : LoggingMiddlewareBase
  {
    public SetUpLoggerMiddleware(RequestDelegate next) : base(next)
    {
    }

    private static readonly char[] LoginSeparators = { '\\' };

    protected override string GetIdentityName(HttpContext context)
    {
      var fullName = context.User.Identity.Name ?? Environment.UserName;
      var names = fullName?.Split(LoginSeparators, StringSplitOptions.RemoveEmptyEntries);
      return names.Length > 1 ? names.Last() : fullName;
    }
  }
}
