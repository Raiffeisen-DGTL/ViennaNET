using System;
using System.Data.Common;
using FluentNHibernate.Conventions;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace ViennaNET.Orm.Conventions
{
  public class StringToEnum<T> : IUserType
  {
    public StringToEnum()
    {
      ReturnedType = typeof(T);
      SqlTypes = new[] { NHibernateUtil.String.SqlType };
      IsMutable = false;
    }

    public new bool Equals(object x, object y)
    {
      if (ReferenceEquals(x, y))
      {
        return true;
      }
      if (x == null || y == null)
      {
        return false;
      }
      return x.Equals(y);
    }

    public int GetHashCode(object x)
    {
      return x.GetHashCode();
    }

    public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
      if (!(NHibernateUtil.String.NullSafeGet(rs, names[0], session) is string value))
      {
        return null;
      }
      value = value.Trim();
      return value.IsEmpty()
        ? null
        : Enum.Parse(ReturnedType, value);
    }

    public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
    {
      object valueToSet = value?.ToString();
      NHibernateUtil.String.NullSafeSet(cmd, valueToSet, index, session);
    }

    public object DeepCopy(object value)
    {
      return value;
    }

    public object Replace(object original, object target, object owner)
    {
      return original;
    }

    public object Assemble(object cached, object owner)
    {
      return DeepCopy(cached);
    }

    public object Disassemble(object value)
    {
      return DeepCopy(value);
    }

    public SqlType[] SqlTypes { get; }
    public Type ReturnedType { get; }
    public bool IsMutable { get; }
  }
}
