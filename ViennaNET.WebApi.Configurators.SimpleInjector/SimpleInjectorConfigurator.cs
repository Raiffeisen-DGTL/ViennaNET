using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Configurators.SimpleInjector.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace ViennaNET.WebApi.Configurators.SimpleInjector
{
  /// <summary>
  /// Внедряет SimpleInjector в качестве стороннего контейнера
  /// </summary>
  public static class SimpleInjectorConfigurator
  {
    /// <summary>
    /// Внедряет SimpleInjector в качестве стороннего контейнера
    /// </summary>
    public static ICompanyHostBuilder UseSimpleInjector(this ICompanyHostBuilder companyHostBuilder)
    {
      return companyHostBuilder.CreateContainer(CreateContainer)
                        .VerifyContainer(VerifyContainer)
                        .ConfigureContainer(ConfigureContainer)
                        .InitializeContainer(Initialize);
    }

    /// <summary>
    /// Создает экземпляр контейнера
    /// </summary>
    /// <returns>Контейнер</returns>
    internal static object CreateContainer()
    {
      return new Container();
    }

    /// <summary>
    /// Проверяет консистентность контейнера
    /// </summary>
    /// <returns>Контейнер</returns>
    internal static void VerifyContainer(object container)
    {
      var typedContainer = (Container)container;
      typedContainer.Verify();
    }

    /// <summary>
    /// Регистрация дополнительных сервисов в DI
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="container"></param>
    internal static void ConfigureContainer(IServiceCollection services, IConfiguration configuration, object container)
    {
      var typedContainer = (Container)container;
      typedContainer.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      services.AddSimpleInjector(typedContainer, options =>
      {
        options.AddAspNetCore()
               .AddControllerActivation();
      });

      typedContainer.Register(() => configuration, Lifestyle.Singleton);

      services.UseSimpleInjectorAspNetRequestScoping(typedContainer);
    }

    /// <summary>
    /// Настройка интеграции со встроенным DI и поиск и подключение пакетов (инсталлеров)
    /// </summary>
    /// <param name="app"></param>
    /// <param name="container"></param>
    /// <param name="configuration"></param>
    internal static void Initialize(IApplicationBuilder app, object container, IConfiguration configuration)
    {
      var typedContainer = container as Container;

      app.UseSimpleInjector(typedContainer);

      var config = configuration.GetSection(SimpleInjectorConfiguration.SectionName)
                                .Get<SimpleInjectorConfiguration>();

      if (config?.LoadPackagesDynamically == true)
      {
        LoadPackagesFromAssemblies(typedContainer);
        return;
      }

      LoadPackagesOnlyFromMainAssembly(typedContainer);
    }

    private static List<Assembly> GetLoadableAssemblies()
    {
      var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly()
                                               .Location);
      if (path == null)
      {
        throw new InvalidOperationException("Cannot resolve the path to the executing service");
      }

      var result = new List<Assembly>();

      var files = new DirectoryInfo(path).GetFiles("*.*", SearchOption.TopDirectoryOnly)
                                         .Where(file => file.Extension.ToLower() == ".dll" && !file.Name.StartsWith("Microsoft")
                                                                                           && !file.Name.StartsWith("System"))
                                         .ToList();
      foreach (var file in files)
      {
        if (TryLoadAssemblyName(file.FullName, out var name))
        {
          result.Add(Assembly.Load(name));
        }
      }

      return result;
    }

    private static bool TryLoadAssemblyName(string path, out AssemblyName name)
    {
      try
      {
        name = AssemblyName.GetAssemblyName(path);
        return true;
      }
      catch
      {
        name = null;
        return false;
      }
    }

    private static void LoadPackagesFromAssemblies(Container container)
    {
      var assemblies = new HashSet<Assembly>(GetLoadableAssemblies());

      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        assemblies.Add(assembly);
      }

      container.RegisterPackages(assemblies);
    }

    private static void LoadPackagesOnlyFromMainAssembly(Container container)
    {
      container.RegisterPackages(new[] { Assembly.GetEntryAssembly() });
    }
  }
}
