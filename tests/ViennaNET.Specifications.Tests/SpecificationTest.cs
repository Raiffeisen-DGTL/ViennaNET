using System;
using System.Linq.Expressions;
using NUnit.Framework;
using ViennaNET.Specifications.Implementations;

namespace ViennaNET.Specifications.Tests.Unit
{
  [TestFixture(Category = "Unit")]
  public class SpecificationTests
  {
    [OneTimeSetUp]
    public void InitTestData()
    {
      _spec1 = new CityMemberSpecification("Omsk");
      member = new Member { City = "Omsk", Age = 42, Name = "Ivan" };
    }

    private CityMemberSpecification _spec1;
    private Member member;

    [Test]
    public void SpecificationTest()
    {
      Assert.That(_spec1.IsSatisfiedBy(member));
    }

    [Test]
    public void ExpressionTest()
    {
      var expression = _spec1.ToExpression();

      var func = expression.Compile();

      Assert.That(func(member));
    }

    public class CityMemberSpecification : Specification<Member>
    {
      private readonly string _city;

      public CityMemberSpecification(string city)
      {
        _city = city;

        expression = member => member.City == _city;
      }

      public override Expression<Func<Member, bool>> ToExpression()
      {
        return expression;
      }
    }

    public class Member
    {
      public string Name { get; set; }
      public string City { get; set; }
      public int Age { get; set; }
    }
  }
}