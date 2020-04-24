using System;
using ViennaNET.CallContext;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Context
{
  public class MessagingContext : ICallContext
  {
    /// <summary>
    /// Идентификатор запроса
    /// </summary>
    public string RequestId { get; private set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// Домен пользователя
    /// </summary>
    public string UserDomain { get; private set; }

    /// <summary>
    /// IP-адрес создателя первого запроса в цепочке
    /// </summary>
    public string RequestCallerIp { get; private set; }

    /// <summary>
    /// Авторизационные данные создателя первого запроса в цепочке
    /// </summary>
    public string AuthorizeInfo { get; private set; }

    protected MessagingContext() { }

    public static MessagingContext Create(BaseMessage message)
    {
      message.Properties.TryGetValue(CallContextHeaders.RequestId, out var requestId);
      message.Properties.TryGetValue(CallContextHeaders.UserId, out var userId);
      message.Properties.TryGetValue(CallContextHeaders.UserDomain, out var userDomain);
      message.Properties.TryGetValue(CallContextHeaders.RequestCallerIp, out var requestCallerIp);
      message.Properties.TryGetValue(CallContextHeaders.AuthorizeInfo, out var authorizeInfo);

      var reqId = string.IsNullOrWhiteSpace(requestId as string)
        ? Guid.NewGuid()
              .ToString("N")
        : (string)requestId;

      var usr = string.IsNullOrWhiteSpace(userId as string)
        ? Environment.UserName
        : (string)userId;

      return new MessagingContext()
      {
        RequestId = reqId,
        UserId = usr,
        UserDomain = (string)userDomain,
        RequestCallerIp = (string)requestCallerIp,
        AuthorizeInfo = (string)authorizeInfo
      };
    }
  }
}
