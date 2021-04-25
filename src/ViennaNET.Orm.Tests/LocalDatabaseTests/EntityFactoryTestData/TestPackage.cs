using SimpleInjector;
using SimpleInjector.Packaging;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm.Tests.LocalDatabaseTests.EntityFactoryTestData
{
  public class TestPackage : IPackage
  {
    public void RegisterServices(Container container)
    {
      container.Collection.Append<IBoundedContext, TestDataContext>(Lifestyle.Singleton);
    }
  }
}
