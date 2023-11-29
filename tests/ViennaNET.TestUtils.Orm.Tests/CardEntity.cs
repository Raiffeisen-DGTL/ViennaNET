using ViennaNET.Orm.Seedwork;

namespace ViennaNET.TestUtils.Orm.Tests
{
  public class CardEntity : IEntityKey<int>
  {
    public CardEntity(int id, string pan)
    {
      Id = id;
      Pan = pan;
    }

    public string Pan { get; }

    public int Id { get; }
  }
}