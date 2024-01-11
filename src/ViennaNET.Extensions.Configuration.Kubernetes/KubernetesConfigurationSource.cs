using Microsoft.Extensions.Configuration;
using ViennaNET.Extensions.Configuration.Kubernetes.Internals;

namespace ViennaNET.Extensions.Configuration.Kubernetes;

/// <summary>
///     Представляет базовый класс для <see cref="IConfigurationSource" /> на основе ресурсов Kubernetes.
/// </summary>
public sealed class KubernetesConfigurationSource : IConfigurationSource
{
    /// <summary>
    ///     Инициализирует новый экземпляр класса <see cref="KubernetesConfigurationSource" />.
    /// </summary>
    /// <remarks>
    ///     Устанавливает <see cref="Namespace" /> в значение "default", <see cref="ReloadOnChange" />
    ///     в значение <see langword="false" />, т. е. выключает автоматическую перезагрузку при изменении ресурса.
    /// </remarks>
    public KubernetesConfigurationSource()
    {
        Namespace = "default";
        Name = "appsettings";
        DataType = DataTypes.Json;
        ReloadOnChange = false;
        Kind = Kinds.ConfigMap;
        KubernetesClientBuilder = new KubernetesClientBuilder();
    }

    /// <summary>
    ///     Пространство имён, в котором осуществляется поиск конфигурации.
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    ///     Имя ресурса. Значение свойства metadata.name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Разновидность ресурса K8S. Допустимые значения ConfigMap или Secret.
    /// </summary>
    internal Kinds Kind { get; set; }

    /// <summary>
    ///     Формат в котором определены данные конфигурации <see cref="DataTypes" />.
    /// </summary>
    internal DataTypes DataType { get; set; }

    /// <summary>
    ///     Имя файла определённого в свойстве <c>data</c> ConfigMap или Secret, если ресурс содержит несколько файлов.
    ///     Если не задано, используется <c>appsettings.json</c> для ConfigMap и <c>appsecrets.json</c> для Secret
    /// </summary>
    /// <remarks>
    ///     Используется, если <see cref="KubernetesConfigurationSource.DataType" /> =
    ///     <see cref="DataTypes.Json" />.
    /// </remarks>
    /// <example>
    ///     Определение конфигурации с несколькими файлами appsettings.json.
    ///     <code>
    ///      apiVersion: v1
    ///      kind: ConfigMap
    ///      data:
    ///        appsettings.json: |
    ///          {
    ///            "Logging": {
    ///              "LogLevel": {
    ///                 "Default": "Information"
    ///              }
    ///            }
    ///          }
    ///         appsettings.Development.json: |
    ///          {
    ///            "Logging": {
    ///              "LogLevel": {
    ///                 "Default": "Debug"
    ///              }
    ///            },
    ///            "AllowedHosts": "*"
    ///          }
    ///  </code>
    /// </example>
    public string? FileName { get; set; }

    /// <summary>
    ///     Указывает на необходимость, автоматической перезагрузки конфигурации, при изменении источника.
    ///     Если установлено <see langword="true" />, отслеживает целевой ресурс K8S.
    /// </summary>
    public bool ReloadOnChange { get; set; }

    /// <summary>
    ///     Действие, которое следует вызвать, когда <see cref="ReloadOnChange" />
    ///     установлено в <see langword="true" /> и во время отслеживания возникло какое-либо исключение.
    /// </summary>
    public Action<Exception>? OnWatchException { get; set; }

    /// <summary>
    ///     Действие, которое следует вызвать, когда <see cref="ReloadOnChange" />
    ///     установлено в <see langword="true" /> и сервер закрывает соединение.
    /// </summary>
    public Action? OnWatchClose { get; set; }

    /// <summary>
    ///     Ссылка на объект <see cref="IKubernetesClientBuilder" />.
    /// </summary>
    public IKubernetesClientBuilder KubernetesClientBuilder { get; set; }

    /// <exception cref="ArgumentOutOfRangeException">
    ///     Возникает, если значение свойства <see cref="Kind" />,
    ///     не является элементом перечисления <see cref="Kinds" />.
    /// </exception>
    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return Kind switch
        {
            Kinds.ConfigMap => new ConfigMapConfigurationProvider(this, KubernetesClientBuilder),
            Kinds.Secret => new SecretConfigurationProvider(this, KubernetesClientBuilder),
            _ => throw new InvalidOperationException(
                "Поддерживаются только ресурсы типа Secret или ConfigMap.")
        };
    }
}