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
      var objValue = new {a = 1};
      object? objNull = null;
      var structValue = 1;
      int? structNull = null;

      Assert.AreEqual(objValue, objValue.ThrowIf(a => false, "error message"));
      Assert.AreEqual(objValue, objValue.ThrowIf(a => false, new Exception("error message")));
      Assert.AreEqual(objValue, objValue.ThrowIf(a => false, a => new Exception($"error message: {a}")));

      Assert.AreEqual(objNull, objNull.ThrowIf(a => false, "error message"));
      Assert.AreEqual(objNull, objNull.ThrowIf(a => false, new Exception("error message")));
      Assert.AreEqual(objNull, objNull.ThrowIf(a => false, a => new Exception($"error message: {a}")));

      Assert.AreEqual(structValue, structValue.ThrowIf(a => false, "error message"));
      Assert.AreEqual(structNull, structNull.ThrowIf(a => false, "error message"));
    }

    [Test]
    public void ThrowIf_WhenPredicateIsTrue_ShouldThrowException()
    {
      var objValue = new {a = 1};
      object? objNull = null;
      var structValue = 1;
      int? structNull = null;

      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIf(a => true, "error message");
      });
      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIf(a => true, new Exception("error message"));
      });
      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIf(a => true, a => new Exception($"error message: {a}"));
      });

      Assert.AreEqual(objNull, objNull.ThrowIf(a => true, "error message"));
      Assert.AreEqual(objNull, objNull.ThrowIf(a => true, new Exception("error message")));
      Assert.AreEqual(objNull, objNull.ThrowIf(a => true, a => new Exception($"error message: {a}")));

      Assert.Throws<Exception>(() =>
      {
        var _ = structValue.ThrowIf(a => true, "error message");
      });
      Assert.AreEqual(structNull, structNull.ThrowIf(a => false, "error message"));
    }

    [Test]
    public void ThrowIf_WhenPredicateIsTrue_ButTargetIsNull_ShouldReturnNull()
    {
      object? objNull = null;
      int? structNull = null;

      Assert.IsNull(objNull.ThrowIf(a => true, "error message"));
      Assert.IsNull(objNull.ThrowIf(a => true, new Exception("error message")));
      Assert.IsNull(objNull.ThrowIf(a => true, a => new Exception($"error message: {a}")));

      Assert.IsNull(structNull.ThrowIf(a => false, "error message"));
    }

    [Test]
    public void ThrowIfNot_WhenPredicateIsTrue_ShouldReturnValue()
    {
      var objValue = new {a = 1};
      object? objNull = null;

      Assert.AreEqual(objValue, objValue.ThrowIfNot(a => true, new Exception("error message")));
      Assert.AreEqual(objValue, objValue.ThrowIfNot(a => true, a => new Exception($"error message: {a}")));

      Assert.AreEqual(objNull, objNull.ThrowIfNot(a => true, new Exception("error message")));
      Assert.AreEqual(objNull, objNull.ThrowIfNot(a => true, a => new Exception($"error message: {a}")));
    }

    [Test]
    public void ThrowIfNot_WhenPredicateIsFalse_ShouldThrowException()
    {
      var objValue = new {a = 1};

      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIfNot(a => false, new Exception("error message"));
      });
      Assert.Throws<Exception>(() =>
      {
        var _ = objValue.ThrowIfNot(a => false, a => new Exception($"error message: {a}"));
      });
    }

    [Test]
    public void ThrowIfNot_WhenPredicateIsFalse_ButTargetIsNull_ShouldReturnNull()
    {
      object? objNull = null;

      Assert.IsNull(objNull.ThrowIfNot(a => false, new Exception("error message")));
      Assert.IsNull(objNull.ThrowIfNot(a => false, a => new Exception($"error message: {a}")));
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
      var collection = new[] {new Foo(1), new Foo(2), new Foo(3)};
      
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
      var collection = new[] {new Foo(1), new Foo(2), new Foo(3)};
      
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
      
      Assert.That(called, Is.False);
      Assert.That(result, Is.Null);
      Assert.That(resultStruct, Is.Null);
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
      Assert.That(result, Is.SameAs(obj));
      Assert.That(called, Is.True);

      called = false;
      var resultStruct = objStruct.Do(o =>
      {
        Assert.That(o, Is.EqualTo(objStruct));
        called = true;
      });
      Assert.That(resultStruct, Is.EqualTo(objStruct));
      Assert.That(called, Is.True);
    }

    [Test]
    public void If_Object_ShouldReturnCorrectResult()
    {
      Foo? fooNull = null;
      Foo foo = new Foo(123);
      
      Assert.IsNull(fooNull.If(f => f.Value == 0x404));
      Assert.IsNull(foo.If(f => f.Value == 0x404));
      
      Assert.That(foo.If(f => f.Value == 123), Is.SameAs(foo));
    }
    
    [Test]
    public void IfNot_Object_ShouldReturnCorrectResult()
    {
      Foo? fooNull = null;
      Foo foo = new Foo(0x404);
      
      Assert.IsNull(fooNull.IfNot(f => f.Value == 0x404));
      Assert.IsNull(foo.IfNot(f => f.Value == 0x404));
      
      Assert.That(foo.IfNot(f => f.Value == 123), Is.SameAs(foo));
    }
    
    [Test]
    public void If_Struct_ShouldReturnCorrectResult()
    {
      int? valueNull = null;
      int? value = 123;
      
      Assert.IsNull(valueNull.If(f => false));
      Assert.IsNull(value.If(f => false));
      
      Assert.That(value.If(f => true), Is.EqualTo(value));
    }
    
    [Test]
    public void IfNot_Struct_ShouldReturnCorrectResult()
    {
      int? valueNull = null;
      int? value = 123;
      
      Assert.IsNull(valueNull.IfNot(f => true));
      Assert.IsNull(value.IfNot(f => true));
      
      Assert.That(value.IfNot(f => false), Is.EqualTo(value));
    }

    [Test]
    public void Return_ShouldReturnInputIfItsNotNull()
    {
      var result = new object();
      Foo? fooNull = null;
      Foo? foo = new Foo(123);
      int? valueNull = null;
      int? value = 123;

      Assert.IsNull(valueNull.Return(v => result));
      Assert.IsNull(fooNull.Return(f => result));

      Assert.That(foo.Return(f => result), Is.EqualTo(result));
      Assert.That(value.Return(f => result), Is.EqualTo(result));
    }
  }
}