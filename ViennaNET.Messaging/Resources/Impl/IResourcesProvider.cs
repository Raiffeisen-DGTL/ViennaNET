using System.Collections.Generic;
using System.IO;

namespace ViennaNET.Messaging.Resources.Impl
{
  /// <summary>
  /// Интерфейс поставщика путей ресурсов для сериализуемых сообщений
  /// </summary>
  public interface IResourcesProvider
  {
    /// <summary>
    /// Получает строку пути для ресурса
    /// </summary>
    /// <typeparam name="T">Тип сообщения</typeparam>
    /// <returns>Строка пути</returns>
    IEnumerable<Stream> GetResourceStream<T>();
  }
}