using System;

namespace ViennaNET.ArcSight
{
  internal static class StringExtensions
  {
    public static string IfNotNullOrWhitespace(this string s, Func<string, string> action)
    {
      return string.IsNullOrWhiteSpace(s)
        ? s
        : action(s);
    }

    public static string EnsureMaxLength(this string s, int maxLength)
    {
      return string.IsNullOrWhiteSpace(s) ? s : s.Length > maxLength ? s.Substring(0, maxLength) : s;
    }
  }
}