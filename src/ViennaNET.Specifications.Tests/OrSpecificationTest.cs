using NUnit.Framework;
using NUnit.Framework.Internal;
using ViennaNET.Specifications.Implementations;

namespace ViennaNET.Specifications.Tests
{
  [TestFixture(Category = "Unit")]
  public class OrSpecificationTest
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
    public void OrSpecification_AgeWrong_Test()
    {
      var orSpec = new OrSpecification<TestObjects.Member>(_spec1, _spec2);

      member.Age = 42;

      Assert.That(orSpec.IsSatisfiedBy(member));
    }

    [Test]
    public void OrSpecification_AgeWrongOper_Test()
    {
      var orSpec = _spec1 | _spec2;

      member.Age = 42;

      Assert.That(orSpec.IsSatisfiedBy(member));
    }

    [Test]
    public void OrSpecification_CityWrong_Test()
    {
      var orSpec = new OrSpecification<TestObjects.Member>(_spec1, _spec2);

      member.Age = 42;
      member.City = "Moscow";

      Assert.That(orSpec.IsSatisfiedBy(member));
    }

    [Test]
    public void OrSpecification_2Wrong_Test()
    {
      var orSpec = new OrSpecification<TestObjects.Member>(_spec1, _spec2);

      member.Age = 43;
      member.City = "Moscow";

      Assert.That(!orSpec.IsSatisfiedBy(member));
    }
  }
}
