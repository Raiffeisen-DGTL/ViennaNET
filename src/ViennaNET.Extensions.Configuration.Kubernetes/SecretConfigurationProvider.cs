using System.Diagnostics.CodeAnalysis;
using System.Text;
using k8s;
using k8s.Models;

namespace ViennaNET.Extensions.Configuration.Kubernetes;

/// <inheritdoc />
internal sealed class SecretConfigurationProvider : KubernetesConfigurationProvider
{
    private const string DefaultFileName = "appsecrets.json";

    /// <inheritdoc />
    public SecretConfigurationProvider(KubernetesConfigurationSource source, IKubernetesClientBuilder builder) : base(
        source, builder)
    {
    }

    public Watcher<V1Secret>? Watcher { get; private set; }

    protected override void Watch()
    {
        var message = Client.CoreV1.ListNamespacedSecretWithHttpMessagesAsync(Source.Namespace,
            fieldSelector: $"metadata.name={Source.Name}", watch: true);

        Watcher = message.Watch<V1Secret, V1SecretList>(OnEvent, Source.OnWatchException, Source.OnWatchClose);
    }


    internal void OnEvent(WatchEventType type, V1Secret secret)
    {
        if (type is not (WatchEventType.Modified or WatchEventType.Added))
        {
            return;
        }

        Extract(secret);
        OnReload();
    }

    protected override void Single()
    {
        Extract(Client.CoreV1.ReadNamespacedSecret(Source.Name, Source.Namespace));
    }

    private void Extract(V1Secret secret)
    {
        if (Source.DataType == DataTypes.Json)
        {
            var fileName = Source.FileName ?? DefaultFileName;

            if (secret.Type != "kubernetes.io/appsecretsjson" || secret.Data.All(pair => pair.Key != fileName))
            {
                return;
            }

            Parse(Encoding.UTF8.GetString(secret.Data.SingleOrDefault(pair => pair.Key == fileName).Value));
        }
        else
        {
            Parse(secret.Data.ToDictionary(item => item.Key, item => Encoding.UTF8.GetString(item.Value)));
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