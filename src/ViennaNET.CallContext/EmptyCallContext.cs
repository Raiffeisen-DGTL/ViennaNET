using System;

namespace ViennaNET.CallContext
{
  public class EmptyCallContext : ICallContext
  {
    public string RequestId =>
      Guid.NewGuid()
          .ToString("N");

    public string UserId => Environment.UserName;

    public string UserDomain => Environment.UserDomainName;

    public string RequestCallerIp => string.Empty;

    public string AuthorizeInfo => string.Empty;
  }
}
