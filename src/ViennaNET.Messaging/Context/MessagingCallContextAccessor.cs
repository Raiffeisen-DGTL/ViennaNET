using Microsoft.Extensions.Logging;
using ViennaNET.CallContext;

namespace ViennaNET.Messaging.Context
{
    public class MessagingCallContextAccessor : IMessagingCallContextAccessor
    {
        private static readonly AsyncLocal<CallContextHolder> messagingContextCurrent = new();

        public void SetContext(ICallContext callContext)
        {
            var holder = messagingContextCurrent.Value;
            if (holder != null)
            {
                holder.Context = null;
            }

            if (callContext != null)
            {
                messagingContextCurrent.Value = new CallContextHolder
                {
                    Context = callContext,
                };
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
            public ICallContext? Context;
        }
    }
}