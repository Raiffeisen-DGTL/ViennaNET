using k8s;
using Microsoft.Extensions.Configuration;

namespace ViennaNET.Extensions.Configuration.Kubernetes;

/// <summary>
///     Предоставляет методы расширения для <see cref="IConfigurationBuilder" />.
/// </summary>
public static class KubernetesConfigurationExtensions
{
    /// <summary>
    ///     Добавляет поставщик конфигурации на основе ресурса ConfigMap.
    /// </summary>
    /// <remarks>
    ///     ConfigMap должен содержать файл с данными в формате
    ///     <a
    ///         href="https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers">
    ///         appsettings.json.
    ///     </a>
    /// </remarks>
    /// <example>
    ///     Пример отслеживаемого ConfigMap c данными в формате Json.
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
    ///  </code>
    /// </example>
    /// <param name="builder">Ссылка на объект <see cref="IConfigurationBuilder" />.</param>
    /// <param name="name">Имя ConfigMap который нужно отслеживать.</param>
    /// <param name="namespace">Пространство имён, в котором расположен ConfigMap.</param>
    /// <param name="fileName">
    ///     Имя файла в свойстве <c>data</c> ресурса ConfigMap,
    ///     если не указано, то используется <b>appsettings.json</b>.
    /// </param>
    /// <param name="reloadOnChange">
    ///     Если <see langword="true" />, то при изменении ConfigMap, конфигурация автоматически перезагрузится.
    /// </param>
    /// <param name="configureClientBuilder">Действие, настраивающее сборщик клиента K8S.</param>
    /// <returns>Ссылка на объект <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddJsonConfigMap(this IConfigurationBuilder builder,
        string name,
        string? @namespace = null,
        string? fileName = null,
        bool reloadOnChange = true,
        Action<IKubernetesClientBuilder>? configureClientBuilder = null)
    {
        var metadata = (name, @namespace, Kinds.ConfigMap);

        return builder.AddKubernetes(metadata, DataTypes.Json, reloadOnChange, fileName,
            configureClientBuilder);
    }

    /// <summary>
    ///     Добавляет поставщик конфигурации на основе ресурса ConfigMap с данными в формате ключ/значение.
    /// </summary>
    /// <remarks>
    ///     Ключи привязываются к объектам конфигурации как описано в
    ///     <a href="https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration#binding-hierarchies">
    ///         Binding hierarchies
    ///     </a>
    ///     , вместо символа "<c>:</c>", допустимым разделителем может быть точка "<b>.</b>",
    ///     нижнее подчёркивание "<b>_</b>" или дефис "<b>-</b>"
    ///     .
    /// </remarks>
    /// <example>
    ///     Пример отслеживаемого ConfigMap c данными в формате Key/Value.
    ///     <code>
    ///     apiVersion: v1
    ///     kind: ConfigMap
    ///     data:
    ///       First.Key: 12345
    ///       Second_Key: 54321
    ///     </code>
    /// </example>
    /// <param name="builder">Ссылка на объект <see cref="IConfigurationBuilder" />.</param>
    /// <param name="namespace">Пространство имён, в котором расположен ConfigMap.</param>
    /// <param name="name">Имя ConfigMap который нужно отслеживать (<c>metadata.name</c>).</param>
    /// <param name="reloadOnChange">
    ///     Если <see langword="true" />, то при изменении ConfigMap, конфигурация автоматически перезагрузится.
    /// </param>
    /// <param name="configureClientBuilder">Действие, настраивающее сборщик клиента K8S.</param>
    /// <returns>Ссылка на объект <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddKeyValueConfigMap(this IConfigurationBuilder builder,
        string name,
        string? @namespace = null,
        bool reloadOnChange = true,
        Action<IKubernetesClientBuilder>? configureClientBuilder = null)
    {
        var metadata = (name, @namespace, Kinds.ConfigMap);

        return builder.AddKubernetes(metadata, DataTypes.KeyValue, reloadOnChange,
            configureClientBuilder: configureClientBuilder);
    }

    /// <summary>
    ///     Добавляет поставщик конфигурации на основе ресурса Secret.
    /// </summary>
    /// <remarks>
    ///     Secret должен содержать файл с данными в формате
    ///     <a
    ///         href="https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers">
    ///         appsettings.json
    ///     </a>
    ///     .
    /// </remarks>
    /// <example>
    ///     Пример отслеживаемого Secret c данными в формате Json.
    ///     <b>Обратите внимание на свойство <c>type</c>!</b>
    ///     <code>
    ///     apiVersion: v1
    ///     kind: Secret
    ///     type: kubernetes.io/appsecretsjson
    ///     data:
    ///       appsecret.json: eyAiU2VjcmV0RmllbGQiOiAi0KLRgNCw0L3QutC4IiB9Cg==
    /// </code>
    /// </example>
    /// <param name="builder">Ссылка на объект <see cref="IConfigurationBuilder" />.</param>
    /// <param name="name">Имя Secret который нужно отслеживать (<c>metadata.name</c>).</param>
    /// <param name="namespace">Пространство имён, в котором расположен Secret.</param>
    /// <param name="fileName">
    ///     Имя файла в свойстве <c>data</c> ресурса Secret,
    ///     если не указано то используется <b>appsecrets.json</b>.
    /// </param>
    /// <param name="reloadOnChange">
    ///     Если <see langword="true" />, то при изменении Secret, конфигурация автоматически перезагрузится.
    /// </param>
    /// <param name="configureClientBuilder">Действие, настраивающее сборщик клиента K8S.</param>
    /// <returns>Ссылка на объект <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddJsonSecret(this IConfigurationBuilder builder,
        string name,
        string? @namespace = null,
        string? fileName = null,
        bool reloadOnChange = true,
        Action<IKubernetesClientBuilder>? configureClientBuilder = null)
    {
        var metadata = (name, @namespace, Kinds.Secret);

        return builder.AddKubernetes(metadata, DataTypes.Json, reloadOnChange, fileName,
            configureClientBuilder);
    }

    /// <summary>
    ///     Добавляет поставщик конфигурации на основе ресурса Secret с данными в формате ключ/значение.
    /// </summary>
    /// <remarks>
    ///     Ключи привязываются к объектам конфигурации как описано в
    ///     <a href="https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration#binding-hierarchies">
    ///         Binding hierarchies
    ///     </a>
    ///     , вместо символа "<c>:</c>", допустимым разделителем может быть точка "<b>.</b>",
    ///     нижнее подчёркивание "<b>_</b>" или дефис "<b>-</b>"
    ///     .
    /// </remarks>
    /// <example>
    ///     Пример отслеживаемого Secret c данными в формате Key/Value.
    ///     <b>Обратите внимание на свойство <c>type</c>!</b>
    ///     <code>
    ///     apiVersion: v1
    ///     kind: Secret
    ///     type: Opaque
    ///     data:
    ///       First.Key: 12345
    ///       SECOND-SECRET: 54321
    /// </code>
    /// </example>
    /// <param name="builder">Ссылка на объект <see cref="IConfigurationBuilder" />.</param>
    /// <param name="name">Имя Secret который нужно отслеживать (<c>metadata.name</c>).</param>
    /// <param name="namespace">Пространство имён, в котором расположен Secret.</param>
    /// <param name="reloadOnChange">
    ///     Если <see langword="true" />, то при изменении ConfigMap, конфигурация автоматически перезагрузится.
    /// </param>
    /// <param name="configureClientBuilder">Действие, настраивающее сборщик клиента K8S.</param>
    /// <returns>Ссылка на объект <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddKeyValueSecret(this IConfigurationBuilder builder,
        string name, string? @namespace = null, bool reloadOnChange = true,
        Action<IKubernetesClientBuilder>? configureClientBuilder = null)
    {
        var metadata = (name, @namespace, Kinds.Secret);

        return builder.AddKubernetes(metadata,
            DataTypes.KeyValue,
            reloadOnChange,
            configureClientBuilder: configureClientBuilder);
    }

    /// <summary>
    ///     Добавляет поставщик конфигурации на основе ресурса, указанного <paramref name="metadata.kind" />,
    ///     в формате, определённом <paramref name="dataTypes" />.
    /// </summary>
    /// <param name="builder">Ссылка на объект <see cref="IConfigurationBuilder" />.</param>
    /// <param name="metadata">Метаданные ресурса, который нужно отслеживать.</param>
    /// <param name="dataTypes">Формат данных <see cref="DataTypes" />.</param>
    /// <param name="reloadOnChange">
    ///     Если <see langword="true" />, то при изменении ConfigMap, конфигурация автоматически перезагрузится.
    /// </param>
    /// <param name="fileName">Имя файла в свойстве <c>data</c>.</param>
    /// <param name="configureClientBuilder">Действие, настраивающее сборщик клиента K8S.</param>
    /// <returns>Ссылка на объект <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddKubernetes(
        this IConfigurationBuilder builder,
        (string name, string? @namespace, Kinds kind) metadata,
        DataTypes dataTypes,
        bool reloadOnChange = true,
        string? fileName = null,
        Action<IKubernetesClientBuilder>? configureClientBuilder = null)
    {
        var (name, ns, kind) = metadata;

        return builder.AddKubernetes(source =>
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                source.Name = name;
            }

            if (string.IsNullOrWhiteSpace(ns))
            {
                if (KubernetesClientConfiguration.IsInCluster())
                {
                    var icc = KubernetesClientConfiguration.InClusterConfig();
                    if (!string.IsNullOrEmpty(icc.Namespace))
                    {
                        source.Namespace = icc.Namespace;
                    }
                }
            }
            else
            {
                source.Namespace = ns;
            }

            source.FileName = fileName;
            source.Kind = kind;
            source.DataType = dataTypes;
            source.ReloadOnChange = reloadOnChange;
            configureClientBuilder?.Invoke(source.KubernetesClientBuilder);
        });
    }

    /// <summary>
    ///     Добавляет источник конфигурации K8S в <paramref name="builder" />.
    /// </summary>
    /// <param name="builder"><see cref="IConfigurationBuilder" /> для добавления.</param>
    /// <param name="configure">Настраивает источник.</param>
    /// <returns>Ссылка на объект <see cref="IConfigurationBuilder" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Возникает, если <paramref name="builder" /> = <see langword="null" />.
    /// </exception>
    public static IConfigurationBuilder AddKubernetes(this IConfigurationBuilder builder,
        Action<KubernetesConfigurationSource>? configure)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.Add(configure);
    }
}