using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using NUnit.Framework;
using ViennaNET.Redis.Exceptions;
using ViennaNET.Redis.Utils;

namespace ViennaNET.Redis.Tests.Unit.Utils;

[TestFixture(Category = "Unit", TestOf = typeof(JsonUtils))]
public class JsonUtilsTests
{
    private class SuccessClass
    {
        public int? Field1 { get; set; }
        public string? Field2 { get; set; }
    }

    private class ExceptionJsonClass
    {
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Вызывается с помощью отражения")]
        public string FieldError => throw new JsonException();
    }

    private class ExceptionClass
    {
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Вызывается с помощью отражения")]
        public string FieldError => throw new Exception();
    }

    [TestCase(@"{""Field1"":123,""Field2"":""test""}", 123, "test")]
    [TestCase(@"{""Field1"":null,""Field2"":null}", null, null)]
    public void DeserializeObject_Object(string json, int? exceptedField1, string? exceptedField2)
    {
        var actual = JsonUtils.DeserializeObject<SuccessClass>(json);

        Assert.Multiple(() =>
        {
            Assert.That(exceptedField1, Is.EqualTo(actual.Field1));
            Assert.That(exceptedField2, Is.EqualTo(actual.Field2));
        });
    }

    [Test]
    public void DeserializeObject_Null()
    {
        Assert.That(JsonUtils.DeserializeObject<SuccessClass>(null!), Is.Null);
    }

    [Test]
    public void DeserializeObject_ExceptionJson()
    {
        Assert.Catch<RedisException>(() => JsonUtils.DeserializeObject<SuccessClass>("invalid-data"));
    }

    [TestCase(123, "test", @"{""Field1"":123,""Field2"":""test""}")]
    [TestCase(null, null, @"{""Field1"":null,""Field2"":null}")]
    public void SerializeObject_Object(int? field1, string? field2, string excepted)
    {
        var value = new SuccessClass { Field1 = field1, Field2 = field2 };

        Assert.That(JsonUtils.SerializeObject(value), Is.EqualTo(excepted));
    }

    [Test]
    public void SerializeObject_Null()
    {
        Assert.That(JsonUtils.SerializeObject(null!), Is.EqualTo("null"));
    }

    [Test]
    public void SerializeObject_ExceptionJson()
    {
        Assert.That(() => JsonUtils.SerializeObject(new ExceptionJsonClass()), Throws.InstanceOf<RedisException>());
    }

    [Test]
    public void SerializeObject_Exception()
    {
        Assert.That(() => JsonUtils.SerializeObject(new ExceptionClass()), Throws.Exception);
    }
}