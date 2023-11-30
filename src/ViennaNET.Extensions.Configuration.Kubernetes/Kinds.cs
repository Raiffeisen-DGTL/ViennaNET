namespace ViennaNET.Extensions.Configuration.Kubernetes;

/// <summary>
///     Представляет разновидности ресурсов K8S.
/// </summary>
public enum Kinds
{
    /// <summary>
    ///     Представляет ConfigMap ресурс.
    /// </summary>
    ConfigMap = 0,

    /// <summary>
    ///     Представляет Secret ресурс.
    /// </summary>
    Secret = 1
}