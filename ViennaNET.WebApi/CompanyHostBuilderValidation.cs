using ViennaNET.WebApi.Validation;
using System.IO;

namespace ViennaNET.WebApi
{
  public sealed partial class CompanyHostBuilder
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
