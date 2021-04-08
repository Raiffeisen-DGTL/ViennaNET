using ViennaNET.CallContext;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class CallContextFactoryMock : ICallContextFactory
  {
    public ICallContext Create()
    {
      return new EmptyCallContext();
    }
  }
}