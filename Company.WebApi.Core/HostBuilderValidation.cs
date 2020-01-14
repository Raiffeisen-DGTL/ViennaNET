using Company.WebApi.Core.Validation;
using System.IO;

namespace Company.WebApi.Core
{
  public sealed partial class HostBuilder
  {
    private void VerifyBuilderState()
    {
      var configFilePath = Path.GetDirectoryName(_serviceAssembly.Location) + "/conf/appsettings.json";
      if (!File.Exists(configFilePath))
      {
        throw new HostBuilderValidationException("Файл конфигурации appsettings.json не найден");
      }

      if (_useServerAction is null)
      {
        throw new HostBuilderValidationException("Не указан сервер для хостинга");
      }
    }
  }
}
