#nullable enable

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ViennaNET.Logging;
using ViennaNET.Utils;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Configurators.Common.Middleware
{
  /// <summary>
  /// Заполняет в логгере поля RequestId и User из входящего запроса
  /// </summary>
  public class SetLoggingScopeMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<SetLoggingScopeMiddleware> _logger;
    
    /// <summary>
    ///  Создаёт и инициализирует новый экземпляр класса <see cref="SetLoggingScopeMiddleware"/>,
    ///  с заданными значениями полей.
    /// </summary>
    /// <param name="next">Ссылка на делегат следующего промежуточное ПО.</param>
    /// <param name="logger">Ссылка на объект <see cref="ILogger{SetLoggingScopeMiddleware}"/>.</param>
    public SetLoggingScopeMiddleware(RequestDelegate next, ILogger<SetLoggingScopeMiddleware> logger)
    {
      _next = next;
      _logger = logger.ThrowIfNull(nameof(logger));
    }

    private string TrimUserDomain(string userName)
    {
      if (userName.Contains('\\'))
      {
        userName = userName.Remove(0, userName.LastIndexOf('\\') + 1);
      }

      return userName;
    }

    /// <summary>
    /// Выполняет шаг конвейера промежуточного ПО настраивающий систему ведения журналов.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
      var userName = TrimUserDomain(context.User.Identity.Name ?? Environment.UserName);
      
      _logger.BeginScope($"Host: {Environment.MachineName}, " +
        $"Thread: {Environment.CurrentManagedThreadId}, " +
        $"User: {userName}");

      Logger.User = userName;
      Logger.RequestId = context.Request.Headers.ContainsKey(CompanyHttpHeaders.RequestId) 
        ? context.Request.Headers[CompanyHttpHeaders.RequestId].ToString()
        : Activity.Current.Id;

      await _next(context).ConfigureAwait(false);
    }
  }
}