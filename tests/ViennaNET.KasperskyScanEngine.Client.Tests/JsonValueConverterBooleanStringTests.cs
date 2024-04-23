using System.Text.Json;

namespace ViennaNET.KasperskyScanEngine.Client.Tests;

public class JsonValueConverterBooleanStringTests
{
    private static readonly JsonSerializerOptions options = new()
    {
        Converters = { new JsonValueConverterBooleanString() }
    };

    [TestCase(@"{""Value"":true}", ExpectedResult = true)]
    [TestCase(@"{""Value"":false}", ExpectedResult = false)]
    [TestCase(@"{""Value"":""true""}", ExpectedResult = true)]
    [TestCase(@"{""Value"":""false""}", ExpectedResult = false)]
    public bool Read_Returns_ExpectedValue(string json)
    {
        return JsonSerializer.Deserialize<Data>(json, options)!.Value;
    }

    [Test]
    public void Read_Throws_JsonException()
    {
        Assert.That(() => JsonSerializer.Deserialize<Data>(@"{""Value"":""not bool""}", options),
            Throws.InstanceOf<JsonException>());
    }

    [Test]
    public void Write_Returns_Valid_Json()
    {
        Assert.That(JsonSerializer.Serialize(new Data(true), options), Is.EqualTo(@"{""Value"":true}"));
    }

    private record Data(bool Value);
}