using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ViennaNET.Security;
using ViennaNET.Utils;

namespace ViennaNET.WebApi.Configurators.Security.Ntlm
{
    /// <summary>
    ///   Атрибут авторизации для контроллеров и действий, проверяющий полномочия пользователя
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Тип будет удалён в последующем рефакторинге.")]
    public class WithPermissionsAttribute : TypeFilterAttribute
    {
        /// <summary>
        ///   Конструктор
        /// </summary>
        /// <param name="permissions">Набор обязательных полномочий</param>
        public WithPermissionsAttribute(params string[] permissions) : base(typeof(WithPermissionsImpl))
        {
            Arguments = new object[] { permissions };
        }

        private class WithPermissionsImpl : IAsyncActionFilter
        {
            private readonly string[] _permissions;
            private readonly ISecurityContextFactory _securityContextFactory;
            private readonly ILogger _logger;

            public WithPermissionsImpl(ILogger<WithPermissionsImpl> logger,
                ISecurityContextFactory securityContextFactory, string[] permissions)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _securityContextFactory = securityContextFactory.ThrowIfNull(nameof(securityContextFactory));
                _permissions = permissions.ThrowIfNull(nameof(permissions));

                if (!_permissions.Any())
                {
                    throw new ArgumentException(nameof(permissions));
                }
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var hasPermissions = await _securityContextFactory.Create()
                        .HasPermissionsAsync(_permissions);
                    if (!hasPermissions)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    context.Result = new ForbidResult();
                    _logger.LogError(ex, "Error while check user permissions");
                    return;
                }

                await next();
            }
        }
    }
}