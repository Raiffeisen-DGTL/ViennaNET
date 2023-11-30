using System;
using System.Collections;
using System.Collections.Generic;
using Apache.NMS;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests.DSL
{
  internal class IPrimitiveMapMock : IPrimitiveMap
  {
    private readonly Dictionary<string, object> _values = new();

    public void Clear()
    {
      throw new NotImplementedException();
    }

    public bool Contains(object key)
    {
      throw new NotImplementedException();
    }

    public void Remove(object key)
    {
      throw new NotImplementedException();
    }

    public string GetString(string key)
    {
      throw new NotImplementedException();
    }

    public void SetString(string key, string value)
    {
      _values.Add(key, value);
    }

    public bool GetBool(string key)
    {
      throw new NotImplementedException();
    }

    public void SetBool(string key, bool value)
    {
      throw new NotImplementedException();
    }

    public byte GetByte(string key)
    {
      throw new NotImplementedException();
    }

    public void SetByte(string key, byte value)
    {
      throw new NotImplementedException();
    }

    public char GetChar(string key)
    {
      throw new NotImplementedException();
    }

    public void SetChar(string key, char value)
    {
      throw new NotImplementedException();
    }

    public short GetShort(string key)
    {
      throw new NotImplementedException();
    }

    public void SetShort(string key, short value)
    {
      throw new NotImplementedException();
    }

    public int GetInt(string key)
    {
      throw new NotImplementedException();
    }

    public void SetInt(string key, int value)
    {
      throw new NotImplementedException();
    }

    public long GetLong(string key)
    {
      throw new NotImplementedException();
    }

    public void SetLong(string key, long value)
    {
      throw new NotImplementedException();
    }

    public float GetFloat(string key)
    {
      throw new NotImplementedException();
    }

    public void SetFloat(string key, float value)
    {
      throw new NotImplementedException();
    }

    public double GetDouble(string key)
    {
      throw new NotImplementedException();
    }

    public void SetDouble(string key, double value)
    {
      throw new NotImplementedException();
    }

    public IList GetList(string key)
    {
      throw new NotImplementedException();
    }

    public void SetList(string key, IList list)
    {
      throw new NotImplementedException();
    }

    public void SetBytes(string key, byte[] value)
    {
      throw new NotImplementedException();
    }

    public void SetBytes(string key, byte[] value, int offset, int length)
    {
      throw new NotImplementedException();
    }

    public byte[] GetBytes(string key)
    {
      throw new NotImplementedException();
    }

    public IDictionary GetDictionary(string key)
    {
      throw new NotImplementedException();
    }

    public void SetDictionary(string key, IDictionary dictionary)
    {
      throw new NotImplementedException();
    }

    public int Count => _values.Count;
    public ICollection Keys => _values.Keys;
    public ICollection Values { get; }

    public object this[string key]
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }
  }
}