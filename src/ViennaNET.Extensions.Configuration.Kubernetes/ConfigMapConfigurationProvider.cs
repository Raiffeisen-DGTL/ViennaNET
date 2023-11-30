using System.Diagnostics.CodeAnalysis;
using k8s;
using k8s.Models;

namespace ViennaNET.Extensions.Configuration.Kubernetes;

internal sealed class ConfigMapConfigurationProvider : KubernetesConfigurationProvider
{
    private const string DefaultFileName = "appsettings.json";

    public ConfigMapConfigurationProvider(KubernetesConfigurationSource source, IKubernetesClientBuilder builder) :
        base(source, builder)
    {
    }

    public Watcher<V1ConfigMap>? Watcher { get; private set; }

    /// <inheritdoc />
    protected override void Watch()
    {
        var message = Client.CoreV1.ListNamespacedConfigMapWithHttpMessagesAsync(Source.Namespace,
            fieldSelector: $"metadata.name={Source.Name}", watch: true);

        Watcher = message.Watch<V1ConfigMap, V1ConfigMapList>(OnEvent, Source.OnWatchException, Source.OnWatchClose);
    }

    internal void OnEvent(WatchEventType type, V1ConfigMap map)
    {
        if (type is not (WatchEventType.Modified or WatchEventType.Added))
        {
            return;
        }

        Extract(map);
        OnReload();
    }

    /// <inheritdoc />
    protected override void Single()
    {
        Extract(Client.CoreV1.ReadNamespacedConfigMap(Source.Name, Source.Namespace));
    }

    private void Extract(V1ConfigMap map)
    {
        if (Source.DataType == DataTypes.Json)
        {
            var fileName = Source.FileName ?? DefaultFileName;

            if (map.Data.All(pair => pair.Key != fileName))
            {
                return;
            }

            Parse(map.Data.SingleOrDefault(pair => pair.Key == fileName).Value);
        }
        else
        {
            Parse(map.Data);
        }
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage(Justification = "Завершение делегируются. Нет логики для тестирования")]
    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        Watcher?.Dispose();
        base.Dispose(disposing);
    }
}