namespace ViennaNET.Extensions.Configuration.Kubernetes;

/// <summary>
///     Представляет формат в котором определены значения свойства <c>deta</c> секрета или конфигурации.
/// </summary>
/// <example>
///     <code>
///     apiVersion: v1
///     kind: ConfigMap или Secret
///     data:
///         #KeyValue или Json
/// </code>
/// </example>
public enum DataTypes
{
    /// <summary>
    ///     В формате ключ/значение.
    /// </summary>
    /// <remarks>
    ///     Если <c>Kind: Secret</c>, тогда должно быть задано значение поля <c>type: Opaque</c> как в примере ниже.
    ///     <para>
    ///         Ключи привязываются к объектам конфигурации как описано в
    ///         <a href="https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration#binding-hierarchies">
    ///             Binding hierarchies
    ///         </a>
    ///         , вместо символа "<c>:</c>", допустимым разделителем может быть точка "<b>.</b>",
    ///         нижнее подчёркивание "<b>_</b>" или дефис "<b>-</b>"
    ///     </para>
    ///     .
    /// </remarks>
    /// <example>
    ///     Определение конфигурации K8S с данными ключ/значение
    ///     <code>
    ///     apiVersion: v1
    ///     kind: ConfigMap
    ///     data:
    ///       First.Key: 12345
    ///       Second_Key: 54321
    ///     </code>
    ///     Определение секрета K8S с данными в формате ключ/значение
    ///     <code>
    ///     apiVersion: v1
    ///     kind: Secret
    ///     type: Opaque
    ///     data:
    ///       First.Key: 12345
    ///       SECOND-SECRET: 54321
    /// </code>
    /// </example>
    KeyValue = 0,

    /// <summary>
    ///     Данными в формате
    ///     <a
    ///         href="https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers">
    ///         appsettings.json
    ///     </a>
    ///     .
    /// </summary>
    /// <remarks>
    ///     Если <c>Kind: Secret</c>, тогда должно быть задано значение поля
    ///     <c>type: kubernetes.io/appsecretsjson</c> как в примере ниже.
    /// </remarks>
    /// <example>
    ///     Определение секрета K8S с данными в формате appsettings.json.
    ///     <code>
    ///      apiVersion: v1
    ///      kind: ConfigMap
    ///      data:
    ///        appsettings.json: |
    ///          {
    ///            "Logging": {
    ///              "LogLevel": {
    ///                 "Default": "Debug"
    ///              }
    ///            },
    ///            "AllowedHosts": "*"
    ///          }
    ///  </code>
    ///     Определение секрета K8S с данными в формате appsettings.json.
    ///     <code>
    ///     apiVersion: v1
    ///     kind: Secret
    ///     type: kubernetes.io/appsecretsjson
    ///     data:
    ///       appsecret.json: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    /// </code>
    /// </example>
    Json = 1
}