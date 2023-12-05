using SimpleInjector;
using SimpleInjector.Packaging;

namespace ViennaNET.SimpleInjector.Extensions
{
  /// <summary>
  ///   Методы расширения для работы с SimpleInjector
  /// </summary>
  public static class SimpleInjectorExtensions
  {
    /// <summary>
    ///   Регистрирует пакет SimpleInjector в ручном режиме
    /// </summary>
    public static Container AddPackage(this Container container, IPackage package)
    {
      package.RegisterServices(container);

      return container;
    }
  }
}