using ViennaNET.Extensions.Configuration.Kubernetes.Internals;

namespace ViennaNET.Extensions.Configuration.Kubernetes.Tests;

public class JsonConfigurationFileParserTests
{
    [Test]
    public void ParseStream_WithRootElement_NoObject_Throws_FormatException()
    {
        Assert.That(() => JsonConfigurationFileParser.Parse(@"[]"),
            Throws.TypeOf<FormatException>().With.Message.EqualTo("Не поддерживаемый JSON токен Array"));
    }

    [Test]
    public void VisitValue_WithDuplications_Throws_FormatException()
    {
        const string json = @"{
            ""SomeOption1"":""Value1"",
            ""SomeOption1"":""Value2"",
        }";

        Assert.That(() => JsonConfigurationFileParser.Parse(json), 
            Throws.TypeOf<FormatException>().With.Message.EqualTo("Дубликат ключа SomeOption1"));
    }
    
    [Test]
    public void VisitValue_WithArray_Returns_Data()
    {
        const string json = @"{
            ""Array1"": [
                {""Item1"": 1}
            ]
        }";

        Assert.That(JsonConfigurationFileParser.Parse(json),
            Has.Count.EqualTo(1).And.ItemAt("Array1:0:Item1").EqualTo("1"));
    }
    
    [Test]
    public void VisitValue_WithObject_Returns_Data()
    {
        const string json = @"{
            ""SomeOption"": {
                ""SomeChildOption"": 1
            }
        }";

        Assert.That(JsonConfigurationFileParser.Parse(json),
            Has.Count.EqualTo(1).And.ItemAt("SomeOption:SomeChildOption").EqualTo("1"));
    }
}