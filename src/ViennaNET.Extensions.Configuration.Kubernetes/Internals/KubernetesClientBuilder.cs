using System.Diagnostics.CodeAnalysis;
using k8s;

namespace ViennaNET.Extensions.Configuration.Kubernetes.Internals;

[ExcludeFromCodeCoverage(Justification = "Не содержит логики, которую следовало бы протестировать.")]
internal sealed class KubernetesClientBuilder : IKubernetesClientBuilder
{
    public string? ConfigPath { get; set; }
    public string? Context { get; set; }
    public string? MasterUrl { get; set; }
    public bool UseRelativePaths { get; set; }

    public IKubernetes Build()
    {
        var config = KubernetesClientConfiguration.IsInCluster()
            ? KubernetesClientConfiguration.InClusterConfig()
            : KubernetesClientConfiguration.BuildConfigFromConfigFile(ConfigPath, Context, MasterUrl, UseRelativePaths);

        return new k8s.Kubernetes(config);
    }
}