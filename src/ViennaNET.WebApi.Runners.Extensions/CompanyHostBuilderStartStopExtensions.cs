using System;
using SimpleInjector;
using ViennaNET.Logging;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Runners.Extensions
{
  /// <summary>
  /// Методы расширения для удобной работы со Start/Stop действиями через SimpleInjector
  /// </summary>
  public static class CompanyHostBuilderStartStopExtensions
  {
    /// <summary>
    /// Добавляет действие при старте сервиса
    /// </summary>
    /// <typeparam name="T">Класс, содержащий действие</typeparam>
    /// <param name="companyHostBuilder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IViennaHostBuilder AddOnStartAction<T>(this IViennaHostBuilder companyHostBuilder, Action<T> action) where T : class
    {
      companyHostBuilder.AddOnStartAction(container => GetAction(container, action));
      return companyHostBuilder;
    }

    /// <summary>
    /// Добавляет действие при остановке сервиса
    /// </summary>
    /// <typeparam name="T">Класс, содержащий действие</typeparam>
    /// <param name="companyHostBuilder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IViennaHostBuilder AddOnStopAction<T>(this IViennaHostBuilder companyHostBuilder, Action<T> action) where T : class
    {
      companyHostBuilder.AddOnStopAction(container => GetAction(container, action));
      return companyHostBuilder;
    }

    private static Action GetAction<T>(this object container, Action<T> action) where T : class
    {
      try
      {
        if (action != null)
        {
          return () => action(((Container)container).GetInstance<T>());
        }

        return () =>
        {
        };
      }
      catch (ActivationException exception)
      {
        Logger.LogError(exception, $"Could not to get instance of type: {typeof(T)}");
        return () =>
        {
        };
      }
    }
  }
}
