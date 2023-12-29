#nullable enable

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ViennaNET.Utils;

namespace ViennaNET.WebApi.Configurators.Common.Middleware
{
  /// <summary>
  ///   Создаёт области Host, Thread и User для текущего запроса, в системе ведения журнала.
  /// </summary>
  [ExcludeFromCodeCoverage(Justification = "Тип будет удалён в последующем рефакторинге.")]
  public class SetLoggingScopeMiddleware
  {
    private readonly ILogger<SetLoggingScopeMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>
    ///   Создаёт и инициализирует новый экземпляр класса <see cref="SetLoggingScopeMiddleware" />,
    ///   с заданными значениями полей.
    /// </summary>
    /// <param name="next">Ссылка на делегат следующего промежуточное ПО.</param>
    /// <param name="logger">Ссылка на объект <see cref="ILogger{SetLoggingScopeMiddleware}" />.</param>
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
    ///   Выполняет шаг конвейера промежуточного ПО настраивающий систему ведения журналов.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
      var userName = TrimUserDomain(context.User.Identity?.Name ?? Environment.UserName);

      _logger.BeginScope($"Host: {Environment.MachineName}, " +
                         $"Thread: {Environment.CurrentManagedThreadId}, " +
                         $"User: {userName}");

      await _next(context).ConfigureAwait(false);
    }
  }
}