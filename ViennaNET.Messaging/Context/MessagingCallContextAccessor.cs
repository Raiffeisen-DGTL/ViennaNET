using System.Threading;
using ViennaNET.CallContext;

namespace ViennaNET.Messaging.Context
{
  public class MessagingCallContextAccessor : IMessagingCallContextAccessor
  {
    private static AsyncLocal<CallContextHolder> messagingContextCurrent = new AsyncLocal<CallContextHolder>();

    public void SetContext(ICallContext callContext)
    {
      var holder = messagingContextCurrent.Value;
      if (holder != null)
      {
        holder.Context = null;
      }

      if (callContext != null)
      {
        messagingContextCurrent.Value = new CallContextHolder { Context = callContext };
      }
    }

    public void CleanContext()
    {
      var holder = messagingContextCurrent.Value;
      if (holder != null)
      {
        holder.Context = null;
      }
    }

    public ICallContext GetContext()
    {
      return messagingContextCurrent.Value?.Context;
    }

    private class CallContextHolder
    {
      public ICallContext Context;
    }
  }
}
