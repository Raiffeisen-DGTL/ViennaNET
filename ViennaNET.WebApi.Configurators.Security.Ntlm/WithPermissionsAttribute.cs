using System;
using System.Linq;
using System.Threading.Tasks;
using ViennaNET.Logging;
using ViennaNET.Security;
using ViennaNET.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ViennaNET.WebApi.Configurators.Security.Ntlm
{
  /// <summary>
  /// Атрибут авторизации для контроллеров и действий, проверяющий полномочия пользователя
  /// </summary>
  public class WithPermissionsAttribute : TypeFilterAttribute
  {
    /// <summary>
    /// Конструктор
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

      public WithPermissionsImpl(ISecurityContextFactory securityContextFactory, string[] permissions)
      {
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
          Logger.LogErrorFormat(ex, "Error while check user permissions");
          return;
        }

        await next();
      }
    }
  }
}
