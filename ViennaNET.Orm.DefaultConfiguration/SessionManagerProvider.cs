using ViennaNET.Orm.DI;
using ViennaNET.Utils;
using SimpleInjector;

namespace ViennaNET.Orm.DefaultConfiguration
{
  internal class SessionManagerProvider : ISessionManagerProvider
  {
    private readonly Container _container;

    public SessionManagerProvider(Container container)
    {
      _container = container.ThrowIfNull(nameof(container));
    }

    public ISessionManager GetSessionManager()
    {
      return _container.GetInstance<ISessionManager>();
    }
  }
}
