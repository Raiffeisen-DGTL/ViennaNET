using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ViennaNET.Utils;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.CallContext;

namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Handlers
{
  /// <summary>
  /// Базовый обработчик Http-запросов, пробрасывающий заголовки CompanyHeaders из входящего запроса в исходящий
  /// </summary>
  public class BaseCompanyRequestHeadersHandler : DelegatingHandler
  {
    private readonly ICallContextFactory _callContextFactory;

    public BaseCompanyRequestHeadersHandler(ICallContextFactory callContextFactory)
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

      SetHeaderParameter(request, CompanyHttpHeaders.RequestId, context, x => x.RequestId);
      SetHeaderParameter(request, CompanyHttpHeaders.RequestHeaderCallerIp, context, x => x.RequestCallerIp);
      SetHeaderParameter(request, CompanyHttpHeaders.UserId, context, x => x.UserId);
      SetHeaderParameter(request, CompanyHttpHeaders.UserDomain, context, x => x.UserDomain);

      return await base.SendAsync(request, cancellationToken);
    }
  }
}
