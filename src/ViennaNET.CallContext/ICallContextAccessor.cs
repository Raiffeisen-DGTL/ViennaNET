namespace ViennaNET.CallContext
{
  public interface ICallContextAccessor
  {
    void SetContext(ICallContext callContext);

    void CleanContext();

    ICallContext GetContext();
  }
}
