using System;
using SimpleInjector;
using ViennaNET.Logging;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Runners.Extensions
{
  /// <summary>
  ///   Методы расширения для удобной работы со Start/Stop действиями через SimpleInjector
  /// </summary>
  public static class CompanyHostBuilderStartStopExtensions
  {
    /// <summary>
    ///   Добавляет действие при старте сервиса
    /// </summary>
    /// <typeparam name="T">Класс, содержащий действие</typeparam>
    /// <param name="companyHostBuilder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ICompanyHostBuilder AddOnStartAction<T>(this ICompanyHostBuilder companyHostBuilder, Action<T> action)
      where T : class
    {
      companyHostBuilder.AddOnStartAction(container => GetAction(container, action));
      return companyHostBuilder;
    }

    /// <summary>
    ///   Добавляет действие перед остановкой сервиса
    /// </summary>
    /// <typeparam name="T">Класс, содержащий действие</typeparam>
    /// <param name="companyHostBuilder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ICompanyHostBuilder AddOnStoppingAction<T>(this ICompanyHostBuilder companyHostBuilder,
      Action<T> action)
      where T : class
    {
      companyHostBuilder.AddOnStoppingAction(container => GetAction(container, action));
      return companyHostBuilder;
    }

    /// <summary>
    ///   Добавляет действие при остановке сервиса
    /// </summary>
    /// <typeparam name="T">Класс, содержащий действие</typeparam>
    /// <param name="companyHostBuilder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ICompanyHostBuilder AddOnStoppedAction<T>(this ICompanyHostBuilder companyHostBuilder,
      Action<T> action)
      where T : class
    {
      companyHostBuilder.AddOnStoppedAction(container => GetAction(container, action));
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