using ViennaNET.Orm.Seedwork;

namespace OrmService.Entities
{
  public class Greeting : IEntityKey<int>
  {
    public virtual int Id { get; protected set; }
    public virtual string Value { get; protected set; }

    public static Greeting Create(string value)
    {
      return new Greeting()
      {
        Value = value
      };
    }
  }
}
