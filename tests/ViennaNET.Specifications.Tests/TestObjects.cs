using System;
using System.Linq.Expressions;
using ViennaNET.Specifications.Implementations;

namespace ViennaNET.Specifications.Tests
{
  internal class TestObjects
  {
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

    public class AgeMemberSpecification : Specification<Member>
    {
      private readonly int _age;

      public AgeMemberSpecification(int age)
      {
        _age = age;

        expression = member => member.Age == _age;
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