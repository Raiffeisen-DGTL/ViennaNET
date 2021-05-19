using Moq;
using NUnit.Framework;
using ViennaNET.CallContext;

namespace ViennaNET.WebApi.Configurators.CallContext.Tests.Unit
{
  [TestFixture(Category = "Unit")]
  [TestOf(typeof(HttpCallContextAccessor))]
  public class HttpCallContextAccessorTests
  {
    [Test]
    public void SetContext_ContextIsEmpty_SetNewContext()
    {
      // arrange
      var fakeContext = new Mock<ICallContext>();
      var accessor = new HttpCallContextAccessor();

      // act
      accessor.SetContext(fakeContext.Object);

      // assert
      Assert.That(accessor.GetContext(), Is.EqualTo(fakeContext.Object));
    }

    [Test]
    public void SetContext_ContextIsNotEmpty_ReSetNewContext()
    {
      // arrange
      var fakeContext1 = new Mock<ICallContext>();
      var fakeContext2 = new Mock<ICallContext>();

      var accessor = new HttpCallContextAccessor();
      accessor.SetContext(fakeContext1.Object);

      // act
      accessor.SetContext(fakeContext2.Object);

      // assert
      Assert.That(accessor.GetContext(), Is.EqualTo(fakeContext2.Object));
    }

    [Test]
    public void CleanContext_ContextIsNotEmpty_BecomeEmpty()
    {
      // arrange
      var accessor = new HttpCallContextAccessor();

      // act
      accessor.CleanContext();

      // assert
      Assert.That(accessor.GetContext(), Is.Null);
    }
  }
}
