using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data.Common;

namespace ViennaNET.Orm.Conventions
{
  public class BoolToDecimal : IUserType
  {
    public new bool Equals(object x, object y)
    {
      if (x == null && y == null)
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
      return x == null
        ? 0
        : x.GetHashCode();
    }

    public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
      var value = NHibernateUtil.Decimal.NullSafeGet(rs, names[0], session);

      if (value == null)
      {
        return null;
      }

      if ((decimal)value == decimal.One)
      {
        return true;
      }

      if ((decimal)value == decimal.Zero)
      {
        return false;
      }

      throw new InvalidOperationException($"Can't convert value {value} to bool type");
    }

    public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
    {
      if (value == null)
      {
        NHibernateUtil.Int32.NullSafeSet(cmd, null, index, session);
      }
      else
      {
        var valueToSet = (bool)value ? decimal.One : decimal.Zero;
        NHibernateUtil.Int32.NullSafeSet(cmd, valueToSet, index, session);
      }
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

    public SqlType[] SqlTypes => new[] {NHibernateUtil.Decimal.SqlType};

    public Type ReturnedType => typeof (bool);

    public bool IsMutable => false;
  }
}