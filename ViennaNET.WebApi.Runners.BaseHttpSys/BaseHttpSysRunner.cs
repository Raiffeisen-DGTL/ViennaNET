using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.Common;
using ViennaNET.WebApi.Configurators.Diagnostic;
using ViennaNET.WebApi.Configurators.HttpClients.Ntlm;
using ViennaNET.WebApi.Configurators.HttpSys;
using ViennaNET.WebApi.Configurators.Security.Ntlm;
using ViennaNET.WebApi.Configurators.SimpleInjector;
using ViennaNET.WebApi.Configurators.Swagger;

namespace ViennaNET.WebApi.Runners.BaseHttpSys
{
  /// <summary>
  /// Класс для создания и конфигурирования базового сервиса
  /// </summary>
  public static class BaseHttpSysRunner
  {
    public static ICompanyHostBuilder Configure()
    {
      return CompanyHostBuilder.Create()
                               .UseHttpSys()
                               .UseCommonModules()
                               .UseSimpleInjector()
                               .UseSwagger()
                               .UseDiagnosing()
                               .UseNtlmHttpClients()
                               .UseNtlmAuth();
    }
  }
}
