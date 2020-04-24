using System.Collections.Generic;

namespace ViennaNET.Orm.Factories
{
  /// <summary>
  /// Менеджер провайдеров фабрик сессий. Инициализирует и
  /// кеширует провайдеры фабрик сессий. 
  /// </summary>
  public interface ISessionFactoryProvidersManager
  {
    /// <summary>
    /// Получает провайдер фабрик сессий по имени
    /// </summary>
    /// <param name="nick">Имя подключения провайдера</param>
    /// <returns>Провайдер фабрик сессий</returns>
    ISessionFactoryProvider GetSessionFactoryProvider(string nick);

    /// <summary>
    /// Получает все провайдеры фабрик сессий по имени
    /// </summary>
    /// <returns>Провайдеры фабрик сессий</returns>
    IEnumerable<ISessionFactoryProvider> GetSessionFactoryProviders();
  }
}
