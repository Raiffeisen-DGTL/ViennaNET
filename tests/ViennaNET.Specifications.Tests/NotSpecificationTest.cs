using NUnit.Framework;
using ViennaNET.Specifications.Implementations;

namespace ViennaNET.Specifications.Tests
{
  [TestFixture(Category = "Unit")]
  public class NotSpecificationTest
  {
    [SetUp]
    public void InitData()
    {
      spec = new TestObjects.CityMemberSpecification("Moscow");
      member = new TestObjects.Member { City = "Omsk", Age = 42, Name = "Ivan" };
    }

    private Specification<TestObjects.Member> spec;
    private TestObjects.Member member;

    [Test]
    public void NotSpecification_Test()
    {
      var notSpec = new NotSpecification<TestObjects.Member>(spec);

      Assert.That(notSpec.IsSatisfiedBy(member));
    }

    [Test]
    public void NotSpecification_Not_Test()
    {
      var notSpec = new NotSpecification<TestObjects.Member>(spec);

      member.City = "Moscow";

      Assert.That(!notSpec.IsSatisfiedBy(member));
    }
  }
}