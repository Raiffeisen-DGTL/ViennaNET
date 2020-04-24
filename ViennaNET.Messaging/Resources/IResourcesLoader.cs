using System.IO;

namespace ViennaNET.Messaging.Resources
{
  /// <summary>
  /// Интерфейс для провайдеров ресурсов
  /// </summary>
  public interface IResourcesLoader
  {
    /// <summary>
    /// Получает в виде потока ресурс по пути
    /// </summary>
    /// <typeparam name="T">Тип для поиска ресурса</typeparam>
    /// <param name="path">Путь до ресурса</param>
    /// <returns>Поток с ресурсом</returns>
    Stream GetResourceStream<T>(string path);
  }
}
