using ViennaNET.Orm.DI;
using ViennaNET.Security;
using ViennaNET.Utils;

namespace ViennaNET.Orm.DefaultConfiguration
{
  internal class DefaultCallContextProvider : ICallContextProvider
  {
    private readonly ISecurityContextFactory _securityContextFactory;

    public DefaultCallContextProvider(ISecurityContextFactory securityContextFactory)
    {
      _securityContextFactory = securityContextFactory.ThrowIfNull(nameof(securityContextFactory));
    }

    public string GetUserName()
    {
      return _securityContextFactory.Create()
                                    .UserName;
    }

    public string GetUserIp()
    {
      return _securityContextFactory.Create()
                                    .UserIp;
    }
  }
}
