using k8s;

namespace ViennaNET.Extensions.Configuration.Kubernetes;

/// <summary>
///     Сборщик клиента API K8S
/// </summary>
public interface IKubernetesClientBuilder
{
    /// <summary>
    ///     Явный путь к файлу kubeconfig.
    ///     Если установленно <see langword="null" />, тогда используется путь по умолчанию.
    /// </summary>
    /// <remarks>
    ///     Подробнее, читайте в
    ///     <a href="https://kubernetes.io/docs/concepts/configuration/organize-cluster-access-kubeconfig/">
    ///         Organizing Cluster
    ///         Access Using kubeconfig Files
    ///     </a>
    /// </remarks>
    public string? ConfigPath { get; set; }

    /// <summary>
    ///     Контекст в файле конфигурации. Используйте <see langword="null" />,
    ///     если не хотите переопределять контекст по умолчанию.
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    ///     Конечная точка сервера kube api.
    /// </summary>
    public string? MasterUrl { get; set; }

    /// <summary>
    ///     Когда <see langword="true" />, пути в файле kubeconfig будут считаться относительными к каталогу,
    ///     в котором находится файл kubeconfig. Когда <see langword="false" />, пути будут считаться
    ///     относительными к текущему рабочему каталогу.
    /// </summary>
    public bool UseRelativePaths { get; set; }

    /// <summary>
    ///     Создаёт и инициализирует клиент API K8S <see cref="IKubernetes" />.
    /// </summary>
    /// <returns>Ссылка на объект <see cref="IKubernetes" />.</returns>
    public IKubernetes Build();
}