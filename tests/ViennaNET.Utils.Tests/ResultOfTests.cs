using Moq;
using NUnit.Framework;

namespace ViennaNET.Utils.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(ResultOf<>))]
    public class ResultOfTests
    {
        private class FromTestClass
        {
            public string? FromField { get; set; }
        }

        private class ToTestClass
        {
            public string? ToField { get; set; }
        }

        [Test]
        public void CreateSuccess_IsOk()
        {
            const string formField = "Test!";
            var actual = ResultOf<FromTestClass>.CreateSuccess(new FromTestClass { FromField = formField });

            Assert.Multiple(() =>
            {
                Assert.That(actual.State, Is.EqualTo(ResultState.Success));
                Assert.That(actual.Result?.FromField, Is.EqualTo(formField));
                Assert.That(actual.InvalidMessage, Is.Null);
            });
        }

        [Test]
        public void CreateEmpty_IsOk()
        {
            var actual = ResultOf<FromTestClass>.CreateEmpty();

            Assert.Multiple(() =>
            {
                Assert.That(actual.State, Is.EqualTo(ResultState.Empty));
                Assert.That(actual.InvalidMessage, Is.Null);
                Assert.That(actual.Result, Is.Null);
            });
        }

        [Test]
        public void CreateInvalid_IsOk()
        {
            const string message = "Message!";

            var actual = ResultOf<FromTestClass>.CreateInvalid(message);

            Assert.Multiple(() =>
            {
                Assert.That(actual.State, Is.EqualTo(ResultState.Invalid));
                Assert.That(actual.InvalidMessage, Is.EqualTo(message));
                Assert.That(actual.Result, Is.Null);
            });
        }

        [Test]
        public void CloneFailedAs_CreateEmpty_IsOk()
        {
            var from = ResultOf<FromTestClass>.CreateEmpty();
            var actual = from.CloneFailedAs<ToTestClass>();

            Assert.Multiple(() =>
            {
                Assert.That(actual.State, Is.EqualTo(ResultState.Empty));
                Assert.That(actual.InvalidMessage, Is.Null);
                Assert.That(actual.Result, Is.Null);
                Assert.That(actual, Is.InstanceOf<ResultOf<ToTestClass>>());
            });
        }

        [Test]
        public void CloneFailedAs_CreateInvalid_IsOk()
        {
            const string message = "Message!";

            var from = ResultOf<FromTestClass>.CreateInvalid(message);
            var actual = from.CloneFailedAs<ToTestClass>();

            Assert.Multiple(() =>
            {
                Assert.That(actual.State, Is.EqualTo(ResultState.Invalid));
                Assert.That(actual.InvalidMessage, Is.EqualTo(message));
                Assert.That(actual.Result, Is.Null);
                Assert.That(actual, Is.InstanceOf<ResultOf<ToTestClass>>());
            });
        }

        [Test]
        public void Equals_Success_IsOk_Test()
        {
            // arrange
            var obj = new FromTestClass();
            var result1 = ResultOf<FromTestClass>.CreateSuccess(obj);
            var result2 = ResultOf<FromTestClass>.CreateSuccess(obj);

            // act
            var actual1 = result1.Equals(result2);
            var actual2 = result2.Equals(result1);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(actual1, Is.True);
                Assert.That(actual2, Is.True);
            });
        }

        [Test]
        public void Equals_Invalid_IsOk_Test()
        {
            // arrange
            var result1 = ResultOf<FromTestClass>.CreateInvalid("Test");
            var result2 = ResultOf<FromTestClass>.CreateInvalid("Test");

            // act
            var actual1 = result1.Equals(result2);
            var actual2 = result2.Equals(result1);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(actual1, Is.True);
                Assert.That(actual2, Is.True);
            });
        }

        [Test]
        public void Equals_Empty_IsOk_Test()
        {
            // arrange
            var result1 = ResultOf<FromTestClass>.CreateEmpty();
            var result2 = ResultOf<FromTestClass>.CreateEmpty();

            // act
            var actual1 = result1.Equals(result2);
            var actual2 = result2.Equals(result1);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(actual1, Is.True);
                Assert.That(actual2, Is.True);
            });
        }

        [Test]
        public void Equals_Success_IsFail_Test()
        {
            // arrange
            var result1 = ResultOf<FromTestClass>.CreateSuccess(new FromTestClass());
            var result2 = ResultOf<FromTestClass>.CreateSuccess(new FromTestClass());

            // act
            var actual1 = result1.Equals(result2);
            var actual2 = result2.Equals(result1);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(actual1, Is.False);
                Assert.That(actual2, Is.False);
            });
        }

        [Test]
        public void Equals_Invalid_IsFail_Test()
        {
            // arrange
            var result1 = ResultOf<FromTestClass>.CreateInvalid("Test1");
            var result2 = ResultOf<FromTestClass>.CreateInvalid("Test2");

            // act
            var actual1 = result1.Equals(result2);
            var actual2 = result2.Equals(result1);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(actual1, Is.False);
                Assert.That(actual2, Is.False);
            });
        }
    }
}