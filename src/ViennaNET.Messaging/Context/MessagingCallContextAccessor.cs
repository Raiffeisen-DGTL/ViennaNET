using System.Threading;
using ViennaNET.CallContext;
using ViennaNET.Logging;

namespace ViennaNET.Messaging.Context
{
  public class MessagingCallContextAccessor : IMessagingCallContextAccessor
  {
    private static readonly AsyncLocal<CallContextHolder> messagingContextCurrent = new AsyncLocal<CallContextHolder>();

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

        Logger.RequestId = callContext.RequestId;
        Logger.User = callContext.UserId;
      }
    }

    public void CleanContext()
    {
      var holder = messagingContextCurrent.Value;
      if (holder != null)
      {
        holder.Context = null;

        Logger.ClearRequestId();
        Logger.ClearUser();
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
