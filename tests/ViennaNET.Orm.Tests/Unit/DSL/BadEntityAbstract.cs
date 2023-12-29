using ViennaNET.Orm.Seedwork;

namespace ViennaNET.Orm.Tests.Unit.DSL
{
    internal abstract class BadEntityAbstract : IEntityKey<int>
  {
    public int Id => throw new NotImplementedException();
  }
}