#nullable disable

using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ViennaNET.Extensions.Configuration.Kubernetes.Internals;

/// <summary>
///     Разбирает JSON документ в словарь параметров конфигурации в формате ключ/значение.
/// </summary>
/// <remarks>
///     Скопированно с
///     <a
///         href="https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Configuration.Json/">
///         JsonConfigurationFileParser.cs.
///     </a>
/// </remarks>
public sealed class JsonConfigurationFileParser
{
    private readonly Stack<string> _context = new();

    private readonly IDictionary<string, string> _data =
        new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private string _currentPath;
    private JsonConfigurationFileParser() { }

    /// <summary>
    ///     Разбирает указанный <paramref name="input" />.
    /// </summary>
    /// <param name="input">JSON документ, содержащий параметры конфигурации.</param>
    /// <returns>Словарь параметров конфигурации.</returns>
    public static IDictionary<string, string> Parse(string input)
    {
        return new JsonConfigurationFileParser().ParseFromString(input);
    }

    private IDictionary<string, string> ParseFromString(string input)
    {
        _data.Clear();

        var jsonDocumentOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true
        };

        using (var doc = JsonDocument.Parse(input, jsonDocumentOptions))
        {
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new FormatException($"Не поддерживаемый JSON токен {doc.RootElement.ValueKind}");
            }

            VisitElement(doc.RootElement);
        }

        return _data;
    }


    private void VisitElement(JsonElement element)
    {
        foreach (var property in element.EnumerateObject())
        {
            EnterContext(property.Name);
            VisitValue(property.Value);
            ExitContext();
        }
    }

    private void VisitValue(JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                VisitElement(value);
                break;

            case JsonValueKind.Array:
                var index = 0;
                foreach (var arrayElement in value.EnumerateArray())
                {
                    EnterContext(index.ToString());
                    VisitValue(arrayElement);
                    ExitContext();
                    index++;
                }

                break;

            case JsonValueKind.Number:
            case JsonValueKind.String:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                var key = _currentPath;
                if (_data.ContainsKey(key))
                {
                    throw new FormatException($"Дубликат ключа {key}");
                }

                _data[key] = value.ToString();
                break;
        }
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