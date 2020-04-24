using System;
using System.Collections.Generic;
using System.Linq;

namespace ViennaNET.TestUtils.Orm.NhQueryable
{
  internal static class TypeSystemHelper
  {
    internal static Type GetElementType(Type seqType) =>
      FindIEnumerable(seqType) is Type foundType
        ? foundType.GetGenericArguments()[0]
        : seqType;

    private static Type FindIEnumerable(Type seqType)
    {
      return seqType.GetInterfaces().SingleOrDefault(
        @interface => @interface.IsGenericType
                      && @interface.IsAssignableFrom(
                        typeof(IEnumerable<>).MakeGenericType(@interface.GetGenericArguments())));
    }
  }
}