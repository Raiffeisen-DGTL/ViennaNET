using System;
using NUnit.Framework;

namespace ViennaNET.Utils.Tests
{
  [TestFixture(Category = "Unit")]
  public class MayBeMonadeTests
  {
    private class Foo
    {
      public Foo(long value)
      {
        Value = value;
      }

      public long Value { get; }
    }

    [Test]
    public void ThrowIf_WhenPredicateIsFalse_ShouldReturnValue()
    {
      var objValue = new { a = 1 };
      object? objNull = null;
      var structValue = 1;
      int? structNull = null;

      Assert.AreEqual(objValue, objValue.ThrowIf(_ => false, "error message"));
      Assert.AreEqual(objValue, objValue.ThrowIf(_ => false, new Exception("error message")));
      Assert.AreEqual(objValue, objValue.ThrowIf(_ => false, a => new Exception($"error message: {a}")));

      Assert.AreEqual(objNull, objNull.ThrowIf(_ => false, "error message"));
      Assert.AreEqual(objNull, objNull.ThrowIf(_ => false, new Exception("error message")));
      Assert.AreEqual(objNull, objNull.ThrowIf(_ => false, a => new Exception($"error message: {a}")));

      Assert.AreEqual(structValue, structValue.ThrowIf(_ => false, "error message"));
      Assert.AreEqual(structNull, structNull.ThrowIf(_ => false, "error message"));
    }

    [Test]
    public void ThrowIf_WhenPredicateIsTrue_ShouldThrowException()
    {
      var objValue = new { a = 1 };
      object? objNull = null;
      var structValue = 1;
      int? structNull = null;

      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIf(_ => true, "error message");
      });
      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIf(_ => true, new Exception("error message"));
      });
      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIf(_ => true, a => new Exception($"error message: {a}"));
      });

      Assert.AreEqual(objNull, objNull.ThrowIf(_ => true, "error message"));
      Assert.AreEqual(objNull, objNull.ThrowIf(_ => true, new Exception("error message")));
      Assert.AreEqual(objNull, objNull.ThrowIf(_ => true, a => new Exception($"error message: {a}")));

      Assert.Throws<Exception>(() =>
      {
        var _ = structValue.ThrowIf(_ => true, "error message");
      });
      Assert.AreEqual(structNull, structNull.ThrowIf(_ => false, "error message"));
    }

    [Test]
    public void ThrowIf_WhenPredicateIsTrue_ButTargetIsNull_ShouldReturnNull()
    {
      object? objNull = null;
      int? structNull = null;

      Assert.IsNull(objNull.ThrowIf(_ => true, "error message"));
      Assert.IsNull(objNull.ThrowIf(_ => true, new Exception("error message")));
      Assert.IsNull(objNull.ThrowIf(_ => true, a => new Exception($"error message: {a}")));

      Assert.IsNull(structNull.ThrowIf(_ => false, "error message"));
    }

    [Test]
    public void ThrowIfNot_WhenPredicateIsTrue_ShouldReturnValue()
    {
      var objValue = new { a = 1 };
      object? objNull = null;

      Assert.AreEqual(objValue, objValue.ThrowIfNot(_ => true, new Exception("error message")));
      Assert.AreEqual(objValue, objValue.ThrowIfNot(_ => true, a => new Exception($"error message: {a}")));

      Assert.AreEqual(objNull, objNull.ThrowIfNot(_ => true, new Exception("error message")));
      Assert.AreEqual(objNull, objNull.ThrowIfNot(_ => true, a => new Exception($"error message: {a}")));
    }

    [Test]
    public void ThrowIfNot_WhenPredicateIsFalse_ShouldThrowException()
    {
      var objValue = new { a = 1 };

      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIfNot(_ => false, new Exception("error message"));
      });
      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIfNot(_ => false, a => new Exception($"error message: {a}"));
      });
    }

    [Test]
    public void ThrowIfNot_WhenPredicateIsFalse_ButTargetIsNull_ShouldReturnNull()
    {
      object? objNull = null;

      Assert.IsNull(objNull.ThrowIfNot(_ => false, new Exception("error message")));
      Assert.IsNull(objNull.ThrowIfNot(_ => false, a => new Exception($"error message: {a}")));
    }

    [Test]
    public void At_WhenCollectionIsNull_ShouldReturnFailureValue()
    {
      // Arrange
      var failureValue = 0xFA1L;
      Foo[]? collection = null;

      // Act
      var result = collection.At(5, f => f.Value * 10, failureValue);

      // Assert
      Assert.That(result, Is.EqualTo(failureValue));
    }

    [Test]
    public void At_WhenIndexIsOutOfRange_ShouldReturnFailureValue()
    {
      // Arrange
      var failureValue = 0xFA1L;
      var collection = new[] { new Foo(1), new Foo(2), new Foo(3) };

      // Act
      var result = collection.At(collection.Length, f => f.Value * 10, failureValue);

      // Assert
      Assert.That(result, Is.EqualTo(failureValue));
    }

    [Test]
    public void At_ShouldReturnEvaluationResult()
    {
      // Arrange
      var failureValue = 0xFA1L;
      var collection = new[] { new Foo(1), new Foo(2), new Foo(3) };

      // Act
      var result = collection.At(1, f => f.Value * 10, failureValue);

      // Assert
      Assert.That(result, Is.EqualTo(20));
    }

    [Test]
    public void Do_WhenObjIsNull_ShouldNotInvokeActionAndReturnNull()
    {
      object? obj = null;
      int? objStruct = null;
      var called = false;

      var result = obj.Do(o => called = true);
      var resultStruct = objStruct.Do(o => called = true);

      Assert.Multiple(() =>
      {
        Assert.That(called, Is.False);
        Assert.That(result, Is.Null);
        Assert.That(resultStruct, Is.Null);
      });
    }

    [Test]
    public void Do_ShouldNotInvokeActionWithObjectAndReturnNull()
    {
      var obj = new Foo(123);
      int? objStruct = 123;
      var called = false;

      var result = obj.Do(o =>
      {
        Assert.That(o, Is.SameAs(obj));
        called = true;
      });
      Assert.Multiple(() =>
      {
        Assert.That(result, Is.SameAs(obj));
        Assert.That(called, Is.True);
      });

      called = false;
      var resultStruct = objStruct.Do(o =>
      {
        Assert.That(o, Is.EqualTo(objStruct));
        called = true;
      });
      Assert.Multiple(() =>
      {
        Assert.That(resultStruct, Is.EqualTo(objStruct));
        Assert.That(called, Is.True);
      });
    }

    [Test]
    public void If_Object_ShouldReturnCorrectResult()
    {
      Foo? fooNull = null;
      Foo foo = new(123);

      Assert.IsNull(fooNull.If(f => f.Value == 0x404));
      Assert.IsNull(foo.If(f => f.Value == 0x404));

      Assert.That(foo.If(f => f.Value == 123), Is.SameAs(foo));
    }

    [Test]
    public void IfNot_Object_ShouldReturnCorrectResult()
    {
      Foo? fooNull = null;
      Foo foo = new(0x404);

      Assert.IsNull(fooNull.IfNot(f => f.Value == 0x404));
      Assert.IsNull(foo.IfNot(f => f.Value == 0x404));

      Assert.That(foo.IfNot(f => f.Value == 123), Is.SameAs(foo));
    }

    [Test]
    public void If_Struct_ShouldReturnCorrectResult()
    {
      int? valueNull = null;
      int? value = 123;

      Assert.IsNull(valueNull.If(_ => false));
      Assert.IsNull(value.If(_ => false));

      Assert.That(value.If(_ => true), Is.EqualTo(value));
    }

    [Test]
    public void IfNot_Struct_ShouldReturnCorrectResult()
    {
      int? valueNull = null;
      int? value = 123;

      Assert.IsNull(valueNull.IfNot(_ => true));
      Assert.IsNull(value.IfNot(_ => true));

      Assert.That(value.IfNot(f => false), Is.EqualTo(value));
    }

    [Test]
    public void Return_ShouldReturnInputEvaluationIfItsNotNull()
    {
      var result = new object();
      Foo? fooNull = null;
      var foo = new Foo(123);
      int? valueNull = null;
      int? value = 123;

      Assert.IsNull(valueNull.Return(v => result));
      Assert.IsNull(fooNull.Return(f => result));

      Assert.That(foo.Return(f => result), Is.EqualTo(result));
      Assert.That(value.Return(f => result), Is.EqualTo(result));
    }

    [Test]
    public void ReturnSuccess_ShouldReturnIfInputIsNotNull()
    {
      Foo? fooNull = null;
      var foo = new Foo(123);

      Assert.That(fooNull.ReturnSuccess(), Is.False);
      Assert.That(foo.ReturnSuccess(), Is.True);
    }

    [Test]
    public void IfNull_ShouldCallActionIfInputIsNotNull()
    {
      Foo? fooNull = null;
      var foo = new Foo(123);

      var called = false;

      void SetCalledTrue()
      {
        called = true;
      }

      Assert.That(foo.IfNull(SetCalledTrue), Is.SameAs(foo));
      Assert.That(called, Is.False);

      Assert.That(fooNull.IfNull(SetCalledTrue), Is.Null);
      Assert.That(called, Is.True);
    }

    [Test]
    public void ThrowIfNull_ShouldThrowIfInputIsNull()
    {
      Foo? fooNull = null;
      var foo = new Foo(123);

      Assert.That(foo.ThrowIfNull(nameof(foo)), Is.SameAs(foo));

      var exception = Assert.Throws<ArgumentNullException>(() => fooNull.ThrowIfNull(nameof(foo)));
      Assert.That(exception.ParamName, Is.EqualTo(nameof(foo)));
    }

    [Test]
    public void ThrowIfNullOrEmpty_ShouldThrowIfInputIsNullOrEmptyString()
    {
      string? nullString = null;
      string emptyString = string.Empty;
      string someString = "foo";

      Assert.That(someString.ThrowIfNull(nameof(someString)), Is.SameAs(someString));

      var exception = Assert.Throws<ArgumentNullException>(() => nullString.ThrowIfNullOrEmpty(nameof(nullString)));
      Assert.That(exception.ParamName, Is.EqualTo(nameof(nullString)));

      exception = Assert.Throws<ArgumentNullException>(() => emptyString.ThrowIfNullOrEmpty(nameof(emptyString)));
      Assert.That(exception.ParamName, Is.EqualTo(nameof(emptyString)));
    }

    [Test]
    public void ThrowIfNullOrEmpty_ShouldThrowSpecifiedExceptionIfInputIsNullOrEmptyString()
    {
      string? nullString = null;
      string emptyString = string.Empty;
      string someString = "foo";
      var expectedException = new ArgumentNullException("null or empty");

      Assert.That(someString.ThrowIfNull(nameof(someString)), Is.SameAs(someString));

      var exception = Assert.Throws<ArgumentNullException>(() => nullString.ThrowIfNullOrEmpty(expectedException));
      Assert.That(exception, Is.SameAs(expectedException));

      exception = Assert.Throws<ArgumentNullException>(() => emptyString.ThrowIfNullOrEmpty(expectedException));
      Assert.That(exception, Is.SameAs(expectedException));
    }

    [Test]
    public void ThrowIfNullOrWhitespace_ShouldThrowIfInputIsNullOrEmptyString()
    {
      string? nullString = null;
      string emptyString = string.Empty;
      string whitespaceString = "\n";
      string someString = "foo";

      Assert.That(someString.ThrowIfNull(nameof(someString)), Is.SameAs(someString));

      var exception =
        Assert.Throws<ArgumentNullException>(() => nullString.ThrowIfNullOrWhiteSpace(nameof(nullString)));
      Assert.That(exception.ParamName, Is.EqualTo(nameof(nullString)));

      exception = Assert.Throws<ArgumentNullException>(() => emptyString.ThrowIfNullOrWhiteSpace(nameof(emptyString)));
      Assert.That(exception.ParamName, Is.EqualTo(nameof(emptyString)));

      exception = Assert.Throws<ArgumentNullException>(() =>
        whitespaceString.ThrowIfNullOrWhiteSpace(nameof(whitespaceString)));
      Assert.That(exception.ParamName, Is.EqualTo(nameof(whitespaceString)));
    }

    [Test]
    public void ThrowIfNullOrWhiteSpace_ShouldThrowSpecifiedExceptionIfInputIsNullOrEmptyOrWhitespaceString()
    {
      string? nullString = null;
      string emptyString = string.Empty;
      string whitespaceString = "\n";
      string someString = "foo";
      var expectedException = new ArgumentNullException("null or empty");

      Assert.That(someString.ThrowIfNull(nameof(someString)), Is.SameAs(someString));

      var exception = Assert.Throws<ArgumentNullException>(() => nullString.ThrowIfNullOrWhiteSpace(expectedException));
      Assert.That(exception, Is.SameAs(expectedException));

      exception = Assert.Throws<ArgumentNullException>(() => emptyString.ThrowIfNullOrWhiteSpace(expectedException));
      Assert.That(exception, Is.SameAs(expectedException));

      exception = Assert.Throws<ArgumentNullException>(
        () => whitespaceString.ThrowIfNullOrWhiteSpace(expectedException));
      Assert.That(exception, Is.SameAs(expectedException));
    }

    [Test]
    public void ThrowIfNull_WhenInputIsNull_ShouldThrowSpecifiedException()
    {
      Foo? fooNull = null;
      var foo = new Foo(123);
      var expectedException = new ArgumentNullException("null or empty");

      Assert.That(foo.ThrowIfNull(nameof(foo)), Is.SameAs(foo));

      var exception = Assert.Throws<ArgumentNullException>(() => fooNull.ThrowIfNull(expectedException));
      Assert.That(exception, Is.SameAs(expectedException));
    }
  }
}