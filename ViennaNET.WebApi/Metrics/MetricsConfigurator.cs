using App.Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.AspNetCore.Tracking;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using System.Linq;

namespace ViennaNET.WebApi.Metrics
{
  /// <summary>
  /// Конфигуратор для подключения сборщика метрик
  /// </summary>
  internal static class MetricsConfigurator
  {
    /// <summary>
    /// Конфигурирует подключение метрик
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IWebHostBuilder ConfigureCompanyMetrics(this IWebHostBuilder builder)
    {
      builder.ConfigureServices((context, services) =>
      {
        ConfigureInternal(services, context.Configuration);
      });

      return builder;
    }

    private static void ConfigureInternal(IServiceCollection services, IConfiguration configuration)
    {
      var config = configuration.GetSection("metrics")
                                ?.Get<MetricsConfiguration>();

      if (config is null || !config.Enabled)
      {
        return;
      }

      switch (config.Reporter)
      {
        case MetricsReportersTypes.Prometheus:
          ConfigurePrometheusMetrics(services, configuration);
          break;
        default:
          ConfigureDefaultMetrics(services, configuration);
          break;
      }
    }

    /// <summary>
    /// Переопределяет форматирование метрик роута "/metrics-text" для использования в Prometheus
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    private static void ConfigurePrometheusMetrics(IServiceCollection services, IConfiguration configuration)
    {
      var metrics = AppMetrics.CreateDefaultBuilder()
                              .OutputMetrics.AsPrometheusPlainText()
                              .Build();
      services.AddMetrics(metrics);

      var options = new MetricsWebHostOptions();
      options.EndpointOptions = endpointsOptions =>
      {
        endpointsOptions.MetricsTextEndpointOutputFormatter = metrics
                                                              .OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>()
                                                              .First();
      };

      services.AddMetricsReportingHostedService(options.UnobservedTaskExceptionHandler);
      services.AddMetricsEndpoints(options.EndpointOptions, configuration);
      services.AddMetricsTrackingMiddleware(options.TrackingMiddlewareOptions, configuration);

      services.AddSingleton<IStartupFilter, DefaultMetricsEndpointsStartupFilter>();
      services.AddMetricsTrackingMiddleware(configuration);
      services.AddSingleton<IStartupFilter, DefaultMetricsTrackingStartupFilter>();
    }

    /// <summary>
    /// Конфигурирование базовых метрик
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    private static void ConfigureDefaultMetrics(IServiceCollection services, IConfiguration configuration)
    {
      var metrics = AppMetrics.CreateDefaultBuilder()
                              .Build();
      services.AddMetrics(metrics);

      services.AddMetricsReportingHostedService();
      services.AddMetricsEndpoints(configuration);
      services.AddMetricsTrackingMiddleware(configuration);

      services.AddSingleton<IStartupFilter, DefaultMetricsEndpointsStartupFilter>();
      services.AddMetricsTrackingMiddleware(configuration);
      services.AddSingleton<IStartupFilter, DefaultMetricsTrackingStartupFilter>();
    }
  }
}
