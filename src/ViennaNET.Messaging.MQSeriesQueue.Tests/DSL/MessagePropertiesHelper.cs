using System.Collections;
using System.Collections.Generic;
using IBM.XMS;
using ViennaNET.Utils;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests.DSL
{
  internal static class MessagePropertiesHelper
  {
    public static IEnumerable<KeyValuePair<string, object>> GetProperties(this IMessage message)
    {
      return new PropertyEnumerable(message.ThrowIfNull(nameof(message)));
    }

    private class PropertyEnumerator : IEnumerator<KeyValuePair<string, object>>
    {
      private readonly IEnumerator _enumerator;
      private readonly IMessage _message;

      public PropertyEnumerator(IMessage message)
      {
        _message = message;
        _enumerator = message.PropertyNames;
        _enumerator.Reset();
      }

      public bool MoveNext()
      {
        return _enumerator.MoveNext();
      }

      public void Reset()
      {
        _enumerator.Reset();
      }

      public KeyValuePair<string, object> Current
      {
        get
        {
          var propName = (string)_enumerator.Current;
          var propVal = _message.GetObjectProperty(propName);
          return new KeyValuePair<string, object>(propName, propVal);
        }
      }

      object IEnumerator.Current => Current;

      public void Dispose()
      {
      }
    }

    private class PropertyEnumerable : IEnumerable<KeyValuePair<string, object>>
    {
      private readonly PropertyEnumerator _enumerator;

      public PropertyEnumerable(IMessage message)
      {
        _enumerator = new PropertyEnumerator(message);
      }

      public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
      {
        return _enumerator;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }
    }
  }
}