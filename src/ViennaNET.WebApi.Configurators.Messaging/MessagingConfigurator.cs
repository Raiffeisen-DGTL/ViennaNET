using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using ViennaNET.Messaging.Listening;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Configurators.Messaging
{
  public static class MessagingExtensions
  {
    /// <summary>
    /// Включает приемники сообщений, необходим для работы очередей.
    /// </summary>
    public static ICompanyHostBuilder EnableAllQueueReceivers(
      this ICompanyHostBuilder companyHostBuilder)
    {
      companyHostBuilder.RegisterServices((collection, container, _) => collection.AddHostedService(serviceProvider =>
      {
        var hostApplicationLifetime = serviceProvider.GetService<IHostApplicationLifetime>();
        return new QueueListenersControlService(hostApplicationLifetime, (Container)container);
      }));

      return companyHostBuilder;
    }
  }
}