using System;
using Company.Logging;
using SimpleInjector;

namespace Company.WebApi.Core.DefaultConfiguration.Extensions
{
  public static class HostBuilderExtensions
  {
    public static HostBuilder AddOnStartAction<T>(this HostBuilder hostBuilder, Action<T> action) where T : class
    {
      hostBuilder.AddOnStartAction(container => GetAction(container, action));
      return hostBuilder;
    }

    public static HostBuilder AddOnStopAction<T>(this HostBuilder hostBuilder, Action<T> action) where T : class
    {
      hostBuilder.AddOnStopAction(container => GetAction(container, action));
      return hostBuilder;
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
