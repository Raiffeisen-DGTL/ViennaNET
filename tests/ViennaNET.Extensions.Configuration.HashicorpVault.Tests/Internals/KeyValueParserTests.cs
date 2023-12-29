using System.Text.Json.Nodes;
using System.Linq;
using System.Text.Json;
using ViennaNET.Extensions.Configuration.HashicorpVault.Internals;

namespace ViennaNET.Extensions.Configuration.HashicorpVault.Tests.Internals;

public class KeyValueParserTests
{
    [Test]
    public void ParseFromJObject_Returns_Data()
    {
        var docOptions = new JsonDocumentOptions() { AllowTrailingCommas = true };
        var inputData = JsonNode
            .Parse(Given.AppSettingsJsonV1, documentOptions: docOptions)?.AsObject()
            .ToDictionary(pair => pair.Key, pair => pair.Value as object);
        var outputData = KeyValueParser.Parse(inputData);

        Assert.Multiple(() =>
        {
            Assert.That(outputData,
                Has.ItemAt("ConnectionStrings:TestDB")
                    .Contains("Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=True;"));
            Assert.That(outputData, Has.ItemAt("RootIntProp").Contains("12345"));
            Assert.That(outputData, Has.ItemAt("RootTimeSpanProp").Contains("02:00:10"));
            Assert.That(outputData, Has.ItemAt("RootArraySimple:0").Contains("Item1"));
            Assert.That(outputData, Has.ItemAt("RootArraySimple:1").Contains("Item2"));
            Assert.That(outputData, Has.ItemAt("RootArrayOfObj:0:BoolProp").Contains("true"));
            Assert.That(outputData, Has.ItemAt("RootArrayOfObj:0:DateTimeProp").Contains("2023-02-15"));
            Assert.That(outputData, Has.ItemAt("RootArrayOfObj:1:BoolProp").Contains("true"));
            Assert.That(outputData, Has.ItemAt("RootArrayOfObj:1:DateTimeProp").Contains("2022-02-15"));
            Assert.That(outputData, Has.ItemAt("RootObj:UriProp").Contains("https://test.raiffeisen.ru"));
            Assert.That(outputData, Has.ItemAt("ChildSection:ArraySimple:0").Contains("Item1"));
            Assert.That(outputData, Has.ItemAt("ChildSection:ArraySimple:1").Contains("Item2"));
        });
    }

    [Test]
    public void ParseFromDictionary()
    {
        var inputData = new Dictionary<string, object>()
        {
            { "ConnectionStrings-TestDB", "Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=True;" },
            { "RootIntProp", "12345" },
            { "RootTimeSpanProp", "02:00:10" },
            { "RootArraySimple_0", "Item1" },
            { "RootArraySimple_1", "Item2" },
            { "RootArrayOfObj.0.BoolProp", "True" },
            { "RootArrayOfObj.0.DateTimeProp", "2023-02-15" },
            { "RootArrayOfObj.1.BoolProp", "False" },
            { "RootArrayOfObj.1.DateTimeProp", "2022-02-11" },
            { "RootObj.UriProp", "https://test.raiffeisen.ru" },
            { "ChildSection-ArraySimple-0", "Item1" },
            { "ChildSection-ArraySimple-1", "Item2" },
        };

        var outputData = KeyValueParser.Parse(inputData);

        Assert.Multiple(() =>
        {
            Assert.That(outputData,
                Has.ItemAt("ConnectionStrings:TestDB")
                    .Contains("Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=True;"));
            Assert.That(outputData, Has.ItemAt("RootIntProp").Contains("12345"));
            Assert.That(outputData, Has.ItemAt("RootTimeSpanProp").Contains("02:00:10"));
            Assert.That(outputData, Has.ItemAt("RootArraySimple:0").Contains("Item1"));
            Assert.That(outputData, Has.ItemAt("RootArraySimple:1").Contains("Item2"));
            Assert.That(outputData, Has.ItemAt("RootArrayOfObj:0:BoolProp").Contains("True"));
            Assert.That(outputData, Has.ItemAt("RootArrayOfObj:0:DateTimeProp").Contains("2023-02-15"));
            Assert.That(outputData, Has.ItemAt("RootArrayOfObj:1:BoolProp").Contains("False"));
            Assert.That(outputData, Has.ItemAt("RootArrayOfObj:1:DateTimeProp").Contains("2022-02-11"));
            Assert.That(outputData, Has.ItemAt("RootObj:UriProp").Contains("https://test.raiffeisen.ru"));
            Assert.That(outputData, Has.ItemAt("ChildSection:ArraySimple:0").Contains("Item1"));
            Assert.That(outputData, Has.ItemAt("ChildSection:ArraySimple:1").Contains("Item2"));
        });
    }

    [Test]
    public void ParseFromDictionary_Throws_FormatException()
    {
        var inputData = new Dictionary<string, object>()
        {
            { "RootIntProp", "12345" }, { "RootTimeSpanProp", "02:00:10" }, { "RootArraySimple__0", "Item1" }
        };

        Assert.That(() => KeyValueParser.Parse(inputData), Throws.TypeOf<FormatException>());
    }
}