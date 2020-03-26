using NUnit.Framework;
using ViennaNET.Specifications.Implementations;

namespace ViennaNET.Specifications.Tests
{
  [TestFixture(Category = "Unit")]
  public class AndSpecificationTest
  {
    private TestObjects.CityMemberSpecification _spec1;
    private TestObjects.AgeMemberSpecification _spec2;
    private TestObjects.Member member;

    [OneTimeSetUp]
    public void InitTestData()
    {
      _spec1 = new TestObjects.CityMemberSpecification("Omsk");
      _spec2 = new TestObjects.AgeMemberSpecification(42);

      member = new TestObjects.Member { City = "Omsk", Age = 42, Name = "Ivan" };
    }

    [Test]
    public void AndSpecification_2spec_Test()
    {
      var andSpec = new AndSpecification<TestObjects.Member>(_spec1, _spec2);
      member.Age = 42;

      Assert.That(andSpec.IsSatisfiedBy(member));
    }

    [Test]
    public void AndSpecification_2specOps_Test()
    {
      var andSpec = _spec1 & _spec2;
      member.Age = 42;

      Assert.That(andSpec.IsSatisfiedBy(member));
    }

    [Test]
    public void AndSpecification_1wrong_Test()
    {
      var andSpec = new AndSpecification<TestObjects.Member>(_spec1, _spec2);

      member.Age = 43;

      Assert.That(!andSpec.IsSatisfiedBy(member));
    }
  }
}
