using System.Collections.Generic;

namespace ViennaNET.Mediator.DefaultConfiguration
{
  internal class MediatorRegistrar
  {
    public MediatorRegistrar(
      IMessageRecipientsRegistrar registrar, IEnumerable<IMessageHandler> messageRecipients,
      IEnumerable<IMessageHandlerAsync> asyncMessageHandlers)
    {
      registrar.Register(messageRecipients, asyncMessageHandlers);
    }
  }
}