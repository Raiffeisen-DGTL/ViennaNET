using System;
using System.Collections.Generic;
using ViennaNET.Orm.Repositories;
using ViennaNET.Utils;

namespace ViennaNET.TestUtils.Orm.Tests
{
  public class GetCustomerCardsCustomQuery : BaseQuery<CardEntity>
  {
    public GetCustomerCardsCustomQuery(CustomerEntity customer)
    {
      Sql = "select * from cards where cnum = :cnum";

      Parameters = new Dictionary<string, object> { { "cnum", TypeWrapper.Create(customer, c => c.CNum) } };
    }

    protected override CardEntity TransformTuple(object[] tuple, string[] aliases)
    {
      return new CardEntity(tuple[0].Return(Convert.ToInt32), tuple[1]?.ToString());
    }
  }
}