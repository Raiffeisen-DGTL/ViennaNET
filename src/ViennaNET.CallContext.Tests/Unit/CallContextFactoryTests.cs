using Moq;
using NUnit.Framework;

namespace ViennaNET.CallContext.Tests.Unit
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(CallContextFactory))]
  public class CallContextFactoryTests
  {
    [Test]
    public void Create_NoAccessors_EmptyCallContext()
    {
      // arrange
      var accessors = new ICallContextAccessor[0];

      // act
      var factory = new CallContextFactory(accessors);
      var context = factory.Create();

      // assert
      Assert.That(context, Is.TypeOf<EmptyCallContext>());
    }

    [Test]
    public void Create_HasAccessor_AccessorCallContext()
    {
      // arrange
      var fakeContext = new Mock<ICallContext>();
      var fakeAccessor = new Mock<ICallContextAccessor>();
      fakeAccessor.Setup(x => x.GetContext())
                  .Returns(fakeContext.Object);
      var accessors = new[] { fakeAccessor.Object };

      // act
      var factory = new CallContextFactory(accessors);
      var context = factory.Create();

      // assert
      Assert.That(context, Is.EqualTo(fakeContext.Object));
    }

    [Test]
    public void Create_HasManyAccessors_ReturnsExistedCallContext()
    {
      // arrange
      var fakeAccessor1 = new Mock<ICallContextAccessor>();
      fakeAccessor1.Setup(x => x.GetContext())
                   .Returns((ICallContext)null);

      var fakeContext = new Mock<ICallContext>();
      var fakeAccessor2 = new Mock<ICallContextAccessor>();
      fakeAccessor1.Setup(x => x.GetContext())
                   .Returns(fakeContext.Object);

      var accessors = new[] { fakeAccessor1.Object, fakeAccessor2.Object };

      // act
      var factory = new CallContextFactory(accessors);
      var context = factory.Create();

      // assert
      Assert.That(context, Is.EqualTo(fakeContext.Object));
    }
  }
}
