using NUnit.Framework;

namespace ViennaNET.Utils.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(ResultOf<>))]
  public class ResultOfTests
  {
    private class FromTestClass
    {
      public string FromField { get; set; }
    }

    private class ToTestClass
    {
      public string ToField { get; set; }
    }

    [Test]
    public void CreateSuccess_IsOk()
    {
      var actual = ResultOf<FromTestClass>.CreateSuccess(new FromTestClass { FromField = "Test!" });

      Assert.AreEqual(ResultState.Success, actual.State);
      Assert.IsNull(actual.InvalidMessage);
      Assert.AreEqual("Test!", actual.Result.FromField);
    }

    [Test]
    public void CreateEmpty_IsOk()
    {
      var actual = ResultOf<FromTestClass>.CreateEmpty();

      Assert.AreEqual(ResultState.Empty, actual.State);
      Assert.IsNull(actual.InvalidMessage);
      Assert.IsNull(actual.Result);
    }

    [Test]
    public void CreateInvalid_IsOk()
    {
      var actual = ResultOf<FromTestClass>.CreateInvalid("Message!");

      Assert.AreEqual(ResultState.Invalid, actual.State);
      Assert.AreEqual("Message!", actual.InvalidMessage);
      Assert.IsNull(actual.Result);
    }

    [Test]
    public void CloneFailedAs_CreateEmpty_IsOk()
    {
      var from = ResultOf<FromTestClass>.CreateEmpty();
      var actual = from.CloneFailedAs<ToTestClass>();

      Assert.AreEqual(ResultState.Empty, actual.State);
      Assert.IsNull(actual.InvalidMessage);
      Assert.IsNull(actual.Result);
      Assert.AreEqual(typeof(ResultOf<ToTestClass>), actual.GetType());
    }

    [Test]
    public void CloneFailedAs_CreateInvalid_IsOk()
    {
      var from = ResultOf<FromTestClass>.CreateInvalid("Message!");
      var actual = from.CloneFailedAs<ToTestClass>();

      Assert.AreEqual(ResultState.Invalid, actual.State);
      Assert.AreEqual("Message!", actual.InvalidMessage);
      Assert.IsNull(actual.Result);
      Assert.AreEqual(typeof(ResultOf<ToTestClass>), actual.GetType());
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
      Assert.IsTrue(actual1);
      Assert.IsTrue(actual2);
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
      Assert.IsTrue(actual1);
      Assert.IsTrue(actual2);
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
      Assert.IsTrue(actual1);
      Assert.IsTrue(actual2);
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
      Assert.IsFalse(actual1);
      Assert.IsFalse(actual2);
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
      Assert.IsFalse(actual1);
      Assert.IsFalse(actual2);
    }
  }
}