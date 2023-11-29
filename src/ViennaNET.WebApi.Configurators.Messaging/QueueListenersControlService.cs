using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using ViennaNET.Messaging.Listening;

namespace ViennaNET.WebApi.Configurators.Messaging
{
  internal class QueueListenersControlService : IHostedService
  {
    private readonly Container _container;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public QueueListenersControlService(IHostApplicationLifetime hostApplicationLifetime, Container container)
    {
      _hostApplicationLifetime = hostApplicationLifetime;
      _container = container;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);

      Start();
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
      => Task.CompletedTask;

    private void OnStopping()
    {
      var queueListeners = _container.GetAllInstances<IQueueListener>();
      foreach (var listener in queueListeners)
      {
        listener.Stop();
      }
    }

    private void Start()
    {
      var queueListeners = _container.GetAllInstances<IQueueListener>();
      foreach (var listener in queueListeners)
      {
        listener.Start();
      }
    }
  }
}