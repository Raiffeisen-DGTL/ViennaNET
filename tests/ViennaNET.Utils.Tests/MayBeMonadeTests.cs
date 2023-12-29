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
            const int structValue = 1;
            const string message = "error message";

            var objValue = new { a = 1 };
            object? objNull = null;
            int? structNull = null;

            Assert.Multiple(() =>
            {
                Assert.That(objValue.ThrowIf(_ => false, message), Is.EqualTo(objValue));
                Assert.That(objValue.ThrowIf(_ => false, new Exception(message)), Is.EqualTo(objValue));
                Assert.That(objValue.ThrowIf(_ => false, a => new Exception($"{message}:{a}")), Is.EqualTo(objValue));
                Assert.That(objNull.ThrowIf(_ => false, message), Is.EqualTo(objNull));
                Assert.That(objNull.ThrowIf(_ => false, new Exception(message)), Is.EqualTo(objNull));
                Assert.That(structValue.ThrowIf(_ => false, message), Is.EqualTo(structValue));
                Assert.That(structNull.ThrowIf(_ => false, message), Is.EqualTo(structNull));
            });
        }

        [Test]
        public void ThrowIf_WhenPredicateIsTrue_ShouldThrowException()
        {
            const string message = "error message";
            const int structValue = 1;
            var objValue = new { a = 1 };
            object? objNull = null;
            int? structNull = null;

            Assert.Multiple(() =>
            {
                Assert.That(objValue, Is.EqualTo(objValue.ThrowIf(_ => false, message)));
                Assert.That(() => objValue.ThrowIf(_ => true, message), Throws.Exception);
                Assert.That(() => objValue.ThrowIf(_ => true, new Exception(message)), Throws.Exception);
                Assert.That(() => objValue.ThrowIf(_ => true, a => new Exception($"{message}: {a}")), Throws.Exception);
                Assert.That(() => structValue.ThrowIf(_ => true, "error message"), Throws.Exception);
                Assert.That(objNull, Is.EqualTo(objNull.ThrowIf(_ => true, message)));
                Assert.That(objNull, Is.EqualTo(objNull.ThrowIf(_ => true, new Exception(message))));
                Assert.That(objNull, Is.EqualTo(objNull.ThrowIf(_ => true, a => new Exception($"{message}: {a}"))));
                Assert.That(structNull, Is.EqualTo(structNull.ThrowIf(_ => false, message)));
            });
        }

        [Test]
        public void ThrowIf_WhenPredicateIsTrue_ButTargetIsNull_ShouldReturnNull()
        {
            const string message = "error message";
            object? objNull = null;
            int? structNull = null;

            Assert.Multiple(() =>
            {
                Assert.That(objNull.ThrowIf(_ => true, message), Is.Null);
                Assert.That(objNull.ThrowIf(_ => true, new Exception(message)), Is.Null);
                Assert.That(objNull.ThrowIf(_ => true, a => new Exception($"{message}: {a}")), Is.Null);
                Assert.That(structNull.ThrowIf(_ => false, message), Is.Null);
            });
        }

        [Test]
        public void ThrowIfNot_WhenPredicateIsTrue_ShouldReturnValue()
        {
            const string message = "error message";
            var objValue = new { a = 1 };
            object? objNull = null;

            Assert.Multiple(() =>
            {
                Assert.That(objValue.ThrowIfNot(_ => true, new Exception(message)), Is.EqualTo(objValue));
                Assert.That(objValue.ThrowIfNot(_ => true, a => new Exception($"{message}:{a}")), Is.EqualTo(objValue));
                Assert.That(objNull.ThrowIfNot(_ => true, new Exception(message)), Is.EqualTo(objNull));
                Assert.That(objNull.ThrowIfNot(_ => true, a => new Exception($"{message}:{a}")), Is.EqualTo(objNull));
            });
        }

        [Test]
        public void ThrowIfNot_WhenPredicateIsFalse_ShouldThrowException()
        {
            const string message = "error message";
            var objValue = new { a = 1 };

            Assert.Multiple(() =>
            {
                Assert.That(() => objValue.ThrowIfNot(_ => false, new Exception(message)), Throws.Exception);
                Assert.That(
                    () => objValue.ThrowIfNot(_ => false, a => new Exception($"{message}:{a}")),
                    Throws.Exception);
            });
        }

        [Test]
        public void ThrowIfNot_WhenPredicateIsFalse_ButTargetIsNull_ShouldReturnNull()
        {
            const string message = "error message";
            object? objNull = null;

            Assert.Multiple(() =>
            {
                Assert.That(objNull.ThrowIfNot(_ => false, new Exception(message)), Is.Null);
                Assert.That(objNull.ThrowIfNot(_ => false, a => new Exception($"{message}:{a}")), Is.Null);
            });
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

            var result = obj.Do(_ => called = true);
            var resultStruct = objStruct.Do(_ => called = true);

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

            Assert.Multiple(() =>
            {
                Assert.That(fooNull.If(f => f.Value == 0x404), Is.Null);
                Assert.That(foo.If(f => f.Value == 0x404), Is.Null);
                Assert.That(foo.If(f => f.Value == 123), Is.SameAs(foo));
            });
        }

        [Test]
        public void IfNot_Object_ShouldReturnCorrectResult()
        {
            Foo? fooNull = null;
            Foo foo = new(0x404);

            Assert.Multiple(() =>
            {
                Assert.That(fooNull.IfNot(f => f.Value == 0x404), Is.Null);
                Assert.That(foo.IfNot(f => f.Value == 0x404), Is.Null);
                Assert.That(foo.IfNot(f => f.Value == 123), Is.SameAs(foo));
            });
        }

        [Test]
        public void If_Struct_ShouldReturnCorrectResult()
        {
            int? valueNull = null;
            int? value = 123;

            Assert.Multiple(() =>
            {
                Assert.That(valueNull.If(_ => false), Is.Null);
                Assert.That(value.If(_ => false), Is.Null);
                Assert.That(value.If(_ => true), Is.EqualTo(value));
            });
        }

        [Test]
        public void IfNot_Struct_ShouldReturnCorrectResult()
        {
            int? valueNull = null;
            int? value = 123;

            Assert.Multiple(() =>
            {
                Assert.That(valueNull.IfNot(_ => true), Is.Null);
                Assert.That(value.IfNot(_ => true), Is.Null);
                Assert.That(value.IfNot(_ => false), Is.EqualTo(value));
            });
        }

        [Test]
        public void Return_ShouldReturnInputEvaluationIfItsNotNull()
        {
            var result = new object();
            Foo? fooNull = null;
            var foo = new Foo(123);
            int? valueNull = null;
            int? value = 123;

            Assert.Multiple(() =>
            {
                Assert.That(valueNull.Return(_ => result), Is.Null);
                Assert.That(fooNull.Return(_ => result), Is.Null);
                Assert.That(foo.Return(_ => result), Is.EqualTo(result));
                Assert.That(value.Return(_ => result), Is.EqualTo(result));
            });
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

            Assert.Multiple(() =>
            {
                Assert.That(foo.ThrowIfNull(nameof(foo)), Is.SameAs(foo));
                Assert.That(() => fooNull.ThrowIfNull(nameof(foo)),
                    Throws.ArgumentNullException.And
                        .Property(nameof(ArgumentNullException.ParamName))
                        .EqualTo(nameof(foo)));
            });
        }

        [Test]
        public void ThrowIfNullOrEmpty_ShouldThrowIfInputIsNullOrEmptyString()
        {
            const string someString = "foo";
            string? nullString = null;
            var emptyString = string.Empty;

            Assert.Multiple(() =>
            {
                Assert.That(someString.ThrowIfNull(nameof(someString)), Is.SameAs(someString));
                Assert.That(() => nullString.ThrowIfNullOrEmpty(nameof(nullString)),
                    Throws.ArgumentNullException.And
                        .Property(nameof(ArgumentNullException.ParamName))
                        .EqualTo(nameof(nullString)));

                Assert.That(() => emptyString.ThrowIfNullOrEmpty(nameof(emptyString)),
                    Throws.ArgumentNullException.And
                        .Property(nameof(ArgumentNullException.ParamName))
                        .EqualTo(nameof(emptyString)));
            });
        }

        [Test]
        public void ThrowIfNullOrEmpty_ShouldThrowSpecifiedExceptionIfInputIsNullOrEmptyString()
        {
            const string someString = "foo";
            const string message = "null or empty";
            string? nullString = null;
            var emptyString = string.Empty;

            Assert.Multiple(() =>
            {
                Assert.That(someString.ThrowIfNull(nameof(someString)), Is.SameAs(someString));
                Assert.That(() => nullString.ThrowIfNullOrEmpty(message),
                    Throws.ArgumentNullException.And.Message.Contains(message));
                Assert.That(() => emptyString.ThrowIfNullOrEmpty(message),
                    Throws.ArgumentNullException.And.Message.Contains(message));
            });
        }

        [Test]
        public void ThrowIfNullOrWhitespace_ShouldThrowIfInputIsNullOrEmptyString()
        {
            const string whitespace = "\n";
            const string someString = "foo";
            string? nullString = null;
            var emptyString = string.Empty;

            Assert.Multiple(() =>
            {
                Assert.That(someString.ThrowIfNull(nameof(someString)), Is.SameAs(someString));
                Assert.That(() => nullString.ThrowIfNullOrWhiteSpace(nameof(nullString)),
                    Throws.ArgumentNullException.And
                        .Property(nameof(ArgumentNullException.ParamName))
                        .EqualTo(nameof(nullString)));
                Assert.That(() => emptyString.ThrowIfNullOrWhiteSpace(nameof(emptyString)),
                    Throws.ArgumentNullException.And
                        .Property(nameof(ArgumentNullException.ParamName))
                        .EqualTo(nameof(emptyString)));
                Assert.That(() => whitespace.ThrowIfNullOrWhiteSpace(nameof(whitespace)),
                    Throws.ArgumentNullException.And
                        .Property(nameof(ArgumentNullException.ParamName))
                        .EqualTo(nameof(whitespace)));
            });
        }

        [Test]
        public void ThrowIfNullOrWhiteSpace_ShouldThrowSpecifiedExceptionIfInputIsNullOrEmptyOrWhitespaceString()
        {
            const string whitespaceString = "\n";
            const string someString = "foo";
            const string message = "null or empty";
            string? nullString = null;
            var emptyString = string.Empty;


            Assert.Multiple(() =>
            {
                Assert.That(someString.ThrowIfNull(nameof(someString)), Is.SameAs(someString));
                Assert.That(() => nullString.ThrowIfNullOrWhiteSpace(message),
                    Throws.ArgumentNullException.And.Message.Contains(message));
                Assert.That(() => emptyString.ThrowIfNullOrWhiteSpace(message),
                    Throws.ArgumentNullException.And.Message.Contains(message));
                Assert.That(() => whitespaceString.ThrowIfNullOrWhiteSpace(message),
                    Throws.ArgumentNullException.And.Message.Contains(message));
            });
        }

        [Test]
        public void ThrowIfNull_WhenInputIsNull_ShouldThrowSpecifiedException()
        {
            const string message = "null or empty";
            Foo? fooNull = null;
            var foo = new Foo(123);

            Assert.Multiple(() =>
            {
                Assert.That(foo.ThrowIfNull(nameof(foo)), Is.SameAs(foo));
                Assert.That(() => fooNull.ThrowIfNull(message),
                    Throws.ArgumentNullException.And.Message.Contains(message));
            });
        }
    }
}