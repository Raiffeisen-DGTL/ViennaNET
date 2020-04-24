using ViennaNET.Diagnostic;
using ViennaNET.Orm.DI;
using ViennaNET.Orm.Diagnostic;
using ViennaNET.Orm.Factories;
using ViennaNET.Orm.Repositories;
using ViennaNET.Orm.Seedwork;
using NHibernate;
using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.DefaultConfiguration
{
  /// <inheritdoc />
  /// <summary>
  /// Пакет SimpleInjector для работы с ORM на основе NHibernate
  /// </summary>
  public class OrmPackage : IPackage
  {
    /// <inheritdoc />
    public void RegisterServices(Container container)
    {
      var registration = Lifestyle.Singleton.CreateRegistration<EntityFactoryService>(container);

      container.AddRegistration<IEntityFactoryService>(registration);
      container.AddRegistration<IEntityRepositoryFactory>(registration);

      container.Register<ISessionFactoryManager, SessionFactoryManager>(Lifestyle.Singleton);
      container.Register<IInterceptor, DomainEventsInterceptor>(Lifestyle.Singleton);
      container.Register<ISessionManagerProvider, SessionManagerProvider>(Lifestyle.Singleton);
      container.Register<ISessionFactoryProvidersManager, SessionFactoryProvidersManager>(Lifestyle.Singleton);
      container.Register<ISessionManager, ScopedSessionManager>(Lifestyle.Scoped);
      container.Register<ICallContextProvider, DefaultCallContextProvider>(Lifestyle.Singleton);
      container.Register<IApplicationContextProvider, ApplicationContextProvider>(Lifestyle.Singleton);

      container.Collection.Register<IBoundedContext>(GetType().Assembly);
      container.Collection.Register<ISessionFactoryProviderGetter>(GetType().Assembly);

      container.Collection.Append<IDiagnosticImplementor, OrmConnectionsChecker>(Lifestyle.Singleton);
    }
  }
}
