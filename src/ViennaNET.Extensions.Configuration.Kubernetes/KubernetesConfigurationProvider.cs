using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using k8s;
using Microsoft.Extensions.Configuration;
using ViennaNET.Extensions.Configuration.Kubernetes.Internals;

namespace ViennaNET.Extensions.Configuration.Kubernetes;

/// <summary>
///     Представляет базовый класс для <see cref="ConfigurationProvider" /> на основе ресурсов Kubernetes.
/// </summary>
public abstract class KubernetesConfigurationProvider : ConfigurationProvider, IDisposable
{
    /// <summary>
    ///     Инициализирует новый экземпляр с указанным источником, и настроенным клиентом API K8S.
    /// </summary>
    /// <param name="source">Ссылка на <see cref="KubernetesConfigurationSource" />.</param>
    /// <param name="builder">Ссылка на <see cref="IKubernetesClientBuilder" />.</param>
    /// <exception cref="ArgumentNullException">
    ///     Возникает, если <paramref name="source" /> = <see langword="null" />.
    /// </exception>
    protected KubernetesConfigurationProvider(KubernetesConfigurationSource source, IKubernetesClientBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        Source = source ?? throw new ArgumentNullException(nameof(source));
        Client = builder.Build();
    }

    /// <summary>
    ///     Ссылка на <see cref="KubernetesConfigurationSource" />.
    /// </summary>
    protected KubernetesConfigurationSource Source { get; }

    /// <summary>
    ///     Ссылка на <see cref="IKubernetes" />.
    /// </summary>
    protected IKubernetes Client { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage(Justification = "Завершение делегируются. Нет логики для тестирования")]
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Получает ресурс из K8S и инициализирует <see cref="Watcher{T}" />, для автоматической перезагрузки.
    /// </summary>
    protected abstract void Watch();

    /// <summary>
    ///     Единожды получает ресурс из K8S.
    /// </summary>
    protected abstract void Single();

    /// <inheritdoc />
    public sealed override void Load()
    {
        if (Source.ReloadOnChange)
        {
            // Watch срабатывает поздно. Некоторые параметры конфигурации, нужны при старте.
            // Поэтому загружаем их синхронно, а потом настраиваем перезагрузку через Watch.
            Single();
            Watch();
        }
        else
        {
            Single();
        }
    }

    /// <summary>
    ///     Разбирает данные в формате JSON.
    /// </summary>
    /// <param name="data">JSON документ для разбора.</param>
    /// <exception cref="FormatException">
    ///     Возникает, если не удалось разобрать JSON, из-за некоректного формата данных.
    /// </exception>
    protected void Parse(string data)
    {
        try
        {
            Data = JsonConfigurationFileParser.Parse(data);
        }
        catch (JsonException e)
        {
            throw new FormatException(
                $"Не удалось разобрать JSONFile {Source.FileName} из {Source.Namespace}.{Source.Name}", e);
        }
    }

    /// <summary>
    ///     Разбирает данные в формате ключ/значение.
    /// </summary>
    /// <param name="data">Словарь данных.</param>
    /// <exception cref="FormatException">
    ///     Возникает, если любой из ключей не соответствует выражению ^[-._a-zA-Z0-9]+$.
    /// </exception>
    protected void Parse(IDictionary<string, string> data)
    {
        const string pattern = "^[-._a-zA-Z0-9]+$";

        var regex = new Regex(pattern);

        if (data.Keys.Any(key => !regex.IsMatch(key)))
        {
            throw new FormatException(
                $"Не удалось разобрать {Source.Namespace}.{Source.Name}. " +
                "Данные должны быть в формате ключ/значение." +
                $"Все ключи, должны соответствовать выражению: {pattern}");
        }

        Data = data.ToDictionary(item => Normalize(item.Key), item => item.Value);
    }

    private static string Normalize(string key)
    {
        return key
            .Replace(".", ConfigurationPath.KeyDelimiter)
            .Replace("_", ConfigurationPath.KeyDelimiter)
            .Replace("-", ConfigurationPath.KeyDelimiter);
    }


    /// <summary>
    ///     Вызывает <see cref="Client" />.<see cref="IDisposable.Dispose" />.
    /// </summary>
    /// <param name="disposing">
    ///     Указывает, необходимо ли освобождать ресурсы. Предотвращает повторное освобождение.
    /// </param>
    [ExcludeFromCodeCoverage(Justification = "Завершение делегируются. Нет логики для тестирования")]
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Client.Dispose();
        }
    }
}