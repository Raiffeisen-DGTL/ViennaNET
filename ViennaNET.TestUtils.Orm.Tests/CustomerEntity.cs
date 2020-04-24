using ViennaNET.Orm.Seedwork;

namespace ViennaNET.TestUtils.Orm.Tests
{
  public class CustomerEntity : IEntityKey<string>
  {
    public string CNum => Id;
    public string Id { get; }
  }
}