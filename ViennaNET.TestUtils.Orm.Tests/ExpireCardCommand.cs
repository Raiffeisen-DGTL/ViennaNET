using System.Collections.Generic;
using ViennaNET.Orm.Repositories;

namespace ViennaNET.TestUtils.Orm.Tests
{
  public class ExpireCardCommand : BaseCommand
  {
    public ExpireCardCommand(CardEntity card)
    {
      Sql = "UPDATE cards SET expired = true WHERE card_id = :card_id";

      Parameters = new Dictionary<string, TypeWrapper> {{"card_id", TypeWrapper.Create(card, c => c.Id)}};
    }
  }
}