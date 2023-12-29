using ViennaNET.Orm.Application;

namespace ViennaNET.Orm.Tests.Unit.DSL
{
    internal class TestIntegrationEvent : IIntegrationEvent
  {
    public int Id => throw new NotImplementedException();

    public int Type => throw new NotImplementedException();

    public DateTime Timestamp => throw new NotImplementedException();

    public string Initiator => throw new NotImplementedException();

    public string Body => throw new NotImplementedException();

    public bool IsSendable => throw new NotImplementedException();
  }
}