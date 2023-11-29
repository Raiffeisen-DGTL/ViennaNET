using System;
using Newtonsoft.Json;
using NUnit.Framework;
using ViennaNET.Redis.Exceptions;
using ViennaNET.Redis.Utils;

namespace ViennaNET.Redis.Tests.Unit.Utils
{
  [TestFixture(Category = "Unit", TestOf = typeof(JsonUtils))]
  public class JsonUtilsTests
  {
    private class SuccessClass
    {
      public int? Field1 { get; set; }
      public string Field2 { get; set; }
    }

    private class ExceptionJsonClass
    {
      public string FieldError => throw new JsonException();
    }

    private class ExceptionClass
    {
      public string FieldError => throw new Exception();
    }

    [TestCase(@"{""Field1"":123,""Field2"":""test""}", 123, "test")]
    [TestCase(@"{""Field1"":null,""Field2"":null}", null, null)]
    public void DeserializeObject_Object(string json, int? exceptedField1, string exceptedField2)
    {
      var actual = JsonUtils.DeserializeObject<SuccessClass>(json);

      Assert.AreEqual(exceptedField1, actual.Field1);
      Assert.AreEqual(exceptedField2, actual.Field2);
    }

    [Test]
    public void DeserializeObject_Null()
    {
      var actual = JsonUtils.DeserializeObject<SuccessClass>(null);

      Assert.IsNull(actual);
    }

    [Test]
    public void DeserializeObject_ExceptionJson()
    {
      Assert.Catch<RedisException>(() => JsonUtils.DeserializeObject<SuccessClass>("invalid-data"));
    }

    [TestCase(123, "test", @"{""Field1"":123,""Field2"":""test""}")]
    [TestCase(null, null, @"{""Field1"":null,""Field2"":null}")]
    public void SerializeObject_Object(int? field1, string field2, string excepted)
    {
      var testObject = new SuccessClass { Field1 = field1, Field2 = field2 };

      var actual = JsonUtils.SerializeObject(testObject);

      Assert.AreEqual(excepted, actual);
    }

    [Test]
    public void SerializeObject_Null()
    {
      var actual = JsonUtils.SerializeObject(null);

      Assert.AreEqual("null", actual);
    }

    [Test]
    public void SerializeObject_ExceptionJson()
    {
      Assert.Catch<RedisException>(() => JsonUtils.SerializeObject(new ExceptionJsonClass()));
    }

    [Test]
    public void SerializeObject_Exception()
    {
      Assert.Catch<Exception>(() => JsonUtils.SerializeObject(new ExceptionClass()));
    }
  }
}