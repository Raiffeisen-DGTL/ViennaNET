using System.Text.Json;
using System.Text.Json.Serialization;

namespace ViennaNET.KasperskyScanEngine.Client;

internal sealed class JsonValueConverterBooleanString : JsonConverter<bool>
{
    /// <inheritdoc />
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.String when reader.GetString() == "true" => true,
            JsonTokenType.String when reader.GetString() == "false" => false,
            _ => throw new JsonException()
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}