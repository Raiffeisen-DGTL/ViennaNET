#nullable disable

using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using VaultSharp.V1.Commons;

namespace ViennaNET.Extensions.Configuration.HashicorpVault.Internals;

/// <summary>
///     Разбирает данные секрета <see cref="IDictionary{TKey,TValue}" />,
///     полученные из <see cref="SecretData" />,
///     в словарь параметров конфигурации в формате ключ/значение.
/// </summary>
/// <remarks>
///     VaultSharp полагается на Newtonsoft.Json при десериализации ответа. Поэтому тип
///     <a
///         href="https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Configuration.Json/">
///         JsonConfigurationFileParser.cs.
///     </a>
///     не подходит для разбора, единственный вариант это повторная сериализация в строку,
///     с последующей передачей в JsonConfigurationFileParser. Вместо этого, написан <see cref="KeyValueParser" />,
///     который служит той же цели но работает с данными предоставленными VaultSharp, без дополнительных преобразований.
/// </remarks>
internal sealed class KeyValueParser
{
    private readonly Stack<string> _context = new();

    private readonly IDictionary<string, string> _data =
        new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private string _currentPath;

    private KeyValueParser() { }

    /// <summary>
    ///     Разбирает указанный <paramref name="data" />.
    /// </summary>
    /// <param name="data">JSON документ, содержащий параметры конфигурации.</param>
    /// <returns>Словарь параметров конфигурации.</returns>
    public static IDictionary<string, string> Parse(IDictionary<string, object> data)
    {
        return data.Values.All(i => i is string)
            ? ParseFromDictionary(data.ToDictionary(pair => pair.Key, pair => pair.Value.ToString()))
            : new KeyValueParser().ParseFromJObject(data);
    }

    /// <summary>
    ///     Разбирает данные в формате ключ/значение.
    /// </summary>
    /// <param name="data">Словарь данных.</param>
    /// <exception cref="FormatException">
    ///     Возникает, если любой из ключей не соответствует выражению `^([a-zA-Z\\d]*[-._]{0,1}[a-zA-Z\\d]+)*$`.
    /// </exception>
    private static IDictionary<string, string> ParseFromDictionary(IDictionary<string, string> data)
    {
        const string pattern = "^([a-zA-Z\\d]*[-._]{0,1}[a-zA-Z\\d]+)*$";

        var regex = new Regex(pattern);

        if (data.Keys.Any(key => !regex.IsMatch(key)))
        {
            throw new FormatException(
                $"Данные должны быть в формате ключ/значение. Все ключи, должны соответствовать выражению: {pattern}");
        }

        return data.ToDictionary(item => item.Key.ReplaceKeyDelimiter(), item => item.Value);
    }

    /// <summary>
    ///     Разбирает данные в формате JSON.
    /// </summary>
    /// <param name="data">Словарь, который содержит экземпляры JObject.</param>
    private IDictionary<string, string> ParseFromJObject(IDictionary<string, object> data)
    {
        _data.Clear();

        foreach (var item in data)
        {
            VisitDictionaryItem(item);
        }

        return _data;
    }

    private void VisitDictionaryItem(KeyValuePair<string, object> item)
    {
        EnterContext(item.Key);

        switch (item.Value)
        {
            case JObject obj:
                VisitObject(obj);
                break;
            case JArray arr:
                VisitArray(arr);
                break;
            default:
                VisitValue(item.Value);
                break;
        }

        ExitContext();
    }

    private void VisitObject(JObject obj)
    {
        foreach (var property in obj.Properties())
        {
            EnterContext(property.Name);
            VisitJToken(property.Value);
            ExitContext();
        }
    }

    private void VisitArray(JArray array)
    {
        for (var i = 0; i < array.Count; i++)
        {
            EnterContext(i.ToString());
            VisitJToken(array[i]);
            ExitContext();
        }
    }

    private void VisitJToken(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                VisitObject(token as JObject);
                break;
            case JTokenType.Array:
                VisitArray(token as JArray);
                break;
            case JTokenType.Boolean:
            case JTokenType.String:
            case JTokenType.Float:
            case JTokenType.Integer:
            case JTokenType.Null:
                VisitValue(token);
                break;
        }
    }

    private void VisitValue(object token)
    {
        _data[_currentPath] = token.ToString();
    }

    private void EnterContext(string context)
    {
        _context.Push(context);
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    private void ExitContext()
    {
        _context.Pop();
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }
}