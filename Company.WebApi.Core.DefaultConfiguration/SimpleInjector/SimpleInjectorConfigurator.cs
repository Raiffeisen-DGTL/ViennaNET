using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Company.Diagnostic.Core;
using Company.Logging;
using Company.WebApi.Core.DefaultConfiguration.Diagnostic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace Company.WebApi.Core.DefaultConfiguration.SimpleInjector
{
  /// <summary>
  /// Внедряет SimpleInjector в качестве стороннего контейнера
  /// </summary>
  public static class SimpleInjectorConfigurator
  {
    /// <summary>
    /// Создает экземпляр контейнера
    /// </summary>
    /// <returns>Контейнер</returns>
    public static object CreateContainer()
    {
      return new Container();
    }

    /// <summary>
    /// Проверяет консистентность контейнера
    /// </summary>
    /// <returns>Контейнер</returns>
    public static void VerifyContainer(object container)
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
    public static void Configure(IServiceCollection services, IConfiguration configuration, object container)
    {
      var typedContainer = (Container)container;
      typedContainer.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(typedContainer));

      typedContainer.Register(() => configuration, Lifestyle.Singleton);

      typedContainer.Collection.Register<IDiagnosticImplementor>(typeof(SimpleInjectorConfigurator).Assembly);
      typedContainer.Register<IHealthCheckingService, HealthCheckingService>(Lifestyle.Singleton);

      services.EnableSimpleInjectorCrossWiring(typedContainer);
      services.UseSimpleInjectorAspNetRequestScoping(typedContainer);
    }

    /// <summary>
    /// Настройка интеграции со встроенным DI и поиск и подключение пакетов (инсталлеров)
    /// </summary>
    /// <param name="app"></param>
    /// <param name="container"></param>
    public static void Initialize(IApplicationBuilder app, object container)
    {
      var typedContainer = container as Container;

      // Add application presentation components:
      typedContainer.RegisterMvcControllers(app);

      var assemblies = new HashSet<Assembly>(GetLoadableAssemblies());

      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        assemblies.Add(assembly);
      }

      // NOTE: Add application services:
      typedContainer.RegisterPackages(assemblies);

      // Allow Simple Injector to resolve services from ASP.NET Core.
      typedContainer.AutoCrossWireAspNetComponents(app);
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
  }
}
