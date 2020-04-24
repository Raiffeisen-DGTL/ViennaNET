using System;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace ViennaNET.Orm.Conventions
{
  public class IntToString : IUserType
  {
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

    private static bool IsInt(object value)
    {
      int unsusedVal;
      return int.TryParse(value.ToString(), out unsusedVal);
    }

    public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
      var value = NHibernateUtil.String.NullSafeGet(rs, names[0], session);
      return int.TryParse(value.ToString(), out var unsusedVal) ? unsusedVal : 0;
    }

    public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
    {
      NHibernateUtil.String.NullSafeSet(cmd, (int)value, index, session);
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

    public SqlType[] SqlTypes => new[] { NHibernateUtil.String.SqlType };

    public Type ReturnedType => typeof(string);

    public bool IsMutable => false;
  }
}