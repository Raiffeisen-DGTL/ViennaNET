using Antlr.Runtime.Misc;
using Moq;
using ViennaNET.Orm.Application;
using ViennaNET.Orm.Factories;
using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm.Tests.Unit.DSL
{
  internal static class Given
  {
    public static ApplicationContextBuilder ApplicationContext => new ApplicationContextBuilder();

    public static ApplicationContextProvider AppliationContextProvider(Func<ApplicationContextBuilder, IApplicationContext> builder = null)
    {
      var context = builder != null ? builder(ApplicationContext) : ApplicationContext.Please();

      var sessionFactoryProvidersManager = new Mock<ISessionFactoryProvidersManager>();
      sessionFactoryProvidersManager
        .Setup(x => x.GetSessionFactoryProvider(It.IsAny<string>()))
        .Returns(Mock.Of<ISessionFactoryProvider>());

      return new ApplicationContextProvider(new [] { (IBoundedContext)context }, sessionFactoryProvidersManager.Object);
    }
  }
}
