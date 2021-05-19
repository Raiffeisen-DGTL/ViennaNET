using System;
using System.Collections.Generic;
using ViennaNET.Messaging.Factories;

namespace ViennaNET.Messaging.Tests.Unit.DSL
{
  internal class MessageAdapterConstructorMock : IMessageAdapterConstructor
  {
    private readonly Dictionary<string, IMessageAdapter> _adapters = new Dictionary<string, IMessageAdapter>();

    public bool HasQueue(string queueId)
    {
      return _adapters.ContainsKey(queueId);
    }

    public IMessageAdapter Create(string queueId)
    {
      _adapters.TryGetValue(queueId, out var adapter);
      return adapter;
    }

    public IReadOnlyCollection<IMessageAdapter> CreateAll()
    {
      return _adapters.Values;
    }

    public MessageAdapterConstructorMock WithAdapter(IMessageAdapter adapter, string queueId)
    {
      _adapters.Add(queueId, adapter);
      return this;
    }

    public MessageAdapterConstructorMock WithAdapter(Func<MessageAdapterBuilder, MessageAdapterBuilder> builder,
      string queueId)
    {
      return WithAdapter(b => builder(b).Please(), queueId);
    }

    public MessageAdapterConstructorMock WithAdapter(Func<MessageAdapterBuilder, IMessageAdapter> builder,
      string queueId)
    {
      _adapters.Add(queueId, builder(new MessageAdapterBuilder()));
      return this;
    }
  }
}