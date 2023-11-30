using Microsoft.Extensions.Configuration;

namespace ViennaNET.Extensions.Configuration.HashicorpVault.Internals;

/// <summary>
///     Предоставляет утилиты для работы со строками и преобразованиями в <see cref="ConfigurationPath" />.
/// </summary>
public static class ConfigurationPathUtils
{
    /// <summary>
    ///     Нормализует указанный <paramref name="key" />, заменяя символы ".", "_", "-" на
    ///     <see cref="ConfigurationPath.KeyDelimiter" />.
    /// </summary>
    /// <param name="key">Ключ - параметра конфигурации.</param>
    /// <returns>Нормализованная версия ключа.</returns>
    public static string ReplaceKeyDelimiter(this string key)
    {
        return key
            .Replace(".", ConfigurationPath.KeyDelimiter)
            .Replace("_", ConfigurationPath.KeyDelimiter)
            .Replace("-", ConfigurationPath.KeyDelimiter);
    }
}