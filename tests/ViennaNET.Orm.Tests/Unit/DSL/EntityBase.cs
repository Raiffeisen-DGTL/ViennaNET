using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm.Tests.Unit.DSL
{
    internal class EntityBase : IEntityKey<int>
  {
    public int Id => throw new NotImplementedException();
  }
}