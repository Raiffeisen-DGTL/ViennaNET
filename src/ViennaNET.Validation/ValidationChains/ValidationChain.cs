using System;
using System.Collections;
using System.Collections.Generic;

namespace ViennaNET.Validation.ValidationChains
{
  internal class ValidationChain<T> : IEnumerable<T>, IEnumerable
  {
    private readonly List<T> _chain = new List<T>();

    public IEnumerator<T> GetEnumerator()
    {
      return _chain.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public event EventHandler<EventArgs<T>> ItemAdded;

    public void Add(T item)
    {
      _chain.Add(item);
      var handler = ItemAdded;
      handler?.Invoke(this, new EventArgs<T>(item));
    }

    public IDisposable OnItemAdded(EventHandler<EventArgs<T>> onItemAdded)
    {
      ItemAdded += onItemAdded;
      return new EventDisposable(this, onItemAdded);
    }

    public IList<T> Clone()
    {
      lock (_chain)
      {
        var result = new List<T>();
        result.AddRange(_chain);
        return result;
      }
    }

    public void Remove(T item)
    {
      _chain.Remove(item);
    }

    private class EventDisposable : IDisposable
    {
      private readonly EventHandler<EventArgs<T>> _handler;
      private readonly ValidationChain<T> _parent;

      public EventDisposable(ValidationChain<T> parent, EventHandler<EventArgs<T>> handler)
      {
        _parent = parent;
        _handler = handler;
      }

      public void Dispose()
      {
        _parent.ItemAdded -= _handler;
      }
    }
  }
}
