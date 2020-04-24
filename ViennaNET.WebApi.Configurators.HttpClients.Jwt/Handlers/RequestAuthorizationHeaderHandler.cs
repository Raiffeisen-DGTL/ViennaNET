using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Utils;
using Microsoft.Net.Http.Headers;
using ViennaNET.CallContext;

namespace ViennaNET.WebApi.Configurators.HttpClients.Jwt.Handlers
{
  /// <summary>
  /// Обработчик Http-запросов, пробрасывающий заголовок Authorization из входящего запроса в исходящий
  /// </summary>
  public class RequestAuthorizationHeaderHandler : DelegatingHandler
  {
    private readonly ICallContextFactory _callContextFactory;

    public RequestAuthorizationHeaderHandler(ICallContextFactory callContextFactory)
    {
      _callContextFactory = callContextFactory.ThrowIfNull(nameof(callContextFactory));
    }

    private static void SetHeaderParameter(
      HttpRequestMessage request, string headerId, ICallContext callContext, Func<ICallContext, string> getParameter)
    {
      if (request.Headers.Contains(headerId))
      {
        request.Headers.Remove(headerId);
      }

      var parameter = getParameter(callContext);
      if (!string.IsNullOrEmpty(parameter))
      {
        request.Headers.Add(headerId, parameter);
      }
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var context = _callContextFactory.Create();

      SetHeaderParameter(request, HeaderNames.Authorization, context, x => x.AuthorizeInfo);

      return await base.SendAsync(request, cancellationToken);
    }
  }
}
