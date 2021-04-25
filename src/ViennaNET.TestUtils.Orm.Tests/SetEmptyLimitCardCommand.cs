using System.Collections.Generic;
using ViennaNET.Orm.Repositories;

namespace ViennaNET.TestUtils.Orm.Tests
{
  public class SetEmptyLimitCardCommand : BaseCommand
  {
    public SetEmptyLimitCardCommand(CardEntity card)
    {
      Sql = "UPDATE cards SET limit = 0 WHERE card_id = :card_id";

      Parameters = new Dictionary<string, TypeWrapper> { { "card_id", TypeWrapper.Create(card, c => c.Id) } };
    }
  }
}