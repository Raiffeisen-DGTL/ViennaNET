using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ViennaNET.CallContext;
using ViennaNET.Security;
using ViennaNET.Utils;
using ViennaNET.WebApi.Net;

namespace ViennaNET.WebApi.Configurators.Security.Ntlm;

/// <summary>
///     Фабрика по получению авторизационных данных пользователя из JWT
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Тип будет удалён в последующем рефакторинге.")]
public class NtlmSecurityContextFactory : ISecurityContextFactory
{
    private static readonly char[] loginSeparators = { '\\' };
    private readonly ICallContextFactory _callContextFactory;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILoopbackIpFilter _loopbackIpFilter;

    public NtlmSecurityContextFactory(IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory,
        ILoopbackIpFilter loopbackIpFilter,
        ICallContextFactory callContextFactory,
        ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _httpContextAccessor = httpContextAccessor.ThrowIfNull(nameof(httpContextAccessor));
        _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
        _loopbackIpFilter = loopbackIpFilter.ThrowIfNull(nameof(loopbackIpFilter));
        _callContextFactory = callContextFactory.ThrowIfNull(nameof(callContextFactory));
    }

    /// <summary>
    ///     Создает контекст и заполняет его из WindowsIdentity
    /// </summary>
    /// <returns>Контекст</returns>
    public ISecurityContext Create()
    {
        var logger = _loggerFactory.CreateLogger<NtlmSecurityContext>();
        var callContext = _callContextFactory.Create();
        var httpContext = _httpContextAccessor.HttpContext;
        if (!(httpContext?.User?.Identity is WindowsIdentity identity) || !identity.IsAuthenticated)
        {
            identity = WindowsIdentity.GetCurrent();
        }

        var name = !string.IsNullOrWhiteSpace(callContext.UserId)
            ? callContext.UserId
            : identity.Name;

        var ip = _loopbackIpFilter.FilterIp(callContext.RequestCallerIp);

        return new NtlmSecurityContext(CleanUserName(name), ip, _httpClientFactory, logger);
    }

    /// <summary>
    ///     Создает контекст и заполняет его из JWT-токена
    /// </summary>
    /// <returns>Контекст</returns>
    public Task<ISecurityContext> CreateAsync()
    {
        return Task.FromResult(Create());
    }

    private string CleanUserName(string name)
    {
        var names = name.Split(loginSeparators, StringSplitOptions.RemoveEmptyEntries);
        return names.Length > 1
            ? names.Last()
            : name;
    }
}