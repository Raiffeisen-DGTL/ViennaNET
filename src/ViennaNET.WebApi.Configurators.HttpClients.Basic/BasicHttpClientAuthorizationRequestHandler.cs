using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ViennaNET.WebApi.Configurators.HttpClients.Basic
{
  /// <summary>
  ///   Обработчик Http-запросов, добавляющий basic-авторизацию в запрос
  /// </summary>
  internal sealed class BasicHttpClientAuthorizationRequestHandler : DelegatingHandler
  {
    /// <summary>
    ///   Закодированные в base64 логин/пароль
    /// </summary>
    private readonly string _encodedBasicCredentials;

    public BasicHttpClientAuthorizationRequestHandler(string userName, string password)
    {
      var encodedCreds = Encoding.ASCII.GetBytes($"{userName}:{password}");
      _encodedBasicCredentials = Convert.ToBase64String(encodedCreds);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      request.Headers.Authorization =
        new AuthenticationHeaderValue("Basic", _encodedBasicCredentials);

      return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
  }
}