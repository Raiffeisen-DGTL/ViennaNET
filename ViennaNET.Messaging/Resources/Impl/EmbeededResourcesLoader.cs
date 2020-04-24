using System.IO;

namespace ViennaNET.Messaging.Resources.Impl
{
  /// <summary>
  /// Получает внедренный ресурс
  /// </summary>
  public class EmbeddedResourcesLoader : IResourcesLoader
  {
    /// <inheritdoc />
    public Stream GetResourceStream<T>(string path)
    {
      return typeof(T).Assembly.GetManifestResourceStream(path);
    }
  }
}
