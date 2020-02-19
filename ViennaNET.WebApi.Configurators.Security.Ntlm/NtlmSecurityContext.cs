using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ViennaNET.Logging;
using ViennaNET.Security;
using ViennaNET.Utils;

namespace ViennaNET.WebApi.Configurators.Security.Ntlm
{
  public class NtlmSecurityContext : ISecurityContext
  {
    private readonly AsyncLazy<List<string>> _permissions;

    public NtlmSecurityContext(string userName, string ip, IHttpClientFactory clientFactory)
    {
      clientFactory.ThrowIfNull(nameof(clientFactory));

      UserIp = ip;
      UserName = userName;

      _permissions = new AsyncLazy<List<string>>(async () => await ReadDataAsync(clientFactory));
    }

    public async Task<bool> IsInRolesAsync(params string[] permissions)
    {
      return (await _permissions).Intersect(permissions)
                                 .Any();
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync()
    {
      return await _permissions;
    }

    private async Task<List<string>> ReadDataAsync(IHttpClientFactory clientFactory)
    {
      try
      {
        var client = clientFactory.CreateClient("security")
                                  .ThrowIfNull("securityRestClient");

        var response = await client.GetAsync($"api/users/{UserName}/permissions");

        response.EnsureSuccessStatusCode();
        var authData = await response.Content.ReadAsAsync<SecurityPermissionsDto>();
        return authData.Permissions.ToList();
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, $"Ошибка при запросе полномочий пользователя {UserName}");
        return new List<string>(0);
      }
    }

    public string UserName { get; }
    public string UserIp { get; }

    public async Task<bool> HasPermissionsAsync(params string[] permissions)
    {
      return (await _permissions).Intersect(permissions)
                                 .Any();
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync()
    {
      return await _permissions;
    }
  }
}
