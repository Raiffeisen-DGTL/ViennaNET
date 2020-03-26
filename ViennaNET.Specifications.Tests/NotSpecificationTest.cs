using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ViennaNET.Specifications.Implementations;

namespace ViennaNET.Specifications.Tests
{
  [TestFixture(Category = "Unit")]
  public class NotSpecificationTest
  {
    private Specification<TestObjects.Member> spec;
    private TestObjects.Member member;

    [SetUp]
    public void InitData()
    {
      spec = new TestObjects.CityMemberSpecification("Moscow");
      member = new TestObjects.Member { City = "Omsk", Age = 42, Name = "Ivan" };
    }

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
